using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    // 어떤 세션에서 되었는지, 어떤 패킷을 받아온건지 인자로 받는다.
    public static void C_ChatHandler(PacketSession session, IPacket packet)
    {
        // packet을 IPacket에서 C_Chat으로 캐스팅 해줘야 한다.
        C_Chat chatPacket = packet as C_Chat;
        // session도 PacketSession에서 ClientSession로 캐스팅을 해줘야 한다.
        ClientSession clientSession = session as ClientSession;

        // 방에 있는 상태가 아니다.
        if (clientSession.Room == null)
            return;

        // Room에 있는 모든 사람에게 chat내용을 보낸다.
        clientSession.Room.Broadcast(clientSession, chatPacket.chat);
    }
}