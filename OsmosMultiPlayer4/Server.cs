using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using OsmosLibrary;

namespace OsmosMultiplayer4
{

    public class Server
    {
        public static List<Packet> received = new List<Packet>();

        static public int PORT = 30000;

        private struct Client
        {
            public EndPoint endPoint;
            public string name;
        }

        public List<Circle> circles;
        private List<Client> clientList;
        private Socket serverSocket;
        private byte[] dataStream = new byte[Packet.PACKET_SIZE];


        public void start()
        {
            try
            {

                this.clientList = new List<Client>();
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint server = new IPEndPoint(IPAddress.Any, PORT);
                serverSocket.Bind(server);
                IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
                EndPoint epSender = clients;
                serverSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None,
                    ref epSender, new AsyncCallback(ReceiveData), epSender);


            }
            catch (Exception e)
            {
                throw e;
            }
        }


        void ReceiveData(IAsyncResult asyncResult)
        {
            IPEndPoint clients = new IPEndPoint(IPAddress.Any, 0);
            EndPoint epSender = (EndPoint)clients;
            try {
                serverSocket.EndReceiveFrom(asyncResult, ref epSender);


                Packet received = new Packet(this.dataStream);
                received.IP = epSender;

                Server.received.Add(received);

            }
            catch
            {

            }
            serverSocket.BeginReceiveFrom(this.dataStream, 0, this.dataStream.Length, SocketFlags.None,
                        ref epSender, new AsyncCallback(ReceiveData), epSender);
        }

        public void sendPacket(Packet sendData)
        {
            byte[] data = sendData.toBytes();

            foreach (Client client in this.clientList)
            {
                serverSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, client.endPoint,
                    new AsyncCallback(this.SendData), client.endPoint);
            }
        }

        public void sendPacketOne(Packet sendData, EndPoint ep)
        {
            byte[] data = sendData.toBytes();

            serverSocket.BeginSendTo(data, 0, data.Length, SocketFlags.None, ep,
                    new AsyncCallback(this.SendData), ep);
            
        }


        public void SendData(IAsyncResult asyncResult)
        {
            serverSocket.EndSend(asyncResult);

        }

        public List<Packet> getAllReceived()
        {
            List<Packet> ret = new List<Packet>(received);
            received = new List<Packet>();
            return ret;
        }
    };
}
