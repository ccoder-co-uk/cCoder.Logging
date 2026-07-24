// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Api.OData;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations;

internal sealed class LoggingMetadataTypeService : ILoggingMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata() =>
    [
        new MetadataContainerSet
        {
            Name = "Logging",
            UriBase = "Logging",
            Types =
            [
                Entity<LogDataItem>(),
                Entity<LogEntry>(),
            ],
        },
    ];

    private static ExtendedMetadataContainer Entity<T>() =>
        new(typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "Logging",
        };
}