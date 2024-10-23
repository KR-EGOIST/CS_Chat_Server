using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class PacketHandler
{
    // 어떤 세션에서 되었는지, 어떤 패킷을 받아온건지 인자로 받는다.
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        // packet을 캐스팅 해줘야 한다.
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

        foreach (C_PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
        }
    }
}