using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    private string gameVersion = "0.030f";
    public string userNickName;


    public void OnConnectPhotonServer()
    {
        this.gameObject.SetActive(true);

        // 마스터 클라이언트의 씬 자동 동기화 옵션
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = string.Format($"Player{Random.Range(0, 16)}");
        //PhotonNetwork.NickName = transform.parent.Find("LoginPanel").Find("NickNameInputField").Find("Text").GetComponent<UnityEngine.UI.Text>().text; ;

        // 포톤 서버와 데이터 전송률 체크
        Debug.Log(PhotonNetwork.SendRate);

        // 포톤 서버 접속
        PhotonNetwork.ConnectUsingSettings();

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터에 접속함.");
        Debug.Log($"PhotonNetwork.InLoby = {PhotonNetwork.InLobby}");
        // 로비에 접속
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log($"PhotonNetwork.InLoby = {PhotonNetwork.InLobby}");

        // 준비 버튼을 누르면 Lobby에 접속하고
        // 닉네임 선택을 하면 랜덤한 방을 찾아서 접속하는 것으로 코드 변경할 필요가 있음.


        // 랜덤한 방을 찾아서 접속 시도
        //PhotonNetwork.JoinRandomRoom();
    }

    // 랜덤한 방에 접속을 실패했을 경우 호출되는 콜백함수
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"방 접속 실패  {returnCode}:{message}");

        // 새로운 방 생성
        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = 3; // 최대 플레이어 수
        ro.IsOpen = true; // 공개 비공개 여부
        ro.IsVisible = true; // 로비에서 룸 목록을 노출할 것인지.

        PhotonNetwork.CreateRoom("Room Name", ro);
    }


    public override void OnCreatedRoom()
    {
        Debug.Log("방 생성 끝");
        Debug.Log($"방 이름 {PhotonNetwork.CurrentRoom.Name}");

    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 접속 완료");
        Debug.Log($"방 접속 유무 {PhotonNetwork.InRoom}");
        Debug.Log($"접속 유저 수: {PhotonNetwork.CurrentRoom.PlayerCount}");

        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"접속한 플레이어 이름: {player.Value.NickName}");
        }

        ButtonCtrl _button = transform.GetComponent<ButtonCtrl>();
        _button.PlayerSettingInRoom();

    }



}
