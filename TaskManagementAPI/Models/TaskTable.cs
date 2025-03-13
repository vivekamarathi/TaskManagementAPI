
namespace TaskManagementAPI.Models
{
    public class TaskTable
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int AssignedUserId { get; set; }
    }
}