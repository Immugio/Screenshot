using System;
using System.Drawing;

namespace Screenshot
{
    public delegate void ImageCaptureEvent(object sender, Uri url, Bitmap image);
}