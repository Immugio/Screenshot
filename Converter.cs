using System;
using System.Drawing;
using System.Windows.Forms;

namespace Screenshot
{
    public class Converter
    {
        private readonly WebBrowser _browser;       
        private Rectangle _screen;             
        
        public delegate void HtmlCaptureEvent(object sender, Uri url, Bitmap image);
        public event HtmlCaptureEvent Callback;
        
        public Converter()
        {
            _browser = new WebBrowser
            {
                Width = _screen.Width,
                Height = _screen.Height,
                ScriptErrorsSuppressed = true,
                ScrollBarsEnabled = false
            };

            _screen = Screen.PrimaryScreen.Bounds;

            bool invoked = false;
            _browser.DocumentCompleted += delegate
            {
                if (invoked) return;
                invoked = true;
                
                _browser.Document.Window.AttachEventHandler("onload", delegate
                {
                    System.Threading.SynchronizationContext.Current.Post(delegate { Render(); }, null);
                });
            };
        }
        
        public void Create(string url)
        {            
            _browser.Navigate(url);
        }                          

        private void Render()
        {                   
            if (_browser == null || _browser.Document == null) return;

            Rectangle body = _browser.Document.Body.ScrollRectangle;
            
            Rectangle docRectangle = new Rectangle
            {
                Location = new Point(0, 0),
                Size = new Size(body.Width > _screen.Width ? body.Width : _screen.Width, body.Height > _screen.Height ? body.Height : _screen.Height)
            };
            
            _browser.Width = docRectangle.Width;
            _browser.Height = docRectangle.Height;

            var bitmap = new Bitmap(docRectangle.Width, docRectangle.Height);
            
            var ivo = _browser.Document.DomDocument as IViewObject;

            using (var g = Graphics.FromImage(bitmap))
            {                
                IntPtr hdc = g.GetHdc();
                ivo.Draw(1, -1, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, hdc, ref docRectangle, ref docRectangle, IntPtr.Zero, 0);
                g.ReleaseHdc(hdc);
            }
            
            if (Callback != null) Callback(this, _browser.Url, bitmap);
        }        
    }
}
