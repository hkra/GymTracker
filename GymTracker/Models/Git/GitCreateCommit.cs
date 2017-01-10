namespace GymTracker.Models
{
    public class GitCreateCommit
    {
        public string[] Parents { get; set; }
        public string Tree { get; set; }
        public string Message { get; set; }
    }
}