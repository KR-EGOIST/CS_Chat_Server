using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Test
    {
        public int testInt;

        public void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.testInt = BitConverter.ToInt32(s.Slice(count, s.Length - count));
            count += sizeof(int);
        }

        public ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;
            bool success = true;

            Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), (ushort)PacketID.Test);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(s.Slice(count, s.Length - count), this.testInt);
            count += sizeof(int);
            success &= BitConverter.TryWriteBytes(s, count);
            if (success == false)
                return null;
            return SendBufferHelper.Close(count);
        }
    }

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            try
            {
                //Packet packet = new Packet() { size = 100, packetId = 10 };

                //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);

                //byte[] buffer = BitConverter.GetBytes(packet.size);
                //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);

                //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

                //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

                //Send(sendBuff);
                Thread.Sleep(5000);
                Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq p = new PlayerInfoReq();
                        p.Read(buffer);
                        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");

                        foreach (PlayerInfoReq.Skill skill in p.skills)
                        {
                            Console.WriteLine($"Skill({skill.id})({skill.level})({skill.duration})");
                        }
                    }
                    break;
            }

            Console.WriteLine($"RecvPacketId: {id}, Size: {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes: {numOfBytes}");
        }
    }
}
