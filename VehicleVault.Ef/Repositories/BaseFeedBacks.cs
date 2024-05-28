using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleVault.Core.DTOS;
using VehicleVault.Ef.Data;

namespace VehicleVault.Ef.Repositories
{
    public class BaseFeedBacks : BaseRepository<FeedBack>, IBaseFeedBacks
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public BaseFeedBacks(ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<string> AddCommentAsync(FeedBackDto dto)
        {
            // Check if the user is allowed to add a comment
            bool canAddComment = await _unitOfWork.BaseRentals.CanUserAddComment(dto.CreatedBy, dto.VehicleId);

            if (canAddComment)
            {
                // Add the comment to the database
                var newFeedback = new FeedBack
                {
                    Comment = dto.Comment,
                    CreatedDate = DateTime.Now,
                    CreatedBy = dto.CreatedBy,
                    VehicleId = dto.VehicleId,
                };
                await _unitOfWork.BaseFeedBacks.CreateAsync(newFeedback);
                _unitOfWork.Complete();
                return "Ok";
            }
            else
            {
                throw new Exception("User is not allowed to add a comment for this car.");
            }

        }

        public async Task<bool> UpdateComment(int id, UpdateCommentDto dto)
        {
            var comment = await _unitOfWork.BaseFeedBacks.GetByID(i => i.Id == id);
            if (comment == null)
                return false;


            if (dto.UpdatedBy != comment.CreatedBy)
                return false;

            comment.Comment = dto.Comment;
            comment.UpdatedDate = DateTime.Now;
            _unitOfWork.Complete();

            return true;
        }
    }
}
