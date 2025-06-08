namespace TaskManager.Domain.InputModels
{
    public class CreateCommentInputModel
    {
        public string Content { get; set; } = string.Empty;
        public Guid TaskItemId { get; set; }
        public Guid UserId { get; set; }
    }
}
