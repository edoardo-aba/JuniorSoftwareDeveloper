namespace TaskManagerApi.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public string TenantId { get; set; } = string.Empty;
         public DateTime CreatedAt { get; set; }  
    }
}