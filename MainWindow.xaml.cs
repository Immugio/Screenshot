using System;
using System.Drawing;

namespace Screenshot
{
   public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Converter converter = new Converter();
            converter.Callback += SaveImage;
            converter.Create("http://stackoverflow.com/questions/20064117/webbrowserreadystate-is-always-shows-interactive-in-documentcompleted-event");       
        }

        void SaveImage(object sender, Uri url, Bitmap image)
        {
            image.Save("C:/" + url.Authority + ".bmp");
        }
    }
}
