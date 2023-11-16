// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.


using System.Diagnostics.CodeAnalysis;

namespace ActivityPub.Types.AS.Extended.Activity;

/// <summary>
///     An IntransitiveActivity that indicates that the actor has arrived at the location.
///     The origin can be used to identify the context from which the actor originated.
///     The target typically has no defined meaning.
/// </summary>
public class ArriveActivity : ASIntransitiveActivity, IASModel<ArriveActivity, ArriveActivityEntity, ASIntransitiveActivity>
{
    public const string ArriveType = "Arrive";
    static string IASModel<ArriveActivity>.ASTypeName => ArriveType;

    public ArriveActivity() : this(new TypeMap()) {}

    public ArriveActivity(TypeMap typeMap) : base(typeMap)
    {
        Entity = new ArriveActivityEntity();
        TypeMap.Add(Entity);
    }

    public ArriveActivity(ASType existingGraph) : this(existingGraph.TypeMap) {}

    [SetsRequiredMembers]
    public ArriveActivity(TypeMap typeMap, ArriveActivityEntity? entity) : base(typeMap, null)
        => Entity = entity ?? typeMap.AsEntity<ArriveActivityEntity>();

    static ArriveActivity IASModel<ArriveActivity>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    private ArriveActivityEntity Entity { get; }
}

/// <inheritdoc cref="ArriveActivity" />
public sealed class ArriveActivityEntity : ASEntity<ArriveActivity, ArriveActivityEntity> {}