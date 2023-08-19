using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using InternalUtils;

namespace ActivityPub.Types.AS;

// TODO new implementation of unmapped JSON

// TODO consider splitting these into separate interfaces, without null

/// <summary>
///     Indicates that the attached type is an ActivityStreams entity
/// </summary>
public interface IEntity
{
    /// <summary>
    ///     Additional entity types that are implicitly included by this entity.
    ///     When this entity is parsed from JSON, all entities listed here will also be parsed and attached to the type graph.
    /// </summary>
    /// <remarks>
    ///     IMPORTANT: the matching Type (if applicable) should follow this type hierarchy.
    /// </remarks>
    public static virtual IEnumerable<Type>? ImpliedEntities => null;
}

/// <summary>
///     Indicates that the implementing type is an Entity with a linked non-entity type.
/// </summary>
/// <typeparam name="TType">Type of the non-entity</typeparam>
public interface IHasNonEntity<out TType> : IEntity
    where TType : ASType
{
    // One-time step - generate IL to call the type's constructor.
    // Will be null on failure, to avoid throwing a type initialization exception.
    private static readonly Func<TypeMap, TType>? TypeConstructor = TypeUtils.TryCreateDynamicConstructor<TypeMap, TType>();

    /// <summary>
    ///     If applicable, the non-entity Type that can wrap this entity into a higher-level API.
    /// </summary>
    internal static virtual Type NonEntityType { get; } = typeof(TType);
    
    /// <summary>
    ///     Creates an instance of the non-entity type that can wrap this entity.
    ///     Internal use only - this requires careful use.
    ///     Will cause graph corruption if called with the wrong TypeMap.
    /// </summary>
    /// <param name="typeMap">TypeMap that contains this entity</param>
    /// <param name="instance">Instance that was constructed</param>
    internal static virtual bool TryCreateTypeInstance(TypeMap typeMap, [NotNullWhen(true)] out ASType? instance)
    {
        if (TypeConstructor != null)
        {
            instance = TypeConstructor(typeMap);
            return true;
        }
    
        instance = null;
        return false;
    }
}

/// <summary>
///     Indicates that the attached type is an Entity that can be converted from JSON or JSON-LD.
/// </summary>
public interface IConvertibleEntity : IEntity
{
    /// <summary>
    ///     ActivityStreams type name.
    /// </summary>
    public static abstract string ASTypeName { get; }

    /// <summary>
    ///     JSON-LD context that defines this entity.
    ///     The entity will not be converted, even with a matching type, unless this context is present in the JSON.
    ///     If unset (null), then assumed to be the default ActivityStreams context.
    /// </summary>
    public static virtual string? Context => null;

    /// <summary>
    ///     Names of ActivityStreams types that should be shadowed / replaced when this entity is attached to a graph.
    ///     This is processed recursively, so there is no need to enumerate the entire graph.
    /// </summary>
    /// <remarks>
    ///     IMPORTANT: the matching Type (if applicable) should follow this type hierarchy.
    /// </remarks>
    public static virtual IEnumerable<string>? ReplacedASTypes => null;
}

/// <summary>
///     Indicates that this entity is part of the "Link" AS type or an extension of it.
///     Links MUST implement this interface, otherwise conversion may fail.
/// </summary>
public interface ILinkEntity : IEntity
{
    /// <summary>
    ///     True if the link's current state can only be represented by the object form.
    ///     MUST return true if any properties are populated, other than HRef!
    /// </summary>
    [JsonIgnore]
    public bool RequiresObjectForm { get; }
}