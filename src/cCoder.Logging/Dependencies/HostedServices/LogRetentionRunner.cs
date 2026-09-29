// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace cCoder.Logging.Dependencies.HostedServices;

internal delegate Task LogRetentionRunner(CancellationToken stoppingToken);