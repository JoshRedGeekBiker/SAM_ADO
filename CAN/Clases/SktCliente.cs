using System.Net;
using System.Net.Sockets;

   public class SktCliente
    {

        private IPEndPoint Dir;

        public SktCliente(Socket SKT, int Puerto_Socket)
        {
        SKT = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dir = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Puerto_Socket);


        }

    }

