namespace privatenupkgserver.Models.Nuspec;

[Serializable]
[XmlRoot("package")]
public class Nuspec
{
    [XmlElement("metadata")]
    public NuspecMetadataModel? Metadata { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public string? FilePath { get; set; }

    [XmlIgnore]
    [JsonIgnore]
    public DateTimeOffset? PublishTime { get; set; }

    static Nuspec()
    {
        SerializerFactory = new XmlSerializerFactory();
        NuspecSerializer = SerializerFactory.CreateSerializer(typeof(Nuspec));
    }

    private static readonly XmlSerializerFactory SerializerFactory;

    private static readonly XmlSerializer NuspecSerializer;

    public static bool TryParse(byte[] metadata, out Nuspec? nuspec)
    {
        if (metadata == null)
        {
            nuspec = null;
            return false;
        }
        using var inputStream = new MemoryStream(metadata);
        return TryParse(inputStream, out nuspec);
    }

    public static bool TryParse(Stream stream, out Nuspec? nuspec)
    {
        nuspec = null;
        if (stream == null)
            return false;
        try
        {
            using (var xmlReader = new XmlTextReader(stream))
            {
                //annoying diff xmlns
                xmlReader.Namespaces = false;
                nuspec = (Nuspec)NuspecSerializer.Deserialize(xmlReader);
                return true;
            }
        }
        catch
        {
        }
        return false;
    }
}
