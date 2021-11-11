﻿namespace privatenupkgserver.Models.ServerIndex;

public class ServerIndexContext
{
    static ServerIndexContext()
    {
        Default = new ServerIndexContext
        {
            Vocab = "http://schema.nuget.org/services#",
            Comment = "http://www.w3.org/2000/01/rdf-schema#comment"
        };
    }

    public static readonly ServerIndexContext Default;

    public string? Vocab { get; set; }

    public string? Comment { get; set; }
}
