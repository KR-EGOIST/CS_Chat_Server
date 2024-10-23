using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientSession : PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");
            // 클라이언트 접속시 강제로 채팅방에 입장
            Program.Room.Enter(this);
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            // 싱글톤 호출, this는 ClientSession 이다.
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if(Room != null)
            {
                Room.Leave(this);
                // 혹시 2번 호출하는 상황을 방지하기 위해 null로 밀어줌
                Room = null;
            }

            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
