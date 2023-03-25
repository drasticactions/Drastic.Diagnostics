using Drastic.Diagnostics.Core;
using Drastic.Diagnostics.Inspection;
using Drastic.Tempest;
using System.Numerics;
using System.Reflection.Emit;

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
        List<InspectView>? sublayers;

        public InspectView? Layer { get; set; }
        public List<InspectView>? Sublayers {
            get { return sublayers; }
            set {
                sublayers = null;
                if (value == null)
                    return;
                foreach (InspectView layer in value)
                    AddSublayer(layer);
            }
        }

        public List<InspectView>? Subviews {
            get { return subviews; }
            set {
                subviews = null;
                if (value == null)
                    return;
                foreach (InspectView view in value)
                    AddSubview(view);
            }
        }

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

        public ViewTransform? Transform { get; set; }

        public ViewKind Kind { get; set; }

        public string Description { get; set; } = string.Empty;

        public byte[] CapturedImage { get; set; } = new byte[0];

        /// <summary>
        /// For multi-window frameworks, like Mac and WPF, the InspectView returned given a handle of
        /// IntPtr.Zero is a "fake" root whose Subviews are the windows of the app. The convention is
        /// to set PublicType to null for these.
        /// </summary>
        public bool IsFakeRoot { get { return string.IsNullOrEmpty(PublicType); } }

        public InspectView Root {
            get {
                int depth;
                return CalculateRootAndDepth(out depth);
            }
        }

        public int Depth {
            get {
                int depth;
                CalculateRootAndDepth(out depth);
                return depth;
            }
        }

        public IEnumerable<InspectView> Children {
            get {
                if (Subviews != null)
                    foreach (var subview in Subviews)
                        yield return subview;

                if (Layer != null)
                    yield return Layer;

                if (Sublayers != null)
                    foreach (var sublayer in Sublayers)
                        yield return sublayer;
            }
        }

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

        public InspectView? FindSelfOrChild(Predicate<InspectView> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            foreach (var inspectView in this.TraverseTree(i => i.Children))
            {
                if (predicate(inspectView))
                    return inspectView;
            }
            return null;
        }

        public virtual void AddSublayer(InspectView sublayer)
        {
            if (sublayers == null)
                sublayers = new List<InspectView>();
            sublayer.Parent = this;
            sublayers.Add(sublayer);
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
            writer.WriteString(this.Description);
            writer.WriteInt16((short)this.Kind);
            writer.Write(context, this.Transform);
            writer.WriteBytes(this.CapturedImage);
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
            this.Description = reader.ReadString();
            this.Kind = (ViewKind)reader.ReadInt16();
            this.Transform = reader.Read<ViewTransform>(context);
            this.CapturedImage = reader.ReadBytes();
        }

        public async Task CaptureAll()
        {
            foreach (var inspectView in this.TraverseTree(i => i.Children))
                await inspectView.UpdateCapturedImage();
        }

        public Task<bool> Capture()
            => this.UpdateCapturedImage();

        protected void PopulateTypeInformationFromObject(object obj)
        {
            this.SetHandle(ObjectCache.Shared.GetHandle(obj));

            var type = obj.GetType();
            var publicType = type.GetPublicType();
            this.Type = type.FullName ?? string.Empty;
            this.PublicType = publicType?.FullName ?? string.Empty;
            this.PublicCSharpType = publicType?.GetCSharpTypeName() ?? string.Empty;
        }

        protected virtual Task<bool> UpdateCapturedImage()
        {
            return Task.FromResult(false);
        }

        private InspectView CalculateRootAndDepth(out int depth)
        {
            depth = 0;
            var view = this;
            while (true)
            {
                if (view.Parent == null)
                    return view;
                view = view.Parent;
                depth++;
            }
        }
    }
}
