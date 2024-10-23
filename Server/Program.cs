using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            PacketManager.Instance.Register();  // 멀티쓰레드 환경이 되기 전에 패킷 등록

            // DNS (Domain Name System)
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");

            while (true)
            {

            }
        }
    }
}
