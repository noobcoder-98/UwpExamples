using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bitmap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var originalBitmap = await ConvertImageSourceToWriteableBitmapAsync(OriginalImage);

            var transformed = await TransformAsync(originalBitmap, 500, 500, 45);

            ModifiedImage.Source = transformed;
        }

        public async Task<WriteableBitmap> ConvertImageSourceToWriteableBitmapAsync(Image image)
        {
            if (image.Source is BitmapImage bitmapImage && bitmapImage.UriSource != null)
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(bitmapImage.UriSource);
                using (var stream = await file.OpenReadAsync())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    var writeableBitmap = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    await writeableBitmap.SetSourceAsync(stream);
                    return writeableBitmap;
                }
            }

            throw new InvalidOperationException("Image.Source is not a BitmapImage with URI.");
        }

        public static async Task<WriteableBitmap> TransformAsync(WriteableBitmap source, int targetWidth, int targetHeight, float angleDegree)
        {
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasBitmap canvasBitmap;
            using (var stream = new InMemoryRandomAccessStream())
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                var pixels = source.PixelBuffer.ToArray();

                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Ignore,
                    (uint)source.PixelWidth,
                    (uint)source.PixelHeight,
                    96, 96,
                    pixels);

                await encoder.FlushAsync();
                stream.Seek(0);
                canvasBitmap = await CanvasBitmap.LoadAsync(device, stream);
            }

            using (var renderTarget = new CanvasRenderTarget(device, targetWidth, targetHeight, 96))
            {
                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.Clear(Windows.UI.Colors.Transparent);
                    var center = new Vector2(targetWidth / 2f, targetHeight / 2f);
                    var scaleX = targetWidth / (float)canvasBitmap.Size.Width;
                    var scaleY = targetHeight / (float)canvasBitmap.Size.Height;
                    var angleRad = (float)(angleDegree * Math.PI / 180);
                    ds.Transform = Matrix3x2.CreateTranslation(-canvasBitmap.Size.ToVector2() / 2)
                                 * Matrix3x2.CreateRotation(angleRad)                             
                                 * Matrix3x2.CreateScale(scaleX, scaleY)                          
                                 * Matrix3x2.CreateTranslation(center);                           

                    ds.DrawImage(canvasBitmap);
                }

                var output = new WriteableBitmap(targetWidth, targetHeight);
                using (var stream = new InMemoryRandomAccessStream())
                {
                    await renderTarget.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                    stream.Seek(0);
                    await output.SetSourceAsync(stream);
                }

                return output;
            }
        }
    }
}
