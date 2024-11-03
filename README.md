# C# 채팅 서버 (TCP 통신)

## 👋 소개

- 멀티쓰레드 환경으로 구현한 TCP 통신 채팅 서버

## 👩‍💻 개발자

<table>
  <tbody>
    <tr>
      <td align="center"><a href="https://github.com/KR-EGOIST"><img src="https://avatars.githubusercontent.com/u/54177070?v=4" width="100px;" alt=""/><br /><sub><b> 개발자 : 윤진호 </b></sub></a><br /></td>
      </tr>
  </tbody>
</table>

## ⚽ 구현 현황

#### 1. 클라이언트와 서버 연동 (Listener)
  - 동시 다발로 Connect 시도 시 순차적으로 Accept되도록 AcceptAsync처리

#### 2. 문자열 Buffer 생성 및 Send, Recv
  - ## Recv
    - 클라이언트에서 Buffer를 Send할 시 서버는 RecevieAsync를 통해 순차적으로 Recv되도록 처리
   
  - ## Send
    - 서버에서 Buffer를 Send할 시 Buffer를 _sendQueue에 Enqueue
    - _sendQueue에 있는 Buffer를 전부 Dequeue하면서 _pendingList에 Add
    - _sendArgs.BufferList에 _pendingList를 할당한 뒤 SendAsync를 통해 Buffer를 한 번에 보냄
    - Buffer를 보낸 뒤 _sendArgs.BufferList와 _pendingList를 초기화

#### 3. 클라이언트와 서버 연결 및 서버와 서버 연결을 위한 Connector
  - ServerCore에 정의된 Connect, Receive, Send, Disconnect 부분을 공용으로 사용
  - 추후 분산 처리 서버끼리 연결하기 위해서 Connector 정의
  - 동시 다발로 클라이언트 접속 시 순차적으로 Connect되도록 ConnectAsync처리

#### 4. ServerCore 라이브러리화, Server와 Client는 ServerCore 참조
  - ServerCore -> Session에 정의된 Connect, Receive, Send, Disconnect 부분을 공용으로 사용 가능

#### 5. RecvBuffer 클래스 정의
  - Receive 동작 시 buffer의 readPos, writePos를 통해 현재 버퍼의 있는 데이터의 크기 또는 남아있는 버퍼의 공간을 알아낼 수 있다.
  - 예를 들어 TCP통신의 혼잡 제어로 인해 100바이트 중 80바이트만 왔을 때 RecvBuffer에 보관을 하고 있다가 나중에 20바이트를 추가로 받은 후 조립하는 과정을 처리

#### 6. SendBuffer 클래스 정의
  - ChunkSize 만큼의 큰 덩어리 byte배열을 만들어 잘라서 사용하는 방법으로 구현
  - ThreadLocal을 통해서 각 쓰레드마다 고유한 SendBuffer를 사용하도록 구현

#### 7. PacketSession 클래스 정의
  - 클라이언트로부터 전달받은 패킷을 Parsing 처리 후 컨텐츠 단으로 buffer 전달
  - 컨텐츠 단에서 전달받은 buffer를 BitConverter를 통해 변환 및 추출

#### 8. 패킷 직렬화/역직렬화 세션 분리
  - 클라이언트와 서버 메인 코드에서 패킷을 관리하지 않고 세션으로 분리해 패킷을 관리

#### 9. 패킷 자동화 구현
  - XML 형식으로 패킷 구조 정의
  - 패킷 자동화 프로그램 구현
  - 패킷 매니저, 핸들러 분리
  - 패킷 매니저 자동화 구현
  - 서버, 클라 패킷 구분

#### 10. 채팅 테스트
  - 세션 매니저 구현
  - 채팅 룸 구현
  - 패킷 구조 변경 (C_Chat, S_Chat)
  - 서버, 클라 실행 시 패킷 등록 Register
  - 더미 클라이언트 수 증가, Connect 횟수 증가
  - 스트레스 테스트

#### 11. JobQueue 또는 Task
  - 패킷의 행동을 하나의 Queue를 통해서 실행되도록 구현
  - 쓰레드의 과잉 공급을 방지하기 위해 구현
