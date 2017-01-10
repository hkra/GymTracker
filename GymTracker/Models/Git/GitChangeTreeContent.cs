namespace GymTracker.Models
{
    public class GitChangeTreeContent
    {
        public string Path { get; set; }
        public string Content { get; set; }
        public string Mode { get; set; } = "100644";
        public string Type { get; set; } = "blob";
    }
}