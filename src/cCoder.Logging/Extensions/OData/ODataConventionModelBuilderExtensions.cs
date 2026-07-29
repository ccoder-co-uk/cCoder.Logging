// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Brokers.OData;
using Microsoft.OData.ModelBuilder;

namespace cCoder.Logging.Extensions.OData;

internal static class ODataConventionModelBuilderExtensions
{
    internal static void ConfigureLoggingApiModel(
        this ODataConventionModelBuilder builder) =>
        new LoggingModelBroker(builder: builder).Configure();
}