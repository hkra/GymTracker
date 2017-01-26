using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GymTracker.Exceptions;
using GymTracker.Extensions;
using GymTracker.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GymTracker.Repositories
{
    public class GitRepository : IGitRepository
    {
        private readonly HttpClient _client;
        private readonly string _userName;
        private readonly string _repository;
        private readonly string _accessToken;

        public GitRepository(HttpClient client, IOptions<Settings> settings)
        {
            _client = client;
            _userName = settings.Value.Git.UserName;
            _repository = settings.Value.Git.Repository;
            _accessToken = settings.Value.Git.AccessToken;
        }

        public async Task<string> GetReadmeAsString()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetReadmeUrl(_userName, _repository)).Authorize(_accessToken);
            var response = await _client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new GitSourceException($"README fetch operation returned with status ({response.StatusCode})");
            }

            var encodedContent = JsonConvert.DeserializeObject<GitFile>(await response.Content.ReadAsStringAsync());
            if (encodedContent.Encoding != "base64")
            {
                throw new GitSourceException($"Unexpected README file encoding ({encodedContent.Encoding}; expected base64)");
            }

            return encodedContent.Content.FromBase64();
        }

        public async Task<IEnumerable<GitCommitInfo>> GetCommits()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GetCommitsUrl(_userName, _repository)).Authorize(_accessToken);
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new GitSourceException($"Commits fetch operation returned with status ({response.StatusCode})");
            }

            var contentString = await response.Content.ReadAsStringAsync();
            var commitInfoList = JsonConvert.DeserializeObject<IEnumerable<GitCommitInfo>>(contentString);
            return commitInfoList;
        }

        public async Task<string> CreateNewReadmeTree(string baseTreeHash, string readmeContent)
        {
            var content = JsonConvert.SerializeObject(new GitChangeTree
            {
                BaseTree = baseTreeHash,
                Tree = new[]
                {
                    new GitChangeTreeContent { Path = "README.md", Content = readmeContent }
                }
            });

            var request = new HttpRequestMessage(HttpMethod.Post, GetGitTreeUrl(_userName, _repository)) 
            { 
                Content = new StringContent(content)
            };

            var response = await _client.SendAsync(request.Authorize(_accessToken));
            if (!response.IsSuccessStatusCode)
            {
                throw new GitSourceException($"Create readme tree operation returned with status ({response.StatusCode})");
            }

            var respContentString = await response.Content.ReadAsStringAsync();
            var responseContent = JsonConvert.DeserializeObject<GitTree>(respContentString);
            return responseContent.Sha;
        }

        public async Task<string> CreateNewCommit(string parentHash, string tree, string message)
        {
            var commitContent = JsonConvert.SerializeObject(new GitCreateCommit
            {
                Parents = new[] { parentHash },
                Tree = tree,
                Message = message
            });

            var request = new HttpRequestMessage(HttpMethod.Post, GetCommitsGitUrl(_userName, _repository)) 
            { 
                Content = new StringContent(commitContent)
            };

            var response = await _client.SendAsync(request.Authorize(_accessToken));
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new GitSourceException($"Create commit operation returned with status ({response.StatusCode}) and body ({body})");
            }

            var responseContent = JsonConvert.DeserializeObject<GitCommit>(await response.Content.ReadAsStringAsync());
            return responseContent.Sha;
        }

        public async Task PatchRefToCommit(string commitHash, string refPath = "refs/heads/master")
        {
            var commitContent = JsonConvert.SerializeObject(new GitCommit { Sha = commitHash });
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, GetPatchReferenceUrl(_userName, _repository, refPath))
            { 
                Content = new StringContent(commitContent)
            };

            var response = await _client.SendAsync(request.Authorize(_accessToken));
            if (!response.IsSuccessStatusCode)
            {
                throw new GitSourceException($"Create commit operation returned with status ({response.StatusCode})");
            }
        }

        private string GetReadmeUrl(string userName, string repoName)
        {
            return $"https://api.github.com/repos/{userName}/{repoName}/readme";
        }

        private string GetCommitsUrl(string userName, string repoName)
        {
            return $"https://api.github.com/repos/{userName}/{repoName}/commits";
        }

        private string GetCommitsGitUrl(string userName, string repoName)
        {
            return $"https://api.github.com/repos/{userName}/{repoName}/git/commits";
        }

        private string GetGitTreeUrl(string userName, string repoName)
        {
            return $"https://api.github.com/repos/{userName}/{repoName}/git/trees";
        }

        private string GetPatchReferenceUrl(string userName, string repoName, string refPath)
        {
            return $"https://api.github.com/repos/{userName}/{repoName}/git/{refPath}";
        }
    }
}