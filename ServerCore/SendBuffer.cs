using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        // SendBuffer에 대해서 경합이 발생되는 것을 방지
        // ChunkSize만큼 Buffer를 만들어 놓은 다음에 조금씩 쪼개서 사용하겠다는 개념
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        // 외부에서 ChunkSize를 조정하고 싶을 때 사용
        public static int ChunkSize { get; set; } = 65535 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            // 아직 SendBuffer를 한 번도 사용하지 않는 상태
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            // SendBuffer가 고갈 되었을 경우 새로운 SendBuffer를 만들어줘야 한다.
            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize = 0;

        // _buffer의 남은 공간
        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        // 얼마만큼의 사이즈를 최대치로 사용할지를 결정
        public ArraySegment<byte> Open(int reserveSize)
        {
            // 현재 예약공간보다 남은 공간이 작다면 Buffer가 고갈된 상태
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        // 최종적으로 사용한 버퍼를 반환
        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}
