using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;


namespace TestCEFWinForm
{
    public partial class Form1 : Form
    {
        public ChromiumWebBrowser chromeBrowser;
        const int WM_MOUSEMOVE = 0x0200;
        const int WM_MOUSELEAVE = 0x02A3;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONDBLCLK = 0x0203;
        const int WM_LBUTTONUP = 0x0202;
        private void ChromeBrowser_IsBrowserInitializedChanged(object sender, EventArgs args)
        {
            var browser = ((ChromiumWebBrowser)sender);
            if (browser.IsBrowserInitialized)
            {
                ChromeWidgetMessageInterceptor.SetupLoop(this.chromeBrowser, (message) =>
                {
                    if (message.Msg == WM_LBUTTONDOWN)
                    {
                        Point point = new Point(message.LParam.ToInt32());

                        if (((DragDropHandler)chromeBrowser.DragHandler).draggableRegion.IsVisible(point))
                        {
                            ReleaseCapture();
                            SendHandleMessage();
                        }
                    }
                    else if (message.Msg == WM_LBUTTONDBLCLK)
                    {
                        Point point = new Point(message.LParam.ToInt32());
                        if (((DragDropHandler)chromeBrowser.DragHandler).draggableRegion.IsVisible(point))
                        {
                            this.Invoke(new Action(() =>
                            {
                                Maximize();
                            }));
                            ReleaseCapture();
                            SendHandleMessage();
                        }
                    }
                });
            }
        }
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public void SendHandleMessage()
        {
            if (InvokeRequired) { Invoke(new SendHandleMessageDelegate(SendHandleMessage), new object[] { }); return; }
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }
        public delegate void SendHandleMessageDelegate();

        public void InitBrowser()
        {
            Cef.Initialize(new CefSettings());
            chromeBrowser = new ChromiumWebBrowser("https://localhost:44377/");
            chromeBrowser.IsBrowserInitializedChanged += ChromeBrowser_IsBrowserInitializedChanged;
            chromeBrowser.DragHandler = new DragDropHandler();
            this.Controls.Add(chromeBrowser);            
            chromeBrowser.Dock = DockStyle.Fill;
            chromeBrowser.JavascriptObjectRepository.Register("boundAsync", new AsyncJavascriptBindingClass(this), true);
        }


        



        public Form1()
        {
            //m_aeroEnabled = false;
            this.FormBorderStyle = FormBorderStyle.None;            
            this.DoubleBuffered = true;
            this.Padding = new Padding(1);
            this.BackColor = Color.DarkGray;
            this.ControlBox = false;
            this.Text = String.Empty;
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            InitBrowser();            
        }

        public void Maximize()
        {
            
            if (this.WindowState == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                var size = Screen.FromControl(this).WorkingArea.Size;
                size.Width = size.Width + 15;
                size.Height = size.Height + 15;
                this.MaximumSize = size;
                this.WindowState = FormWindowState.Maximized;
            }           
        }
        public void Minimize()
        {
            this.WindowState = FormWindowState.Minimized;
        }




        const int WS_CLIPCHILDREN = 0x2000000;
        const int WS_MINIMIZEBOX = 0x20000;
        const int WS_MAXIMIZEBOX = 0x10000;
        const int WS_SYSMENU = 0x80000;
        const int CS_DBLCLKS = 0x8;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style = WS_CLIPCHILDREN | WS_MINIMIZEBOX | WS_SYSMENU;
                cp.ClassStyle = CS_DBLCLKS;
                return cp;
            }
        }



        protected override void OnPaint(PaintEventArgs e) // you can safely omit this method if you want
        {
            e.Graphics.FillRectangle(Brushes.DarkGray, Top);
            e.Graphics.FillRectangle(Brushes.DarkGray, Left);
            e.Graphics.FillRectangle(Brushes.DarkGray, Right);
            e.Graphics.FillRectangle(Brushes.DarkGray, Bottom);
        }

        private const int
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17;

        const int _ = 10; // you can rename this variable if you like

        Rectangle Top { get { return new Rectangle(0, 0, this.ClientSize.Width, _); } }
        Rectangle Left { get { return new Rectangle(0, 0, _, this.ClientSize.Height); } }
        Rectangle Bottom { get { return new Rectangle(0, this.ClientSize.Height - _, this.ClientSize.Width, _); } }
        Rectangle Right { get { return new Rectangle(this.ClientSize.Width - _, 0, _, this.ClientSize.Height); } }

        Rectangle TopLeft { get { return new Rectangle(0, 0, _, _); } }
        Rectangle TopRight { get { return new Rectangle(this.ClientSize.Width - _, 0, _, _); } }
        Rectangle BottomLeft { get { return new Rectangle(0, this.ClientSize.Height - _, _, _); } }
        Rectangle BottomRight { get { return new Rectangle(this.ClientSize.Width - _, this.ClientSize.Height - _, _, _); } }


        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);

            if (message.Msg == 0x84) // WM_NCHITTEST
            {
                var cursor = this.PointToClient(Cursor.Position);

                if (TopLeft.Contains(cursor)) message.Result = (IntPtr)HTTOPLEFT;
                else if (TopRight.Contains(cursor)) message.Result = (IntPtr)HTTOPRIGHT;
                else if (BottomLeft.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMLEFT;
                else if (BottomRight.Contains(cursor)) message.Result = (IntPtr)HTBOTTOMRIGHT;

                else if (Top.Contains(cursor)) message.Result = (IntPtr)HTTOP;
                else if (Left.Contains(cursor)) message.Result = (IntPtr)HTLEFT;
                else if (Right.Contains(cursor)) message.Result = (IntPtr)HTRIGHT;
                else if (Bottom.Contains(cursor)) message.Result = (IntPtr)HTBOTTOM;
            }
        }


        




    }
}
