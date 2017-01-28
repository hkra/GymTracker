namespace GymTracker.Models
{
    public class GitCommitInfo
    {
        public string Sha { get; set; }
        public GitCommit Commit { get; set; }
    }
}