namespace GymTracker.Models
{
    public class GitCommit
    {
        public string Sha { get; set; }
        public GitTree Tree { get; set; }
    }
}