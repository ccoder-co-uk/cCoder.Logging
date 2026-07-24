// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Data.Models.Packaging;

namespace cCoder.Logging.Exposures.Setup;

public static partial class UIBaseline
{
    static Package PageRoles => new()
    {
        Name = "Logging Page Roles",
        Category = "Logging",
        Description = "Logging Page Roles.",
        SourceApi = "https://ccoder.co.uk/Api/",
        Items =
        [
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Admin/FullLogStream",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/LogStream",
  "Role": "Administrators"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/LogStream",
  "Role": "Users"
}
"""
            },
            new PackageItem
            {
                Type = "Core/PageRole",
                Data = """
{
  "Path": "Documentation/CoreDocumentation/AppManagement/LogStream",
  "Role": "Guests"
}
"""
            }
        ]
    };
}