using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using OsmosLibrary;
using System.Net;

namespace OsmosMultiplayer4
{
    public enum DataType
    {
        Update,
        GetState,
        LogIn,
        LogOut,
        Null
    }

    [Serializable]
    public class Packet
    {
        public static int PACKET_SIZE = 10024;
        public string SenderName { get; set; }
        public DataType Type { get; set; }
        public List<Circle> Circles { get; set; }
        public int User;
        public EndPoint IP;

        public Packet()
        {
            SenderName = null;
            Type = DataType.Null;
            Circles = new List<Circle>();
            User = 0;
            IP = null;
        }

        public Packet(byte[] dataStream)
        {
            BinaryFormatter bin = new BinaryFormatter();
            Packet p = (Packet)bin.Deserialize(new MemoryStream(dataStream));
            this.SenderName = p.SenderName;
            this.Type = p.Type;
            this.Circles = p.Circles;
            this.User = p.User;
            IP = null;
        }

        public byte[] toBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bin = new BinaryFormatter();
            bin.Serialize(ms, this);
            return ms.ToArray();
        }

    }
}


