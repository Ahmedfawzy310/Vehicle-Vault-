using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using VehicleVault.Core.DTOS.Auth;
using VehicleVault.Core.Settings;
using VehicleVault.Ef.Data;

namespace VehicleVault.Ef.Repositories
{
    public class UserServices : IUserServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IUnitOfWork _unitOfWork;


        public UserServices(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;

        }



        #region Register
        public async Task<AuthModel> RegisterAsync([FromForm] RegisterModel model)
        {
            var email = await _userManager.FindByEmailAsync(model.Email);


            if (email is not null && email.EmailConfirmed)
                return new AuthModel { Message = "Email is already registered!" };

            if (email is not null && !email.EmailConfirmed)
                await _userManager.DeleteAsync(email);



            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                StateId=model.StateId
            };

            var confirmationCode = GenerateRandomCode();
            user.ConfirmationCode = confirmationCode;

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }
            else
            {
                var filePath = Path.Combine(
                      Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                      "Vehicel Rental",
                      "VehicleVault",
                      "VehicleVault.Api",
                      "Templates",
                      "Confirmation.html"); var str = new StreamReader(filePath);

                var mailText = await str.ReadToEndAsync();
                str.Close();
                mailText = mailText.Replace("[Username]", model.FullName)
                    .Replace("[Code]", user.ConfirmationCode);


                await _unitOfWork.MailServices.SendEmailAsync(model.Email, "Confirm Email", mailText, null, confirmationCode);

                return new AuthModel { Message = "Check mail for confirmation" };
            }


        }

        public async Task<AuthModel> RegisteConfirm(string userId, string code)
        {
            if (userId == null || code == null)
                return new AuthModel { Message = "Invalid" };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.ConfirmationCode != code)
            {
                return new AuthModel { Message = "Invalid user or confirmation Code" };
            }

            user.EmailConfirmed = true;
            user.ConfirmationCode = null;

            await _userManager.AddToRoleAsync(user, "User");


            var jwtSecurityToken = await CreateJwtToken(user);

            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            return new AuthModel
            {
                Email = user.Email,
                ExpiresOn = jwtSecurityToken.ValidTo,
                IsAuthenticated = true,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Username = user.UserName,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };

        }

        #endregion

       


        #region Login
        public async Task<AuthModel> GetTokenAsync([FromForm]TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Email or Password is incorrect!";
                return authModel;
            }
            if (!user.EmailConfirmed)
            {
                return new AuthModel { Message = "Email need to confirm" };
            }

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpiresOn = jwtSecurityToken.ValidTo;
            authModel.Roles = rolesList.ToList();
            authModel.StateId = user.StateId;

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }
        #endregion

        #region JWT

        #region CreateToken
        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.PrimarySid, user.Id),

            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwt.DurationInHours),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }
        #endregion

        #region RefreshToken
        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            {
                authModel.Message = "Invalid token";
                return authModel;
            }

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }
        #endregion     

        #region GenerateRefreshToken
        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };
        }


        #endregion
        #endregion

        #region Revoke Token As Logout
        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);
            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return true;
        }
        #endregion




        #region RessetPassword
        public async Task<bool> SendResetPasswordCodeAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // User not found
                return false;
            }

            var code = GenerateRandomCode(); // Generate a random code
            user.ResetPasswordCode = code;

            // Save the code to the user entity (assuming you have a property like ResetPasswordCode in your ApplicationUser model)

            await _userManager.UpdateAsync(user);


            var filePath = Path.Combine(
                      Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                      "Vehicel Rental",
                      "VehicleVault",
                      "VehcileVault.Api",
                      "Templates",
                      "Resset.html"); var str = new StreamReader(filePath);

            var mailText = await str.ReadToEndAsync();
            str.Close();
            mailText = mailText.Replace("[Username]", user.UserName)
                .Replace("[Code]", user.ResetPasswordCode);


            await _unitOfWork.MailServices.SendEmailAsync(email, "Resset Password", mailText, null, user.ResetPasswordCode);


            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || user.ResetPasswordCode != code)
            {
                return false;
            }

            var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), newPassword);

            if (result.Succeeded)
            {
                user.ResetPasswordCode = null;
                await _userManager.UpdateAsync(user);
            }

            return result.Succeeded;
        }
        #endregion

       
        private string GenerateRandomCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }


    }
}
