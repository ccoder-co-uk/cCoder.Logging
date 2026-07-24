// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Dependencies.OData;

internal interface ILoggingMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}