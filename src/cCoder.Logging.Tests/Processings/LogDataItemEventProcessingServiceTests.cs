// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;
using cCoder.Logging.Services.Foundations.Events;
using cCoder.Logging.Services.Processings;
using FizzWare.NBuilder;
using Moq;


namespace cCoder.Core.Services.Tests.Logging.Processings;

public partial class LogDataItemEventProcessingServiceTests
{
    private readonly Mock<ILogDataItemEventService> logDataItemEventServiceMock;
    private readonly LogDataItemEventProcessingService service;

    public LogDataItemEventProcessingServiceTests()
    {
        logDataItemEventServiceMock = new Mock<ILogDataItemEventService>(behavior: MockBehavior.Strict);
        service = new LogDataItemEventProcessingService(eventService: logDataItemEventServiceMock.Object);
    }

    private static LogDataItem CreateRandomLogDataItem() =>
        Builder<LogDataItem>.CreateNew()
        .Build();
}