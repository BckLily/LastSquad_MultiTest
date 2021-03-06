using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;
    //public WeaponManager weaponManager;

    public BunkerDoor bunkerDoor;

    // 각 특전이 활성화 되었는지 확인하기 위한 변수
    public bool perk0_Active = false;
    public bool perk1_Active = false;
    public bool perk2_Active = false;

    private int maxStage = 30;
    public int _stage; // 현재 몇 스테이지인지를 저장한 변수
    public int _remainEnemyCount; // 스테이지에 남은 적 수

    private float stageDelay = 60f;
    private float stageTime;

    // 멀티가 된다면 사용하게될? 플레이어들 목록
    // Photon에서는 PhotonNetwork.CurrentRoom.Players 을 사용해서 플레이어 목록을 확인할 수 있다.
    // 특전이 활성화되면 특전 활성화에 사용해야한다.
    public List<PlayerCtrl> players;

    // 적을 생성할 위치
    Transform[] enemyPoints;

    private IEnumerator _coEnemySpawn;

    UnityEngine.UI.Text stageText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        _coEnemySpawn = EnemySpawn();
        _stage = 1;

        /*

        // 지금은 게임 씬에서 시작하니까 바로 GameStart를 실행시킨다.
        GameStart();
        */
    }

    // Update is called once per frame
    void Update()
    {

        // 유니티 에디터에서 동작
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ExitGame();
        }

        // 윈도우에서 동작
#elif UNITY_STANDALONE_WIN
        if (Input.GetKeyDown(KeyCode.F12))
        {
            ExitGame();
        }


