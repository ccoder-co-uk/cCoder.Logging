// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models.OData;

namespace cCoder.Logging.Brokers.OData;

public interface IODataModelBroker
{
    ODataModel Build();
}