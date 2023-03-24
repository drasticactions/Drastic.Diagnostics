// <copyright file="StringArraySerializer.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Drastic.Tempest;

namespace Drastic.Diagnostics.Transport
{
    public class StringArraySerializer : ISerializer<string>
    {
        /// <inheritdoc/>
        public string Deserialize(ISerializationContext context, IValueReader reader)
        {
            return reader.ReadString();
        }

        /// <inheritdoc/>
        public void Serialize(ISerializationContext context, IValueWriter writer, string element)
        {
           writer.WriteString(element);
        }
    }
}
