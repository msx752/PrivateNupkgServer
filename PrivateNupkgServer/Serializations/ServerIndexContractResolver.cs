using Newtonsoft.Json.Serialization;

namespace privatenupkgserver.Serializations
{
    public class ServerIndexContractResolver : DefaultContractResolver
    {
        public ServerIndexContractResolver()
        {
            NamingStrategy = new ServerIndexNamingStrategy();
        }
    }
}