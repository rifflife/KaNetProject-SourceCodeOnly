# KaNetProject-SourceCodeOnly
Unity Engine과 SteamworksAPI를 이용한 4인 Coop P2P 멀티플레이 게임입니다.
Steam에서 제공하는 유저간 ReliableUDP 통신 모듈을 제외한 세션 관리 및 객체 동기화 구조를 직접 작성했습니다.
코드가 아닌 리소스 및 불필요한 일부 코드는 제외했습니다.

# 핵심 코드

## KaNet
### KaNet/Synchronizers
- 프로퍼티 및 RPC 동기화 관련 코드 (Sync~~.cs)
- 동기화 객체 정의 (NetworkObject.cs)
- 객체 생명주기 컨트롤 및 역/직렬화 코드 (NetworkObjectManager.cs)

### KaNet/Session/
- Session 접속 관리

### KaNet/Session/Steam
- Steam 연동 관련 코드

## Scripts/NetworkObject
- 동기화 객체 구문

## Utils
- 네트워크 패킷 관련 코드
- 역/직렬화 코드

## Tests
- 단위 테스트 코드
