using System.Collections.Generic;
using Newtonsoft.Json;

namespace GymTracker.Models
{
    public class GitChangeTree
    {
        [JsonProperty("base_tree")]
        public string BaseTree { get; set; }

        public IEnumerable<GitChangeTreeContent> Tree { get; set; }
    }
}