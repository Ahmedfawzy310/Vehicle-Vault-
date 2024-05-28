namespace VehicleVault.Core.Interfaces
{
    public interface IBaseFeedBacks : IBaseRepository<FeedBack>
    {
        Task<string> AddCommentAsync(FeedBackDto dto);
        Task<bool> UpdateComment(int id, UpdateCommentDto dto);
    }
}
