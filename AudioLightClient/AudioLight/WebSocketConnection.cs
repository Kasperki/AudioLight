using System;
using Quobject.SocketIoClientDotNet.Client;

namespace AudioLight
{
    class WebSocketConnection
    {
        Socket socket;

        private const string CONNECTION_URL = "http://192.168.1.57";
        private const int CONNECTION_PORT = 8080;

        public event EventHandler<EventArgs> OnConnectionOpened;
        public event EventHandler<EventArgs> OnFailedConnection;

        public void OpenConnection(string ip)
        {


            socket = IO.Socket(String.IsNullOrEmpty(ip) ? CONNECTION_URL + ":" + CONNECTION_PORT : "http://" + ip);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                socket.Emit("connection", "AudioLightClient");
                OnConnectionOpened.Invoke(this, new EventArgs());
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, () =>
            {
                OnFailedConnection.Invoke(this, new EventArgs());
            });
        }

        public void Write(string s)
        {
            socket.Emit("colorData", s);
        }

        public void Close()
        {
            socket.Disconnect();
        }
    }
}
