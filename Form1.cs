using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoFisher
{
    public partial class Form1 : Form
    {
        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_Y = 0x59;
        const int VK_K = 0x4B;
        bool hasCasted = false;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        public Form1()
        {
            InitializeComponent();
            Timer tm = new Timer();
            tm.Interval = 100;
            tm.Tick += Ticker;
            tm.Start();
        }

        private void Ticker(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName("GTA5");
            try
            {
                label3.Text = "GTA Process: " + processes[0].Id.ToString();
                label4.Text = "Status: Ready";
            }
            catch
            {
                label3.Text = "Please open GTA V";
                label4.Text = "Status: Waiting for game";
            }

            Point bottomChat = new Point();
            bottomChat.X = 37;
            bottomChat.Y = 301;

            Point bottomWindow = new Point();
            bottomWindow.X = 951;
            bottomWindow.Y = 1046;
            
            var cChat = GetColorAt(bottomChat);
            var cBot = GetColorAt(bottomWindow);

            if (cChat.R == 255 && cChat.G == 255 && cChat.B == 0 && !hasCasted)
            {
                PostMessage(processes[0].MainWindowHandle, WM_KEYDOWN, VK_Y, 0);
                label4.Text = "Status: CASTING";
                hasCasted = true;
            }
            
            if ((cBot.R >= 90 && cBot.G >= 170 && cBot.B >= 90) && (cBot.R <= 130 && cBot.G <= 230 && cBot.B <= 130))
            {
                PostMessage(processes[0].MainWindowHandle, WM_KEYDOWN, VK_K, 0);
                label4.Text = "Status: CATCHING";
                hasCasted = false;
            }
        }

        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        public Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
            return screenPixel.GetPixel(0, 0);
        }
    }
}
