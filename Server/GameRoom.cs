using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;

namespace Server
{
    class GameRoom
    {
        // 공유하는 자원이면 무조건 lock을 걸어줘야 한다.
        // 여기서는 _session 이 공유자원이다.
        List<ClientSession> _session = new List<ClientSession>();
        object _lock = new object();

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = chat;
            ArraySegment<byte> segment = packet.Write();

            lock (_lock)
            {
                foreach (ClientSession s in _session)
                    s.Send(segment);
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                _session.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                _session.Remove(session);
            }
        }
    }
}
