// <copyright file="UnifiedExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using CoreAnimation;
using Drastic.Diagnostics.Remote;

namespace Drastic.Diagnostics.Client.iOS
{
    static class CoreAnimationExtensions
    {
        public static CATransform3D Prepend(this CATransform3D a, CATransform3D b) =>
            b.Concat(a);

        public static CATransform3D GetLocalTransform(this CALayer layer)
        {
            return CATransform3D.Identity
                .Translate(
                    layer.Position.X,
                    layer.Position.Y,
                    layer.ZPosition)
                .Prepend(layer.Transform)
                .Translate(
                    -layer.AnchorPoint.X * layer.Bounds.Width,
                    -layer.AnchorPoint.Y * layer.Bounds.Height,
                    -layer.AnchorPointZ);
        }

        public static CATransform3D GetChildTransform(this CALayer layer)
        {
            var childTransform = layer.SublayerTransform;

            if (childTransform.IsIdentity)
                return childTransform;

            return CATransform3D.Identity
                .Translate(
                    layer.AnchorPoint.X * layer.Bounds.Width,
                    layer.AnchorPoint.Y * layer.Bounds.Height,
                    layer.AnchorPointZ)
                .Prepend(childTransform)
                .Translate(
                    -layer.AnchorPoint.X * layer.Bounds.Width,
                    -layer.AnchorPoint.Y * layer.Bounds.Height,
                    -layer.AnchorPointZ);
        }

        public static CATransform3D TransformToAncestor(this CALayer fromLayer, CALayer toLayer)
        {
            var transform = CATransform3D.Identity;

            CALayer? current = fromLayer;
            while (current != toLayer)
            {
                transform = transform.Concat(current.GetLocalTransform());

                current = current.SuperLayer;
                if (current == null)
                    break;

                transform = transform.Concat(current.GetChildTransform());
            }
            return transform;
        }

        public static ViewTransform ToViewTransform(this CATransform3D transform) =>
            new ViewTransform
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
                OffsetX = transform.M41,
                OffsetY = transform.M42,
                OffsetZ = transform.M43,
                M44 = transform.M44,
            };
    }
}
