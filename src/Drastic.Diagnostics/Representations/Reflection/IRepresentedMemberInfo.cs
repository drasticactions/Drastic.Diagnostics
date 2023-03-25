// <copyright file="IRepresentedMemberInfo.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

namespace Drastic.Diagnostics.Representations.Reflection
{
    public interface IRepresentedMemberInfo
    {
        IRepresentedType? DeclaringType { get; }
        RepresentedMemberKind? MemberKind { get; }
        IRepresentedType? MemberType { get; }
        string? Name { get; }
        bool CanWrite { get; }
    }
}