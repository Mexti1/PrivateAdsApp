using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AdsClientWpf.Helpers
{
    public static class ImageHelper
    {
        public static ImageSource? BytesToImageSource(byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            try
            {
                using var ms = new MemoryStream(bytes);
                var img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = ms;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();
                return img;
            }
            catch
            {
                return null;
            }
        }
    }
}
