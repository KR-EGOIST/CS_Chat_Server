using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    // 어떤 세션에서 되었는지, 어떤 패킷을 받아온건지 인자로 받는다.
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        //if(chatPacket.playerId == 1)
            Console.WriteLine(chatPacket.chat);
    }
}