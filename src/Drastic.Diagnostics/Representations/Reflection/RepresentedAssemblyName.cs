// <copyright file="RepresentedAssemblyName.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Diagnostics.Serialization;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Drastic.Diagnostics.Representations.Reflection
{
    [Serializable]
    sealed class RepresentedAssemblyName : ISerializable
    {
        readonly AssemblyName assemblyName;

        public string? Name => assemblyName.Name;
        public string FullName => assemblyName.FullName;
        public Version? Version => assemblyName.Version;

        public RepresentedAssemblyName (AssemblyName assemblyName)
            => this.assemblyName = assemblyName
                ?? throw new ArgumentNullException (nameof (assemblyName));

        internal RepresentedAssemblyName (SerializationInfo info, StreamingContext context)
            => assemblyName = new AssemblyName (info.GetValue<string> ("AssemblyName") ?? string.Empty);

        void ISerializable.GetObjectData (SerializationInfo info, StreamingContext context)
            => info.AddValue ("AssemblyName", assemblyName.FullName);

        public bool Equals (RepresentedAssemblyName other)
            => ReferenceEquals (other, this) || FullName.Equals (other.FullName);

        public override bool Equals (object? obj)
            => obj is RepresentedAssemblyName other && Equals (other);

        public override int GetHashCode ()
            => FullName.GetHashCode ();

        public override string ToString ()
            => FullName;

        public static implicit operator AssemblyName (RepresentedAssemblyName representedAssemblyName)
            => representedAssemblyName.assemblyName;
    }
}