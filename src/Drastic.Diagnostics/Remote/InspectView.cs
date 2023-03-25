using Drastic.Diagnostics.Core;
using Drastic.Diagnostics.Inspection;
using Drastic.Tempest;

namespace Drastic.Diagnostics.Remote
{
    public class InspectView : IInspectView, ISerializable
    {
        public unsafe void SetHandle(IntPtr handle)
        {
            this.Handle = (long)(void*)handle;
        }

        public void SetHandle(long handle)
        {
            this.Handle = handle;
        }

        List<InspectView>? subviews;

        public long Handle { get; internal set; }

        public long X { get; set; }

        public long Y { get; set; }

        public long Width { get; set; }

        public long Height { get; set; }

        public InspectView? Parent { get; set; }

        public string Type { get; set; } = string.Empty;

        public string PublicType { get; set; } = string.Empty;
        public string PublicCSharpType { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public virtual void AddSubview(IInspectView subview) => this.AddSubview((InspectView)subview);

        public virtual void AddSubview(InspectView subview)
        {
            if (this.subviews == null)
            {
                this.subviews = new List<InspectView>();
            }

            subview.Parent = this;
            this.subviews.Add(subview);
        }

        public void Serialize(ISerializationContext context, IValueWriter writer)
        {
            writer.WriteInt64(this.Handle);
            writer.WriteInt64(this.X);
            writer.WriteInt64(this.Y);
            writer.WriteInt64(this.Width);
            writer.WriteInt64(this.Height);
            writer.WriteString(this.Type);
            writer.WriteString(this.PublicType);
            writer.WriteString(this.PublicCSharpType);
            writer.WriteString(this.DisplayName);
        }

        public void Deserialize(ISerializationContext context, IValueReader reader)
        {
            this.Handle = reader.ReadInt64();
            this.X = reader.ReadInt64();
            this.Y = reader.ReadInt64();
            this.Width = reader.ReadInt64();
            this.Height = reader.ReadInt64();
            this.Type = reader.ReadString();
            this.PublicType = reader.ReadString();
            this.PublicCSharpType = reader.ReadString();
            this.DisplayName = reader.ReadString();
        }

        protected void PopulateTypeInformationFromObject(object obj)
        {
            this.SetHandle(ObjectCache.Shared.GetHandle(obj));

            var type = obj.GetType();
            var publicType = type.GetPublicType();
            this.Type = type.FullName ?? string.Empty;
            this.PublicType = publicType?.FullName ?? string.Empty;
            this.PublicCSharpType = publicType?.GetCSharpTypeName() ?? string.Empty;
        }
    }
}
