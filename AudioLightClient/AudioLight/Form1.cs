using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace AudioLight
{
    public partial class Form1 : Form
    {
        //Sound
        private SoundCapture soundCapture;

        //Connection
        private Socket socket;
        private const string CONNECTION_URL = "http://192.168.1.57";
        private const int CONNECTION_PORT = 8080;

        //Drawing
        private float screenWidth { get { return this.Width; } }
        private float screenHeight { get { return this.Height; } }

        private Brush brushWhite;
        private Pen whitePen;

        private const float PointsLength = 22;
        private Point[] points = new Point[(int)PointsLength];
        private Point[] points2 = new Point[(int)PointsLength];

        public Form1()
        {
            InitializeComponent();
            InitWindowProperties();

            soundCapture = new SoundCapture();
            soundCapture.ColorCalculated += new EventHandler<EventArgs>(Render);

            brushWhite = new SolidBrush(System.Drawing.Color.White);
            whitePen = new Pen(brushWhite, 3);

            socket = IO.Socket(CONNECTION_URL + ":" + CONNECTION_PORT);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("connection", "AudioLightClient");

            });
        }

        private void InitWindowProperties()
        {
            this.Name = "Form";
            this.Text = "AudioLight - K³";
            this.TransparencyKey = System.Drawing.Color.Pink;
            this.BackColor = System.Drawing.Color.Pink;
        }

        private void Render(object sender, EventArgs e)
        {
            this.Invalidate();
            socket.Emit("colorData", soundCapture.GetColorString());
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = this.CreateGraphics();

            Brush coloredBrush = new SolidBrush(soundCapture.color.GetSystemColor());
            Pen coloredPen = new Pen(coloredBrush, 3);

            for (int i = 0; i < PointsLength; i++)
            {
                int x = (int)((i / PointsLength) * screenWidth);
                int power = (int)soundCapture.spectrumHeight[i] * 5;
                int y = (int)screenHeight / 2 - (power > screenHeight / 2 ? (int)screenHeight / 2 : power);

                points[i] = new Point(x, y);
                points2[i] = new Point(x, y - 2);
            }

            graphics.DrawBeziers(whitePen, points);
            graphics.DrawBeziers(coloredPen, points2);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            soundCapture.StopSoundCapture();
        }
    }
}
