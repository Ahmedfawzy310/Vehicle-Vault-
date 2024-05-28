namespace VehicleVault.Core.Settings
{
    public static class VehiclesSettings
    {
        public const string ImagePath = "/assets/images/games";
        public const string AllowdExtension = ".jpg,.jpeg,.png,.jfif";
        public const int AllowdFileSizeByMB = 3;
        public const int AllowdFileSizeByByte = AllowdFileSizeByMB * 1024 * 1024;
    }
}
