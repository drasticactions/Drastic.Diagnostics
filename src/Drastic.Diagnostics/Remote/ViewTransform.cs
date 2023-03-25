// <copyright file="ViewTransform.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using System.Numerics;
using Drastic.Tempest;

namespace Drastic.Diagnostics.Remote
{
    public class ViewTransform : ISerializable
    {
        public double M11 = 1;
        public double M12;
        public double M13;
        public double M14;
        public double M21;
        public double M22 = 1;
        public double M23;
        public double M24;
        public double M31;
        public double M32;
        public double M33 = 1;
        public double M34;
        public double OffsetX;
        public double OffsetY;
        public double OffsetZ;
        public double M44 = 1;

        public void Deserialize(ISerializationContext context, IValueReader reader)
        {
            this.M11 = reader.ReadDouble();
            this.M12 = reader.ReadDouble();
            this.M13 = reader.ReadDouble();
            this.M14 = reader.ReadDouble();
            this.M21 = reader.ReadDouble();
            this.M22 = reader.ReadDouble();
            this.M23 = reader.ReadDouble();
            this.M24 = reader.ReadDouble();
            this.M31 = reader.ReadDouble();
            this.M32 = reader.ReadDouble();
            this.M33 = reader.ReadDouble();
            this.M34 = reader.ReadDouble();
            this.OffsetX = reader.ReadDouble();
            this.OffsetY = reader.ReadDouble();
            this.OffsetZ = reader.ReadDouble();
            this.M44 = reader.ReadDouble();
        }

        public void Serialize(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteDouble(this.M11);
            writer.WriteDouble(this.M12);
            writer.WriteDouble(this.M13);
            writer.WriteDouble(this.M14);
            writer.WriteDouble(this.M22);
            writer.WriteDouble(this.M23);
            writer.WriteDouble(this.M24);
            writer.WriteDouble(this.M33);
            writer.WriteDouble(this.M34);
            writer.WriteDouble(this.OffsetX);
            writer.WriteDouble(this.OffsetY);
            writer.WriteDouble(this.OffsetZ);
            writer.WriteDouble(this.M44);
        }
    }

    public static class ViewTransformExtensions
    {
        public static ViewTransform ToViewTransform(this Matrix4x4 transform)
        {
            return new ViewTransform()
            {
                M11 = transform.M11,
                M12 = transform.M12,
                M13 = transform.M13,
                M14 = transform.M14,
                M21 = transform.M21,
                M22 = transform.M22,
                M23 = transform.M23,
                M24 = transform.M24,
                M31 = transform.M31,
                M32 = transform.M32,
                M33 = transform.M33,
                M34 = transform.M34,
                OffsetX = transform.Translation.X,
                OffsetY = transform.Translation.Y,
                OffsetZ = transform.Translation.Z,
                M44 = transform.M44,
            };
        }
    }
}
