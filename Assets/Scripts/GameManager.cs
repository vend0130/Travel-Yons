using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool _loadAnim;
    private int _moneyCount = 50;
    private const int _countMoneyUnlockLevel = 100;
    private const int _countCoinsBuy = 1000;
    private float _lastTimeShow = -1;
    private int _timeDelayShow = 2;
    [HideInInspector] public bool loadAnim { get => _loadAnim; set => _loadAnim = value; }
    [HideInInspector] public int moneyCount { get => _moneyCount; set => _moneyCount = value; }
    [HideInInspector] public int countMoneyUnlockLevel => _countMoneyUnlockLevel;
    [HideInInspector] public int countCoinsBuy => _countCoinsBuy;
    [HideInInspector] public float lastTimeShow { get => _lastTimeShow; set => _lastTimeShow = value; }
    [HideInInspector] public int timeDelayShow { get => _timeDelayShow; set => _timeDelayShow = value; }

    [SerializeField] private GMUI gmanim;
    private int _levelsLength;
    private int _worldSelectLevel;
    private bool _loadPanel;
    [HideInInspector] public int levelsLength => _levelsLength;
    [HideInInspector] public int worldSelectLevel { get => _worldSelectLevel; set => _worldSelectLevel = value; }
    [HideInInspector] public bool loadPanel { get => _loadPanel; set => _loadPanel = value; }

    private AsyncOperation async;
    private bool loadScene;
    private string nameScene;
    private Coroutine coroutine;
    private bool appStatus = true;

    public Levels[] levels;

    private void Awake()
    {
        if(I.gm != null)
        {
            Destroy(gameObject);
            return;
        }
        I.gm = this;
        DontDestroyOnLoad(gameObject);

        I.Load();
        Chck();

        _levelsLength = levels.Length;
    }

    private void Start()
    {
        if (I.currentLevel >= levels.Length || I.currentLevel < 0)
        {
            Debug.LogError("err");
            I.currentLevel = levels.Length - 1;
        }

        if (I.full)
            _moneyCount *= 3;
    }

    public void DataSave()
    {
        if (I.dataLevels[I.currentLevel].stars < I.levelManager.idStar)
            I.dataLevels[I.currentLevel].stars = I.levelManager.idStar;
        if (I.levelManager.idStar == 3)
        {
            I.dataLevels[I.currentLevel].coins.AddRange(I.levelManager.coinsList.ToArray());
            if (I.currentLevel + 1 < levels.Length)
                I.dataLevels[I.currentLevel + 1].unlock = true;
            I.coins += I.levelManager.coinsInLevel;
        }
        I.Save();
    }

    public void Chck()
    {
        bool _save = false;
        if (I.firstRun == false)
        {
            I.firstRun = true;
            I.coins = 0;
            I.currentLevel = 0;

            Array.Resize(ref I.dataLevels, levels.Length);
            for (int i = 0; i < I.dataLevels.Length; i++)
            {
                I.dataLevels[i] = new DataLevels() { stars = 0, unlock = false, coins = new List<string>() };
            }

            I.dataLevels[0].unlock = true;

            _save = true;
        }

        if(levels.Length > I.dataLevels.Length)
        {
            int _oldDL = I.dataLevels.Length;
            Array.Resize(ref I.dataLevels, levels.Length);
            for (int i = _oldDL; i < I.dataLevels.Length; i++)
            {
                I.dataLevels[i] = new DataLevels() { stars = 0, unlock = false, coins = new List<string>() };
            }

            _save = true;
        }

        if (_save)
            I.Save();
    }

    public void LoadSceneAsync(string _nameScene)
    {
        nameScene = _nameScene;
        loadScene = false;
        _loadPanel = true;
        gmanim.Anima("Transition", true);
        StartCoroutine(DelayLoad());
        LoadScene();
    }
    private IEnumerator DelayLoad()
    {
        async = SceneManager.LoadSceneAsync(nameScene);
        yield return true;
        async.allowSceneActivation = false;
        loadScene = true;
        LoadScene();
    }
    public void LoadScene()
    {
        if (loadScene && _loadAnim)
        {
            if (nameScene == "Level")
                I.gmui.Coins(false);
            else
                I.gmui.Coins(true);


            _loadPanel = false;
            Time.timeScale = 1;
            gmanim.Anima("Transition", false);
            async.allowSceneActivation = true;
        }
    }

    private void OnApplicationPause(bool _pause)
    {
        if (_pause)
            appStatus = !_pause;
    }

    public void OpenUrl(string _app, string _web)
    {
        if (coroutine != null)
            return;

        coroutine = StartCoroutine(Url(_app, _web));
    }

    private IEnumerator Url(string _app, string _web)
    {
        Application.OpenURL(_app);
        yield return new WaitForSeconds(1f);
        if (appStatus)
            Application.OpenURL(_web);
        appStatus = true;
        coroutine = null;
    }
}

[System.Serializable]
public class Levels
{
    public string name;
    public GameObject prefab;
}
