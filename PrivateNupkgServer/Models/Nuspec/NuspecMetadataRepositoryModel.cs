using System;
using System.Xml.Serialization;

namespace privatenupkgserver.Models.Nuspec
{
    [Serializable]
    public class NuspecMetadataRepositoryModel
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("url")]
        public string Url { get; set; }
    }
}