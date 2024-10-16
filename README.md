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
