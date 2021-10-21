namespace privatenupkgserver.Models.ServerIndex
{
    public class ServerIndexResourceModel
    {
        public ServerIndexResourceModel()
        {
            Comment = "";
        }

        public string Id { get; set; }

        public string Type { get; set; }

        public string Comment { get; set; }
    }
}