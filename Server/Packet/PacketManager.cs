using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PacketManager
    {
        // 싱글톤을 사용하면 PacketManager가 전체 코드에서 하나만 있는 것처럼 사용할 수 있다.
        // 사용법 : PacketManager.Instance
        #region Singleton
        // PacketHandler를 다 등록 시킨 다음에는 수정할 것이 없기 때문에 싱글톤으로 만들어준다.
        static PacketManager _Instance;
        public static PacketManager Instance
        {
            get
            {
                // _Instance가 없다면 처음 한 번만 new PacketManager()로 만들어 주고
                // 그게 아니면 그냥 _Instance를 반환해준다.
                if (_Instance == null)
                    _Instance = new PacketManager();
                return _Instance;
            }
        }
        #endregion

        // key : 패킷 ID , value : 패킷 생성 함수
        Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> ();
        // key : 패킷 ID , value : 패킷 출력 함수
        Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

        // 모든 패킷의 동작을 등록한다.
        public void Register()
        {
            _onRecv.Add((ushort)PacketID.PlayerInfoReq, MakePacket<PlayerInfoReq>);
            _handler.Add((ushort)PacketID.PlayerInfoReq, PacketHandler.PlayerInfoReqHandler);
        }

        public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
        {
            ushort count = 0;

            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            // _onRecv의 key값을 id로 찾아서 어떤 행동을 해야 될지를 추출한다.
            Action<PacketSession, ArraySegment<byte>> action = null;
            if (_onRecv.TryGetValue(id, out action))
                action.Invoke(session, buffer); // MakePacket<T> 호출
        }

        // 제너릭 T 의 where 조건 : 반드시 IPacket을 인터페이스로 구현하는 클래스이여야 한다.
        // 제너릭 T 의 where 조건 : new()가 가능해야 된다.
        void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
        {
            T pkt = new T();
            pkt.Read(buffer);

            Action<PacketSession, IPacket> action = null;
            // _handler의 key값을 p.Protocol로 찾아서 어떤 행동을 해야 될지를 추출한다.
            if (_handler.TryGetValue(pkt.Protocol, out action))
                action.Invoke(session, pkt);    // PacketHandler의 핸들러 호출
        }
    }
}
