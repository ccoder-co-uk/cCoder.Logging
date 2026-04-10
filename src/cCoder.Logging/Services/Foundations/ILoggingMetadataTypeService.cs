using cCoder.Logging.Api.OData;


namespace cCoder.Logging.Services.Foundations;

internal interface ILoggingMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}

