// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;
using cCoder.Logging.Exposures.Setup;

namespace cCoder.Logging.Exposures;

public interface IBaselineExposure
{
    Package[] GetBaselinePackages();
}

internal sealed class BaselineExposure : IBaselineExposure
{
    public Package[] GetBaselinePackages() =>
        UIBaseline.Packages;
}
