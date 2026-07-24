// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Dependencies.OData;


namespace cCoder.Logging.Services.Foundations;

internal interface ILoggingMetadataTypeService
{
    IEnumerable<MetadataContainerSet> GetKnownMetadata();
}