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
        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");
            // JobQueue에 Enter 행동을 넣는다.
            // 클라이언트 접속시 강제로 채팅방에 입장
            Program.Room.Push(() => Program.Room.Enter(this));
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
                // Room은 ClientSession의 Room인데 Room을 null로 밀어주고 람다로 Room.Leave 콜백을 하므로
                // Room이 null인데 어떻게 Leave를 하니? 하면서 에러가 발생
                // 그러므로 ClientSession은 없어지지만 GameRoom은 그대로 있으므로 GameRoom을 참조한다.
                GameRoom room = Room;
                // JobQueue에 Leave 행동을 넣는다.
                room.Push(() => room.Leave(this));
                // 혹시 2번 호출하는 상황을 방지하기 위해 null로 밀어줌
                Room = null;
            }

            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
