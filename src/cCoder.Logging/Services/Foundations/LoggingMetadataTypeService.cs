// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Extensions.OData;
using cCoder.Logging.Models.OData;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LoggingMetadataTypeService : ILoggingMetadataTypeService
{
    public IEnumerable<MetadataContainerSet> GetKnownMetadata()
=>
        TryCatch(operation: IEnumerable<MetadataContainerSet> () =>
        {

            return [
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
        });

    private static ExtendedMetadataContainer Entity<T>() =>
        new(type: typeof(T), isEntity: true, hasEndpoint: true)
        {
            Category = "Logging",
        };
}