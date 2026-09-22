// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models.OData;

namespace cCoder.Logging.Brokers.Metadata;

internal interface IMetadataBroker
{
    ExtendedMetadataContainer CreateEntityMetadata(Type type);
}