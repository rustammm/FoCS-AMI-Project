using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using OsmosLibrary;

namespace OsmosMultiplayer4
{
    // public delegate void OnReceive (Packet packet);
    public class Client
    {
        string name;
        private IPAddress serverIP;
        private IPEndPoint server;
        private EndPoint epServer;
        private byte[] dataStream;
        private UdpClient udpClient;
        private List<Packet> received;

        public Client()
        {
            this.serverIP = null;
            this.server = null;
            this.epServer = null;
            this.dataStream = new byte[1024];
            udpClient = new UdpClient();
            received = new List<Packet>();
        }

        public Client(string serverIP, int serverPort, string userName)
        {
            received = new List<Packet>();
            name = userName;
            udpClient = new UdpClient();
            this.dataStream = new byte[Packet.PACKET_SIZE];

            this.serverIP = IPAddress.Parse(serverIP);
            IPEndPoint server = new IPEndPoint(this.serverIP, serverPort);
            this.epServer = (EndPoint)server;

            udpClient.Connect(server);

            sendMessage(DataType.LogIn, null, 0, name);
        }

        public void sendMessage(DataType type, List<Circle> circles, int user, string name)
        {
            Packet sendMessage = new Packet();

            sendMessage.Type = type;
            sendMessage.Circles = circles;
            sendMessage.User = user;
            sendMessage.SenderName = name;

            byte[] data = sendMessage.toBytes();

            udpClient.BeginSend(data, data.Length, new AsyncCallback(this.SendData), null);
            udpClient.BeginReceive(new AsyncCallback(this.ReceiveData), null);
        }

        private void SendData(IAsyncResult ar)
        {
            udpClient.EndSend(ar);
        }

        private void ReceiveData(IAsyncResult ar)
        {
            try {
                dataStream = udpClient.EndReceive(ar, ref server);
                Packet receivedData;

                try
                {
                    receivedData = new Packet(dataStream);
                }
                catch
                {
                    return;
                }

                received.Add(receivedData);

                Console.Write("Debug Client ReceiveData: message = {0}\n", receivedData.Type);
                this.dataStream = new byte[Packet.PACKET_SIZE];

            }
            catch
            {

            }
            udpClient.BeginReceive(new AsyncCallback(this.ReceiveData), null);
        }


        public List <Packet> getAllReceived()
        {
            List<Packet> ret = new List<Packet>(received);
            received = new List<Packet>();
            return ret;
        }


        //public void addMessageReceiveHandler()
        //{
        //    messageReceived += eventHander;
        //}
    }
}

