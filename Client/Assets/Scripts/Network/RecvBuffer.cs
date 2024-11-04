using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class RecvBuffer
    {
        ArraySegment<byte> _buffer;
        int _readPos; // 버퍼에서 현재 읽고 있는 위치
        int _writePos; // 버퍼에서 현재 쓰고 있는 위치

        public RecvBuffer(int bufferSize)
        {
            _buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        // 유효범위, 즉 버퍼에 현재 쌓여 있고 읽을 수 있는 데이터 크기
        public int DataSize { get { return _writePos - _readPos; } }
        // 버퍼에 현재 남아 있는 공간
        public int FreeSize { get { return _buffer.Count - _writePos; } }

        // 어디부터 데이터를 읽으면 되는지를 컨텐츠 단에 전달하는 역할
        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _readPos, DataSize); }
        }

        // 다음에 Receive를 할 때 어디서부터 어디까지 쓸 수 있는지에 대한 유효 범위
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer.Array, _buffer.Offset + _writePos, FreeSize); }
        }

        public void Clean()
        {
            int dataSize = DataSize;

            if(dataSize == 0)
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋
                _readPos = _writePos = 0;
            }
            else
            {
                // 남은 조각 데이터가 있으면 시작 위치로 복사
                Array.Copy(_buffer.Array, _buffer.Offset + _readPos, _buffer.Array, _buffer.Offset, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }

        public bool OnRead(int numOfByte)
        {
            // 읽은 데이터가 현재 읽어야 할 데이터보다 클 경우
            if (numOfByte > DataSize)
                return false;

            // 처리한 데이터 만큼 앞으로 이동
            _readPos += numOfByte;
            return true;
        }

        public bool OnWrite(int numOfByte)
        {
            // 쓰는 데이터가 현재 남아있는 공간보다 클 경우
            if (numOfByte > FreeSize)
                return false;

            // 쓴 데이터 만큼 앞으로 이동
            _writePos += numOfByte;
            return true;
        }
    }
}
