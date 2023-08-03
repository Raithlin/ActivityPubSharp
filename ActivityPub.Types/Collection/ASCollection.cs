// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using ActivityPub.Types.Attributes;
using ActivityPub.Types.Util;
using static ActivityPub.Types.Collection.CollectionTypes;

namespace ActivityPub.Types.Collection;

// TODO custom conversion for Ordered

/// <summary>
/// A Collection is a subtype of Object that represents ordered or unordered sets of Object or Link instances.
/// May be paged or unpaged, and ordered or unordered. 
/// </summary>
/// <remarks>
/// Refer to the <a href="https://www.w3.org/TR/activitystreams-core/#collection">Activity Streams 2.0 Core specification</a> for a complete description of the Collection type.
/// </remarks>
/// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-collection"/>
/// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-orderedcollection"/>
public class ASCollection : ASObject
{
    private ASCollectionEntity Entity { get; }


    public ASCollection() => Entity = new ASCollectionEntity(TypeMap);
    public ASCollection(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<ASCollectionEntity>();


    /// <summary>
    /// Indicates if this collection is ordered or unordered.
    /// If true, this will convert to an Ordered collection type.
    /// </summary>
    /// <remarks>
    /// This is a synthetic type that does not exist in the ActivityStreams specification.
    /// </remarks>
    public bool IsOrdered
    {
        get => Entity.IsOrdered;
        set => Entity.IsOrdered = value;
    }

    /// <summary>
    ///  In a paged Collection, indicates the page that contains the most recently updated member items. 
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-current"/>
    public Linkable<ASCollectionPage>? Current
    {
        get => Entity.Current;
        set => Entity.Current = value;
    }

    /// <summary>
    /// In a paged Collection, indicates the furthest preceding page of items in the collection. 
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-first"/>
    public Linkable<ASCollectionPage>? First
    {
        get => Entity.First;
        set => Entity.First = value;
    }

    /// <summary>
    /// In a paged Collection, indicates the furthest proceeding page of the collection.
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-last"/>
    public Linkable<ASCollectionPage>? Last
    {
        get => Entity.Last;
        set => Entity.Last = value;
    }

    /// <summary>
    /// A non-negative integer specifying the total number of objects contained by the logical view of the collection.
    /// This number might not reflect the actual number of items serialized within the Collection object instance. 
    /// </summary>
    /// <remarks>
    /// If not explicitly set, then this will default to the number of elements in <see cref="Items"/>.
    /// Setting this to any value will disable this logic.
    /// If neither is set, returns zero.
    /// This cannot be set to less than zero.
    /// </remarks>
    public int TotalItems
    {
        get => Entity.TotalItems;
        set => Entity.TotalItems = value;
    }

    /// <summary>
    /// Identifies the items contained in a collection.
    /// The items might be ordered or unordered. 
    /// </summary>
    /// <remarks>
    /// In ordered collection types, this will map to "orderedItems".
    /// Otherwise, it maps to "items".
    /// In a paged collection, this MAY be null
    /// </remarks>
    /// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-items"/>
    public LinkableList<ASObject>? Items
    {
        get => Entity.Items;
        set => Entity.Items = value;
    }

    /// <summary>
    /// True if this is a paged collection, false otherwise.
    /// </summary>
    public bool IsPaged => Entity.IsPaged;

    /// <summary>
    /// True if this collection instance contains items, false otherwise.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Items))]
    public bool HasItems => Entity.HasItems;

    public static implicit operator ASCollection(List<ASObject> collection) => new() { Items = new(collection) };
}

/// <inheritdoc cref="ASCollection"/>
[ASTypeKey(CollectionType)]
[ASTypeKey(OrderedCollectionType)]
[ImpliesOtherEntity(typeof(ASObjectEntity))]
public sealed class ASCollectionEntity : ASBase<ASCollection>
{
    /// <inheritdoc cref="ASBase{T}(string?, TypeMap)"/>
    public ASCollectionEntity(TypeMap typeMap) : base(CollectionType, typeMap) {}

    /// <inheritdoc cref="ASBase{T}(string?)"/>
    [JsonConstructor]
    public ASCollectionEntity() : base(CollectionType) {}

    /// <inheritdoc cref="ASCollection.Current"/>
    [JsonPropertyName("current")]
    public Linkable<ASCollectionPage>? Current { get; set; }

    /// <inheritdoc cref="ASCollection.First"/>
    [JsonPropertyName("first")]
    public Linkable<ASCollectionPage>? First { get; set; }

    /// <inheritdoc cref="ASCollection.Last"/>
    [JsonPropertyName("last")]
    public Linkable<ASCollectionPage>? Last { get; set; }

    /// <inheritdoc cref="ASCollection.TotalItems"/>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    [Range(0, int.MaxValue)]
    [JsonPropertyName("totalItems")]
    public int TotalItems
    {
        get => _totalItems ?? Items?.Count ?? 0;
        set => _totalItems = Math.Max(value, 0);
    }

    private int? _totalItems;

    /// <inheritdoc cref="ASCollection.Items"/>
    [JsonPropertyName("items")]
    public LinkableList<ASObject>? Items { get; set; }

    /// <inheritdoc cref="ASCollection.IsPaged"/>
    [JsonIgnore]
    public bool IsPaged => Current != null || First != null || Last != null;

    /// <inheritdoc cref="ASCollection.HasItems"/>
    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(Items))]
    public bool HasItems => Items?.Any() == true;

    /// <inheritdoc cref="ASCollection.IsOrdered"/>
    [JsonIgnore] // TODO this should toggle Items between "items" and "orderedItems"
    public bool IsOrdered { get; set; }
}