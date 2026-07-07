using cCoder.Data.Models.Packaging;

namespace cCoder.Logging.Exposures.Setup;

public static partial class UIBaseline
{
    static Package Pages => new()
    {
        Name = "Logging Pages",
        Category = "Logging",
        Description = "Logging Pages.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Admin/FullLogStream",
  "Name": "Full Log Stream",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-09-06T15:26:50.9276367+01:00",
  "Layout": "Default",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "[component[LogStream]]"
    },
    {
      "CultureId": "en-GB",
      "Name": "body",
      "Html": " [component[LogStream]]"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "Full Log Stream for all portals again",
      "Keywords": "",
      "Title": "Full Log Stream"
    }
  ]
}
"""
            },
        ]
    };
}