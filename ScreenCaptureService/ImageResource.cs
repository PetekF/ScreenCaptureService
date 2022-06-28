using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenCaptureService
{
    public class ImageResource
    {
        public static Bitmap? lastScreenshot;
        public static object lastScreenshotLock = new object();
    }
}
