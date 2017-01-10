using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymTracker.Exceptions;
using GymTracker.Repositories;

namespace GymTracker.Services
{
    public class SessionsSerivce : ISessionsSerivce
    {
        private readonly IGitRepository _repository;
        
        public SessionsSerivce(IGitRepository repository)
        {
            _repository = repository;
        }

        public async Task RecordSession(DateTimeOffset sessionDate)
        {
            var latestCommit = (await _repository.GetCommits()).FirstOrDefault();
            if (latestCommit == null)
            {
                throw new RepositoryException("No commits present. Has the repository been initialized with a README.md?");
            }

            var now = DateTimeOffset.UtcNow;
            var readme = new StringBuilder(await _repository.GetReadmeAsString())
                .Append(now.ToString("YYYY/MM/dd"))
                .Append("\n")
                .ToString();
            var commitDateString = now.ToString("MMMM dd, YYYY");
            var commitMessage = $"Gym attendance for {commitDateString}";

            var tree = await _repository.CreateNewReadmeTree(latestCommit.Commit.Tree.Sha, readme);
            var newCommit = await _repository.CreateNewCommit(latestCommit.Sha, tree, commitMessage);
            await _repository.PatchRefToCommit(newCommit);
        }
    }
}