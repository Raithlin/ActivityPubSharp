// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Text.Json;
using System.Text.Json.Serialization;
using ActivityPub.Types.Json;
using ActivityPub.Types.Json.Attributes;
using ActivityPub.Types.Util;

namespace ActivityPub.Types;

/// <summary>
/// Base type for all activities that are not intransitive
/// </summary>
/// <remarks>
/// This is a synthetic type, and not part of the ActivityStreams standard.
/// </remarks>
public class ASTransitiveActivity : ASActivity
{
    private ASTransitiveActivityEntity Entity { get; }


    public ASTransitiveActivity() => Entity = new ASTransitiveActivityEntity(TypeMap)
    {
        Object = null!
    };

    public ASTransitiveActivity(TypeMap typeMap) : base(typeMap) => Entity = TypeMap.AsEntity<ASTransitiveActivityEntity>();


    /// <summary>
    /// Describes the direct object of the activity.
    /// For instance, in the activity "John added a movie to his wishlist", the object of the activity is the movie added. 
    /// </summary>
    /// <seealso href="https://www.w3.org/TR/activitystreams-vocabulary/#dfn-object"/>
    [JsonPropertyName("object")]
    public required LinkableList<ASObject> Object
    {
        get => Entity.Object;
        set => Entity.Object = value;
    }
}

/// <inheritdoc cref="ASTransitiveActivity"/>
[NarrowJsonType(nameof(NarrowType))]
public sealed class ASTransitiveActivityEntity : ASBase
{
    public ASTransitiveActivityEntity(TypeMap typeMap) : base(null, typeMap) {}


    /// <inheritdoc cref="ASTransitiveActivity.Object"/>
    [JsonPropertyName("object")]
    public required LinkableList<ASObject> Object { get; set; } = new();

    /// <inheritdoc cref="NarrowTypeDelegate"/>
    public static Type NarrowType(JsonElement element, DeserializationMetadata meta)
    {
        // If it has the "target" property, then its Targeted.
        var isTransient = element.TryGetProperty("object", out _);

        // Pivot to the correct type.
        return isTransient
            ? typeof(ASTargetedActivityEntity)
            : typeof(ASTransitiveActivityEntity);
    }
}