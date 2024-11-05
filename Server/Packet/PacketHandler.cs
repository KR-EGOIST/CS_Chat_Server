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
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        // session을 PacketSession에서 ClientSession로 캐스팅을 해줘야 한다.
        ClientSession clientSession = session as ClientSession;

        // 방에 있는 상태가 아니다.
        if (clientSession.Room == null)
            return;

        // JobQueue에 broadcast 행동을 집어 넣는다.
        // Room에 있는 모든 사람에게 패킷을 보낸다.
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Leave(clientSession)
        );
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        // packet을 IPacket에서 C_Move로 캐스팅을 해줘야 한다.
        C_Move movePacket = packet as C_Move;
        // session을 PacketSession에서 ClientSession로 캐스팅을 해줘야 한다.
        ClientSession clientSession = session as ClientSession;

        // 방에 있는 상태가 아니다.
        if (clientSession.Room == null)
            return;

        //Console.WriteLine($"{movePacket.posX}, {movePacket.posY}, {movePacket.posZ}");

        // Room에 있는 모든 사람에게 패킷을 보낸다.
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Move(clientSession, movePacket)
        );
    }
}