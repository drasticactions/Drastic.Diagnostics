// <copyright file="RepresentedMemberKind.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.Diagnostics.Representations.Reflection
{
    [Serializable]
    public enum RepresentedMemberKind : byte
    {
        None,
        Field,
        Property
    }
}