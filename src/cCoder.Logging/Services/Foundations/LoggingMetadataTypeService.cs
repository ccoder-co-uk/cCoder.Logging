// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers.Metadata;
using cCoder.Logging.Models.OData;
using cCoder.Data.Models.Logging;


namespace cCoder.Logging.Services.Foundations;

internal sealed partial class LoggingMetadataTypeService(
    IMetadataBroker metadataBroker) : ILoggingMetadataTypeService
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

    private ExtendedMetadataContainer Entity<T>()
    {
        ExtendedMetadataContainer metadata =
            metadataBroker.CreateEntityMetadata(type: typeof(T));

        metadata.Category = "Logging";

        return metadata;
    }
}