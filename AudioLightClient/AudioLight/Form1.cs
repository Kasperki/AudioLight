using CommandLine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace AudioLight
{
    public partial class Form1 : Form
    {
        private const string TITLE = "AudioLight - K³";

        //Sound
        private SoundCapture soundCapture;

        //Connection
        private delegate void SendColorData(string json);
        private SendColorData sendColorData;
        private delegate void CloseConnection();
        private CloseConnection closeConnection;

        private ConnectionOptions connectionOptions;
        class ConnectionOptions
        {
            [Option('i', "ip", Required = false,
              HelpText = "Websocket IP address")]
            public string IP { get; set; }

            [Option('s', "serial", Required = false,
              HelpText = "SerialPort to open")]
            public string SerialPort { get; set; }
        }

        //Drawing
        private float screenWidth { get { return this.Width; } }
        private float screenHeight { get { return this.Height; } }

        private Brush brushWhite;
        private Pen whitePen;

        private const float PointsLength = 22;
        private Point[] points = new Point[(int)PointsLength];
        private Point[] points2 = new Point[(int)PointsLength];

        //ConnectionMethods
        //SerialPort - PRIMARY
        private SerialPortConnection serialPortConnection;

        //WebSocket - SECONDARY
        private WebSocketConnection webSocketConnction;

        public Form1()
        {
            InitializeComponent();
            InitWindowProperties();

            soundCapture = new SoundCapture();
            soundCapture.ColorCalculated += new EventHandler<EventArgs>(Render);

            brushWhite = new SolidBrush(System.Drawing.Color.White);
            whitePen = new Pen(brushWhite, 3);

            //Get options
            connectionOptions = new ConnectionOptions();
            Parser.Default.ParseArgumentsStrict(Environment.GetCommandLineArgs(), connectionOptions);

            //Try connect with serialport
            SetSerialConnection();
        }

        private void SetSerialConnection()
        {
            serialPortConnection = new SerialPortConnection();
            serialPortConnection.OnFailedConnection += new EventHandler<EventArgs>(SerialConnectionFailed);
            serialPortConnection.OnConnectionOpened += new EventHandler<EventArgs>((object sender, EventArgs e) => {
                this.Text = TITLE + " Serial Connection";
                sendColorData = serialPortConnection.Write;
                closeConnection = serialPortConnection.Close;
            });

            serialPortConnection.OpenConnection(connectionOptions.SerialPort);
        }

        public void SerialConnectionFailed(object sender, EventArgs e)
        {
            SetWebSocketConnection();
        }

        private void SetWebSocketConnection()
        {
            webSocketConnction = new WebSocketConnection();
            webSocketConnction.OnFailedConnection += new EventHandler<EventArgs>((object s, EventArgs ev) =>
            {
                this.Text = TITLE + "  - no connection available";
            });
            webSocketConnction.OnConnectionOpened += new EventHandler<EventArgs>((object s, EventArgs ev) =>
            {
                this.Text = TITLE + " Websocket Connection";
            });

            webSocketConnction.OpenConnection(connectionOptions.IP);
        }

        private void InitWindowProperties()
        {
            this.Text = TITLE;
            this.TransparencyKey = System.Drawing.Color.Pink;
            this.BackColor = System.Drawing.Color.Pink;
        }

        private void Render(object sender, EventArgs e)
        {
            this.Invalidate();
            sendColorData?.Invoke(soundCapture.GetColorString());
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
            closeConnection?.Invoke();
        }
    }
}
