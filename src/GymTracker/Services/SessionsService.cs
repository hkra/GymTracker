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
        // GitHub doesn't respect normal line breaks, so add a couple of spaces at the end of
        // of the line to indicate a line break.
        private const string LineBreakIndicator = "  ";
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

            var readme = new StringBuilder(await _repository.GetReadmeAsString())
                .Append(sessionDate.ToString("yyyy/MM/dd"))
                .Append(LineBreakIndicator)
                .Append("\n")
                .ToString();
            var commitDateString = sessionDate.ToString("MMMM dd, yyyy");
            var commitMessage = $"Gym attendance for {commitDateString}";

            var tree = await _repository.CreateNewReadmeTree(latestCommit.Commit.Tree.Sha, readme);
            var newCommit = await _repository.CreateNewCommit(latestCommit.Sha, tree, commitMessage);
            await _repository.PatchRefToCommit(newCommit);
        }
    }
}