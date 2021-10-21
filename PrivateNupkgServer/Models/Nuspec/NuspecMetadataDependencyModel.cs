using System;
using System.Xml.Serialization;

namespace privatenupkgserver.Models.Nuspec
{
    [Serializable]
    public class NuspecMetadataDependencyModel
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("exclude")]
        public string Exclude { get; set; }
    }
}