#endif
    }

    public void ExitGame()
    {

        // 유니티 에디터에서 동작
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;

        // 윈도우에서 동작
#elif UNITY_STANDALONE_WIN
        Application.Quit();

#endif


    }



    IEnumerator CoFadeIn()
    {
        GameObject gameCanvas = GameObject.Find("GameWorldCanvas");
        UnityEngine.UI.Image fadein = gameCanvas.transform.Find("GameOverPanel").Find("GameOverFadeIn").GetComponent<UnityEngine.UI.Image>();

        while (fadein.color.a < 1f)
        {
            Color _color = fadein.color;
            _color.a += Time.deltaTime;
            fadein.color = _color;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        gameCanvas.GetComponent<ButtonCtrl>().OnMainMenuButtonClick();
        GameFail();
    }


    /// <summary>
    /// 게임 씬으로 넘어갔을 때 실행될 함수
    /// </summary>
    public void GameStart(UnityEngine.AsyncOperation _operation, string _playerNickName, string _className)
    {
        string _class;
        if (_className == "소총병")
        {
            _class = "Soldier";
        }
        else if (_className == "의무병")
        {
            _class = "Medic";
        }
        else if (_className == "공병")
        {
            _class = "Engineer";
        }
        else
        {
            _class = null;
        }

        StartCoroutine(CoGameStart(_operation, _playerNickName, _class));

    }

    // 나중에 플레이어 정보가 DB에 있게되면 DB에서 플레이어 정보를 받아올 수 있을까?
    // 직업 설정은 어떻게 넘겨줄까?
    IEnumerator CoGameStart(UnityEngine.AsyncOperation _operation, string _playerNickName, string _className)
    {
        yield return _operation;

        // 벙커의 위치를 받아온다.
        bunkerDoor = GameObject.FindGameObjectWithTag("BUNKERDOOR").GetComponent<BunkerDoor>();

        // 플레이어 스폰 지점 목록을 가져온다.
        Transform[] points = GameObject.Find("PlayerSpawnPoints").GetComponentsInChildren<Transform>();

        // 부모의 위치도 포함되기 때문에 부모인 0을 제외한 1부터 시작한다.
        int idx = UnityEngine.Random.Range(1, points.Length);

        // 생성할 플레이어 프리팹을 찾음
        GameObject _playerPref = Resources.Load<GameObject>("Prefabs/Player/Player");
        // 생성할 위치를 설정
        players.Add(Instantiate(_playerPref, points[idx].position, Quaternion.identity).GetComponent<PlayerCtrl>());
        // 플레이어의 이름 및 직업 설정
        // Lobyy에서 입력받은 플레이어 이름과 선택된 직업을 사용해서 설정해야한다.
        players[0].playerName = _playerNickName;

        //players[0].playerClass = PlayerClass.ePlayerClass.Medic;
        players[0].playerClass = (PlayerClass.ePlayerClass)System.Enum.Parse(typeof(PlayerClass.ePlayerClass), _className);


        enemyPoints = GameObject.Find("EnemySpawnPoints").GetComponentsInChildren<Transform>();
        Debug.Log("EnemyPoints: " + enemyPoints.Length);

        stageText = GameObject.Find("StageText").GetComponent<UnityEngine.UI.Text>();
        stageText.text = _stage.ToString();

        yield return new WaitForSeconds(1f);

        if (_coEnemySpawn != null)
            StartCoroutine(_coEnemySpawn);
    }


    #region 게임 패배
    // 게임이 종료되면 실행되는 함수.
    public void GameOver()
    {
        // 대충 화면이 어두워지고 Game Over 라거나 패배 라는 글자가 뜨고
        // 잠시후에 로비로 돌아가는 버튼이 활성화되거나 하면 된다.

        GameObject gameCanvas = GameObject.Find("GameWorldCanvas");
        gameCanvas.transform.Find("GameOverPanel").gameObject.SetActive(true);

        StartCoroutine(CoFadeIn());
    }


    internal void GameFail()
    {
        // 테스트 중에는 오류가 발생할 수 있지만
        // 일반적으로 게임 중에는 항상 적은 스폰되고 있을 것이기 때문에 _coEnemeySpawn이 null일 확률은 없다.
        // 테스트 코드
        try
        {
            StopCoroutine(_coEnemySpawn);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }

        perk0_Active = false;
        perk1_Active = false;
        perk2_Active = false;

        players.Clear();
        bunkerDoor = null;
        enemies.Clear();

        CursorState.CursorLockedSetting(false);

    }
    #endregion


    // 테스트 코드
    #region 적 생성 테스트 코드

    public void EnemySpawnDebug()
    {
        if (_coEnemySpawn != null)
        {
            StopCoroutine(_coEnemySpawn);
            _coEnemySpawn = null;
        }
        else
        {
            _coEnemySpawn = EnemySpawn();
            StartCoroutine(_coEnemySpawn);
        }
    }

    public List<LivingEntity> enemies = new List<LivingEntity>();

    IEnumerator EnemySpawn()
    {
        /*
         * 0
         * 1/ 2 3 4 5 6 7
         * 8/ 9 10 11 12 13 14
         * 15/ 16 17 18 19 20 21
         */


        yield return new WaitForSeconds(3f);
        GameObject zombie = Resources.Load<GameObject>("Prefabs/Enemy/Zombie");
        while (true)
        {
            int idx = UnityEngine.Random.Range(2, 8);
            idx += (UnityEngine.Random.Range(0, 3) * 7);
            if (enemies.Count < 15)
            {
                enemies.Add(Instantiate(zombie, enemyPoints[idx].position, Quaternion.identity).GetComponent<LivingEntity>());
            }


            //Debug.Log("____ ENEMY COUNT: " + enemies.Count + " ____");

            yield return new WaitForSeconds(0.5f);
        }

    }
    #endregion


    #region 씬 로드 함수 영역

    // 게임 씬이나 다른 씬으로 넘어갈 때 사용하는 함수
    public void SceneLoadingFunction(string _sceneName)
    {
        StartCoroutine(SceneLoadingCoroutine(_sceneName));
    }


    private IEnumerator SceneLoadingCoroutine(string _sceneName)
    {
        AsyncOperation _loadingOperation = SceneManager.LoadSceneAsync("LoadingScene");

        while (_loadingOperation.progress < 0.9f) { Debug.Log("loading"); yield return null; }
        yield return new WaitForSeconds(0.5f);
        UnityEngine.UI.Slider _slider = GameObject.Find("LoadingSlider").GetComponent<UnityEngine.UI.Slider>();

        AsyncOperation _operation = SceneManager.LoadSceneAsync(_sceneName);
        _operation.allowSceneActivation = false;

        Debug.Log("___ Loading... ___");
        while (_operation.progress < 0.9f)
        {
            _slider.value = (float)_operation.progress;
            Debug.Log("Loading");
            yield return null;
        }
        Debug.Log("____ Loading almost complete ____ ");
        _slider.value = 0.9f;


        //Game Manager Game Start
        GameManager.instance.GameStart(_operation,
            PlayerPrefs.GetString("Player_NickName"),
            PlayerPrefs.GetString("Player_Class"));
        // 이전에 쓰던 방식이 있어서 함수를 유지시켰지만
        // GameStart 함수 내에서 PlayerPrefs를 통해서 받아도 된다.

        int count = 0;
        while (count < 10)
        {
            _slider.value += 1 / 100f;
            count++;
            yield return new WaitForSeconds(0.5f);
        }

        _operation.allowSceneActivation = true;

    }

    #endregion


    #region Photon Function

    public override void OnLeftRoom()
    {
        // 방을 나가면 로비를 나간다.
        Photon.Pun.PhotonNetwork.LeaveLobby();

    }

    public override void OnLeftLobby()
    {
        // 로비를 나가면 메인 메뉴 씬으로 간다.
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    // 플레이어가 접속하면
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
    }

    // 플레이어가 접속하거나 접속이 끊기면 방을 새로 세팅해야 한다.
    private void SetRoomInfo()
    {
#if UNITY_EDITOR
        Debug.Log($"접속 유저 수: {PhotonNetwork.CurrentRoom.PlayerCount}");
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log($"접속한 플레이어 이름: {player.Value.NickName}");
        }
#endif

        GameObject playerListPanel = GameObject.Find("PlayerListPanel");

        Debug.Log($"____ Player Count: {PhotonNetwork.CurrentRoom.PlayerCount} ____");

        int _count = 0;
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            Debug.Log("player Nick name: " + player.Value.NickName);
            Transform _panel = playerListPanel.transform.Find($"PlayerInfoPanel_{_count++}");
            //_panel.gameObject.SetActive(true);
            Transform _nameText = _panel.Find("PlayerNameText");
            _nameText.gameObject.SetActive(true);
            _nameText.GetComponent<UnityEngine.UI.Text>().text = player.Value.NickName;
            _panel.Find("ClassDropdown").gameObject.SetActive(true);
        }





    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"접속 유저 수: {PhotonNetwork.CurrentRoom.PlayerCount}");
        Debug.Log($"접속 끊긴 유저: {otherPlayer.NickName}");
        SetRoomInfo();
    }


    #endregion


}
