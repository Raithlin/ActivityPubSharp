// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace ActivityPub.Types.Internal;

// This is such a hack

internal class ASNameTree
{
    public ASNameTree? Parent { get; }

    public IReadOnlySet<string> DerivedTypeNames => _derivedTypeNames;
    private readonly HashSet<string> _derivedTypeNames = new();

    public ASNameTree(string? asTypeName, ASNameTree? parent)
    {
        Parent = parent;

        if (asTypeName != null)
            Parent?.AddDerivedName(asTypeName);
    }

    private void AddDerivedName(string name)
    {
        _derivedTypeNames.Add(name);
        Parent?.AddDerivedName(name);
    }
}