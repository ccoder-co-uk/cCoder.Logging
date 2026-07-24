// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            new PackageItem
            {
                Type = "Core/Page",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/LogStream",
  "Name": "Log Stream",
  "ShowOnMenus": true,
  "Order": 0,
  "LastUpdated": "2024-08-22T12:04:12.3419104+01:00",
  "Layout": "Documentation",
  "Contents": [
    {
      "CultureId": "",
      "Name": "body",
      "Html": "<div class=\"documentation\"><h2>Accessing the Log Stream </h2><p class=\"mainText\">You access this functionality through the App Management Tabs. From here, you can see any API\n        calls that are being made in the system at the time.</p><h2>The UI</h2><p class=\"mainText\">When you access the log stream tab, you&rsquo;re greeted with something that looks like this: </p><img src=\"[app[root]]Api/DMS/Content/Documentation/Core Documentation/App Management/Log Stream/Logstream-en.png\" style=\"display:block;margin-left:auto;margin-right:auto;\" /><p class=\"mainText\">Here, you can see the API calls that are made when you hover over the navigation menu in our\n        demo site. The toolbar options at the top of the page allow you toggle on and off &ldquo;Auto Scroll&rdquo;, and select what\n        levels you want to be logged into the console. This allows you to easily debug and see what errors occur from\n        what API calls.\n    </p></div>"
    }
  ],
  "PageInfo": [
    {
      "CultureId": "",
      "Description": "How to access the portal's log stream.",
      "Title": "Log Stream"
    }
  ]
}
"""
            }
        ]
    };
}