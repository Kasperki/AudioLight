using System;
using System.IO.Ports;

namespace AudioLight
{
    class SerialPortConnection
    {
        SerialPort serialPort;

        private const string PORT_NAME = "COM3";
        private const int BAUD_RATE = 57600;

        public event EventHandler<EventArgs> OnConnectionOpened;
        public event EventHandler<EventArgs> OnFailedConnection;

        public SerialPortConnection()
        {
            serialPort = new SerialPort(PORT_NAME, BAUD_RATE);

            serialPort.ReadTimeout = 500;
            serialPort.WriteTimeout = 500;
        }

        public void OpenConnection(string port)
        {
            try
            {
                serialPort.PortName = String.IsNullOrEmpty(port) ? PORT_NAME : port;
                serialPort.Open();
                OnConnectionOpened?.Invoke(this, new EventArgs());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                OnFailedConnection?.Invoke(this, new EventArgs());
            }
        }

        public void Write(string s)
        {
            if (serialPort.IsOpen)
            {
                serialPort.WriteLine(s);
            }
        }

        public void Close()
        {
            serialPort.Close();
        }
    }
}
    

