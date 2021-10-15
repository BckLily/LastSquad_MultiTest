using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonCtrl : MonoBehaviour
{
    public void OnGameReadyButtonClick()
    {
        // 멀티 용 코드
        SceneManager.LoadScene("LobbyScene");

        // 솔로용 코드
        //    GameObject _lobby = transform.Find("LobbyPanel").gameObject;
        ////    _lobby.SetActive(true);
        //GameObject _mainButton = transform.Find("ButtonPanel").gameObject;
        //_mainButton.SetActive(false);

        //// 플레이어 닉네임 입력하는 창
        //// 나중에는 아이디 비밀번호를 입력하고
        //// 닉네임을 받아와서 닉네임을 자동 입력하는 방식으로 처리.
        //GameObject _login = transform.Find("LoginPanel").gameObject;
        //_login.SetActive(true);

    }


    public void OnGameStartButtonClick()
    {
        PlayerPrefs.SetString("Player_NickName", transform.Find("LobbyPanel").Find("PlayerListPanel").Find("PlayerInfoPanel_0").Find("PlayerNameText").GetComponent<UnityEngine.UI.Text>().text);
        PlayerPrefs.SetString("Player_Class", transform.Find("LobbyPanel").Find("PlayerListPanel").Find("PlayerInfoPanel_0").Find("ClassDropdown").Find("Label").GetComponent<UnityEngine.UI.Text>().text);

        // 로딩
        // 멀티용
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            Photon.Pun.PhotonNetwork.LoadLevel("MapScene");
        }
        // 싱글용
        //GameManager.instance.SceneLoadingFunction("MapScene");

        // 로딩 씬 아직 없음.
        //StartCoroutine(GameSceneLoad());


    }



    IEnumerator GameSceneLoad()
    {
        AsyncOperation _operation = SceneManager.LoadSceneAsync("MapScene");
        _operation.allowSceneActivation = false;

        Debug.Log("___ Loading... ___");
        while (_operation.progress < 0.9f)
        {
            yield return null;
        }
        Debug.Log("____ Loading almost complete ____ ");


        //Game Manager Game Start
        // 자신이 몇 번째로 접속했는지 확인할 수 있나?
        // 가능하면 몇 번째 패널에 자신의 데이터가 있는지 확인할 수 있다.

        GameManager.instance.GameStart(_operation,
            transform.Find("LobbyPanel").Find("PlayerListPanel").Find("PlayerInfoPanel_0").Find("PlayerNameText").GetComponent<UnityEngine.UI.Text>().text,
            transform.Find("LobbyPanel").Find("PlayerListPanel").Find("PlayerInfoPanel_0").Find("ClassDropdown").Find("Label").GetComponent<UnityEngine.UI.Text>().text);
        _operation.allowSceneActivation = true;

    }


    public void OnGameExitButtonClick()
    {
        GameManager.instance.ExitGame();
    }


    public void OnLobbyExitButtonClick()
    {
        // 멀티 용 코드
        // 지금의 로비는 방이다.
        Photon.Pun.PhotonNetwork.LeaveRoom();

        //Photon.Pun.PhotonNetwork.LeaveLobby();
        //SceneManager.LoadScene("MainMenuScene");



        // 솔로 용 코드
        //GameObject _lobby = transform.Find("LobbyPanel").gameObject;
        //_lobby.SetActive(false);

        //GameObject _mainButton = transform.Find("ButtonPanel").gameObject;
        //_mainButton.SetActive(true);
    }


    public void OnLogInSelectButtonClick()
    {
        string _playerNickName = transform.Find("LoginPanel").Find("NickNameInputField").Find("Text").GetComponent<UnityEngine.UI.Text>().text;
        //string _playerNickName = Photon.Pun.PhotonNetwork.NickName;

        if (_playerNickName == "" || _playerNickName == null || _playerNickName.Substring(0, 1) == " ")
        {
            transform.Find("LoginPanel").Find("NickNameErrorText").gameObject.SetActive(true);
            return;
        }

        // 닉네임 입력 에러가 발생했었을 경우 비활성화.
        if (transform.Find("LoginPanel").Find("NickNameErrorText").gameObject.activeSelf)
        {
            transform.Find("LoginPanel").Find("NickNameErrorText").gameObject.SetActive(false);
        }


        GameObject _login = transform.Find("LoginPanel").gameObject;
        _login.SetActive(false);

        GameObject _lobby = transform.Find("LobbyPanel").gameObject;
        _lobby.SetActive(true);

        int count = Photon.Pun.PhotonNetwork.CurrentRoom.PlayerCount;

        // 플레이어가 여러명이면 PlayerInfoPanel의 번호가 달라진다.
        Transform _panel = transform.Find("LobbyPanel").Find("PlayerListPanel").Find($"PlayerInfoPanel_{count - 1}");
        _panel.Find("PlayerNameText").gameObject.SetActive(true);
        _panel.Find("ClassDropdown").gameObject.SetActive(true);
        _panel.Find("PlayerNameText").GetComponent<UnityEngine.UI.Text>().text = _playerNickName;


    }

    public void OnLogInExitButtonClick()
    {
        SceneManager.LoadScene("MainMenuScene");

        //GameObject _login = transform.Find("LoginPanel").gameObject;
        //_login.SetActive(false);

        //GameObject _mainButton = transform.Find("ButtonPanel").gameObject;
        //_mainButton.SetActive(true);
    }



    public void OnContinueGameButtonClick()
    {
        PlayerCtrl playerCtrl = transform.parent.parent.GetComponent<PlayerCtrl>();
        playerCtrl.MenuOpen();
    }


    public void OnMainMenuButtonClick()
    {
        StartCoroutine(MainMenuSceneLoad());
    }

    IEnumerator MainMenuSceneLoad()
    {
        AsyncOperation _operation = SceneManager.LoadSceneAsync("MainMenuScene");
        _operation.allowSceneActivation = false;

        Debug.Log("____ Loading... ____");
        while (_operation.progress < 0.9f)
        {
            yield return null;
        }
        Debug.Log("____ Loading almost Complete ____");

        GameManager.instance.GameFail();
        yield return null;
        _operation.allowSceneActivation = true;

    }


    public void OnKeyInformationOpenButtonClick()
    {
        transform.Find("KeyInfoPanel").gameObject.SetActive(true);
    }

    public void OnKeyInformationExitButtonClick()
    {
        transform.Find("KeyInfoPanel").gameObject.SetActive(false);
    }


}
