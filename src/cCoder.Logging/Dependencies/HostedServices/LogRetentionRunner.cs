// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Logging.Dependencies.HostedServices;

internal delegate Task LogRetentionRunner(CancellationToken stoppingToken);