using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        // 공유하는 자원이면 무조건 lock을 걸어줘야 한다.
        // 여기서는 _session 이 공유자원이다.
        List<ClientSession> _session = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            foreach (ClientSession s in _session)
                s.Send(_pendingList);

            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            _session.Add(session);
            session.Room = this;
        }

        public void Leave(ClientSession session)
        {
            _session.Remove(session);
        }
    }
}
