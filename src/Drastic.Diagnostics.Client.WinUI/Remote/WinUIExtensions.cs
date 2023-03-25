// <copyright file="WinUIExtensions.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

namespace Drastic.Diagnostics.Client.WinUI.Remote
{
    internal static class WinUIExtensions
    {
        public static async Task<byte[]> ToImage(this UIElement element)
        {
            var bmp = new RenderTargetBitmap();

            // NOTE: Return to the main thread so we can access view properties such as
            //       width and height. Do not ConfigureAwait!
            await bmp.RenderAsync(element);

            // get the view information first
            var width = bmp.PixelWidth;
            var height = bmp.PixelHeight;

            // then potentially move to a different thread
            var pixels = await bmp.GetPixelsAsync().AsTask().ConfigureAwait(false);

            using MemoryStream ms = new MemoryStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms.AsRandomAccessStream()).AsTask().ConfigureAwait(false);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)width, (uint)height, 96, 96, pixels.ToArray());
            await encoder.FlushAsync().AsTask().ConfigureAwait(false);

            return ms.ToArray();
        }
    }
}
