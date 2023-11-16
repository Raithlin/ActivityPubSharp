// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.


using System.Diagnostics.CodeAnalysis;

namespace ActivityPub.Types.AS.Extended.Activity;

/// <summary>
///     A specialization of Offer in which the actor is extending an invitation for the object to the target.
/// </summary>
public class InviteActivity : OfferActivity, IASModel<InviteActivity, InviteActivityEntity, OfferActivity>
{
    public const string InviteType = "Invite";
    static string IASModel<InviteActivity>.ASTypeName => InviteType;

    public InviteActivity() : this(new TypeMap()) {}

    public InviteActivity(TypeMap typeMap) : base(typeMap)
    {
        Entity = new InviteActivityEntity();
        TypeMap.Add(Entity);
    }

    public InviteActivity(ASType existingGraph) : this(existingGraph.TypeMap) {}

    [SetsRequiredMembers]
    public InviteActivity(TypeMap typeMap, InviteActivityEntity? entity) : base(typeMap, null)
        => Entity = entity ?? typeMap.AsEntity<InviteActivityEntity>();

    static InviteActivity IASModel<InviteActivity>.FromGraph(TypeMap typeMap) => new(typeMap, null);

    private InviteActivityEntity Entity { get; }
}

/// <inheritdoc cref="InviteActivity" />
public sealed class InviteActivityEntity : ASEntity<InviteActivity, InviteActivityEntity> {}