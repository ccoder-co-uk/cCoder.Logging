// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

using cCoder.Logging.Models.OData;

namespace cCoder.Logging.Brokers.Metadata;

internal interface IMetadataBroker
{
    ExtendedMetadataContainer CreateEntityMetadata(Type type);
}