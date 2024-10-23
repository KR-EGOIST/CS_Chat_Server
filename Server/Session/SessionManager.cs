using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class SessionManager
    {
        // 싱글톤
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        int _sessionId = 0;
        Dictionary<int, ClientSession> _sessions = new Dictionary<int, ClientSession>();
        object _lock = new object();

        // Session 생성 및 ID 발급
        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++_sessionId;
                // 요청이 왔을 때 동적으로 생성하는 방법을 사용
                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                _sessions.Add(sessionId, session);

                Console.WriteLine($"Connected: {sessionId}");

                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                ClientSession session = null;
                _sessions.TryGetValue(id, out session);

                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                _sessions.Remove(session.SessionId);
            }
        }
    }
}
