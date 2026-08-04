// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using cCoder.Logging.Models.OData;

namespace cCoder.Logging.Extensions.OData;

internal static class PropertyInfoExtensions
{
    private static readonly Dictionary<Type, string> TypeNames = new()
    {
        { typeof(short), "number" },
        { typeof(int), "number" },
        { typeof(long), "number" },
        { typeof(short?), "number" },
        { typeof(int?), "number" },
        { typeof(long?), "number" },
        { typeof(ushort), "number" },
        { typeof(uint), "number" },
        { typeof(ulong), "number" },
        { typeof(ushort?), "number" },
        { typeof(uint?), "number" },
        { typeof(ulong?), "number" },
        { typeof(byte), "number" },
        { typeof(byte?), "number" },
        { typeof(decimal), "number" },
        { typeof(decimal?), "number" },
        { typeof(string), "string" },
        { typeof(DateTime), "date" },
        { typeof(DateTime?), "date" },
        { typeof(TimeSpan), "time" },
        { typeof(TimeSpan?), "time" },
        { typeof(DateTimeOffset), "date" },
        { typeof(DateTimeOffset?), "date" },
        { typeof(Guid), "guid" },
        { typeof(Guid?), "guid" },
        { typeof(bool), "bool" },
        { typeof(bool?), "bool" },
        { typeof(double), "number" },
        { typeof(double?), "number" },
        { typeof(float), "number" },
        { typeof(float?), "number" }
    };

    internal static PropertyContainer CreatePropertyContainer(
        this PropertyInfo property) =>
        new()
        {
            Name = property.Name,
            Type = GetTypeName(type: property.PropertyType),
            ServerType = property.PropertyType.ToString(),
            ServerTypeName = property.PropertyType.GetCSharpTypeName(),
            IsValueType = property.PropertyType.IsValueType || property.PropertyType == typeof(string),
            DisplayName = property.Name,
            ShortDisplayName = property.Name,
            Description = property.Name,
            IsReadOnly = !property.CanWrite,
            Template = property.GetCustomAttribute<KeyAttribute>() is not null || property.Name == "Id"
                ? "key"
                : property.Name,
            IsRequired = (!(property.PropertyType.IsGenericType
                    && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                && property.PropertyType.IsValueType)
                || property.GetCustomAttribute<RequiredAttribute>() is not null
        };

    internal static MetadataContainer CreateMetadataContainer(
        Type type,
        bool isEntity = false,
        bool hasEndpoint = false) =>
        PopulateMetadataContainer(
            metadata: new MetadataContainer(),
            type: type,
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

    internal static ExtendedMetadataContainer CreateExtendedMetadataContainer(
        Type type,
        bool isEntity = false,
        bool hasEndpoint = false) =>
        PopulateMetadataContainer(
            metadata: new ExtendedMetadataContainer(),
            type: type,
            isEntity: isEntity,
            hasEndpoint: hasEndpoint);

    internal static string GetTypeName(Type type)
    {
        if (type == typeof(string))
        {
            return "string";
        }

        if (typeof(IEnumerable).IsAssignableFrom(c: type))
        {
            return "array";
        }

        return TypeNames.TryGetValue(key: type, value: out string name)
            ? name
            : "object";
    }

    private static TMetadata PopulateMetadataContainer<TMetadata>(
        TMetadata metadata,
        Type type,
        bool isEntity,
        bool hasEndpoint)
        where TMetadata : MetadataContainer
    {
        bool isValueType = type.IsValueType || type == typeof(string);

        metadata.IsValueType = isValueType;
        metadata.Type = GetTypeName(type: type);
        metadata.Name = type.Name;
        metadata.DisplayName = type.Name;
        metadata.Description = type.Name;
        metadata.ServerType = type.AssemblyQualifiedName;
        metadata.ServerTypeName = type.GetCSharpTypeName();

        metadata.Properties = isValueType
            ? []
            : type.GetProperties()
                .Select(selector: CreatePropertyContainer)
                .ToArray();

        metadata.IsEntity = isEntity;
        metadata.IsJoinEntity = isEntity && type.IsJoinType();
        metadata.HasEndpoint = hasEndpoint;

        return metadata;
    }
}