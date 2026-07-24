// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Models;
using cCoder.Data.Models.Logging;

namespace cCoder.Logging.Services;

public interface ILogDataItemService
{
    LogDataItem Get(int id);

    IQueryable<LogDataItem> GetAll(bool ignoreFilters = false);

    ValueTask<LogDataItem> AddAsync(LogDataItem logDataItem);

    ValueTask<LogDataItem> UpdateAsync(LogDataItem logDataItem);

    ValueTask DeleteAsync(int id);

    ValueTask<IEnumerable<Result<LogDataItem>>> AddOrUpdate(IEnumerable<LogDataItem> items);

    ValueTask DeleteAllAsync(IEnumerable<LogDataItem> items);
}