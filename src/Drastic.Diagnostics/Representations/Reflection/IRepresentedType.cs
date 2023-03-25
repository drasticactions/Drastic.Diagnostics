// <copyright file="IRepresentedType.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System;

namespace Drastic.Diagnostics.Representations.Reflection
{
    public interface IRepresentedType
    {
        string? Name { get; }
        Type? ResolvedType { get; }
        IRepresentedType? BaseType { get; }
    }
}