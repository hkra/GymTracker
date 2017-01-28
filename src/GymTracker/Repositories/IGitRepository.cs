using System.Collections.Generic;
using System.Threading.Tasks;
using GymTracker.Models;

namespace GymTracker.Repositories
{
    public interface IGitRepository
    {
        Task<string> GetReadmeAsString();
        Task<IEnumerable<GitCommitInfo>> GetCommits();
        Task<string> CreateNewReadmeTree(string baseTreeHash, string readmeContent);
        Task<string> CreateNewCommit(string parentHash, string tree, string message);
        Task PatchRefToCommit(string commitHash, string refPath = "refs/heads/master");
    }
}