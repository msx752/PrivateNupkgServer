using System.Collections.Generic;

namespace privatenupkgserver.Models.ServerIndex
{
    public class ServerIndexModel
    {
        public ServerIndexModel(IEnumerable<ServerIndexResourceModel> resources)
        {
            Resources = new HashSet<ServerIndexResourceModel>(resources, ServerIndexResourceModelComparer.Instance);
        }

        public string Version { get; set; }

        public IEnumerable<ServerIndexResourceModel> Resources { get; private set; }

        public ServerIndexContext Context { get; set; }
    }
}