// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Logging.Extensions.OData;

namespace cCoder.Logging.Models.OData;

public class MetadataContainer
{
    public string Type { get; set; }
    public string ServerTypeName { get; set; }
    public bool IsValueType { get; set; }
    public bool IsEntity { get; set; }
    public bool IsJoinEntity { get; set; }
    public bool HasEndpoint { get; set; }
    public bool IsSystemManaged { get; set; }
    public string Category { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public string ServerType { get; set; }
    public IEnumerable<PropertyContainer> Properties { get; set; }

    public MetadataContainer() { }

    public MetadataContainer(Type type)
    {
        IsValueType = type.IsValueType || type == typeof(string);
        Type = PropertyInfoExtensions.GetTypeName(type: type);
        Name = type.Name;
        DisplayName = type.Name;
        Description = type.Name;
        ServerType = type.AssemblyQualifiedName;
        ServerTypeName = type.GetCSharpTypeName();
        Properties = type.IsValueType || type == typeof(string)
            ? []
            : type.GetProperties()
            .Select(selector: PropertyInfoExtensions.CreatePropertyContainer)
            .ToArray();
    }

    public MetadataContainer(Type type, bool isEntity, bool hasEndpoint)
        : this(type: type)
    {
        IsEntity = isEntity;
        IsJoinEntity = isEntity && type.IsJoinType();
        HasEndpoint = hasEndpoint;
    }
}