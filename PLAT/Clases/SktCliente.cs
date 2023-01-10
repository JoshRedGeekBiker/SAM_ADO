using System.Net;
using System.Net.Sockets;

   public class SktCliente
    {

    public IPEndPoint Dir;
    public Socket PlatSocket;

    public SktCliente(int Puerto_Socket)
    {
        PlatSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Dir = new IPEndPoint(IPAddress.Parse("127.0.0.1"), Puerto_Socket);
    }

    private void Conectar()
    {
        PlatSocket.Connect(Dir);
    }

    }

