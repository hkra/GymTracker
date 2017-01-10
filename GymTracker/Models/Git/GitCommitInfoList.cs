using System.Collections.Generic;

namespace GymTracker.Models
{
    public class GitCommitInfoList
    {
        public IEnumerable<GitCommitInfo> Commits { get; set; }
    }
}