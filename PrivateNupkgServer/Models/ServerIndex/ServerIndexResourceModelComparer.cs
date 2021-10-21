using System.Collections.Generic;

namespace privatenupkgserver.Models.ServerIndex
{
    public class ServerIndexResourceModelComparer : IEqualityComparer<ServerIndexResourceModel>
    {
        static ServerIndexResourceModelComparer()
        {
            Instance = new ServerIndexResourceModelComparer();
        }

        public static ServerIndexResourceModelComparer Instance { get; private set; }

        public bool Equals(ServerIndexResourceModel x, ServerIndexResourceModel y)
        {
            return ReferenceEquals(x, y) || string.Equals(x?.Type, y?.Type, System.StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ServerIndexResourceModel obj)
        {
            return obj?.Type?.ToLowerInvariant()?.GetHashCode() ?? 0;
        }
    }
}