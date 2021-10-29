using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] public GameObject setings;
    [SerializeField] private Image sfx;
    [SerializeField] private Sprite[] sfxIcons;
    [SerializeField] private Image music;
    [SerializeField] private Sprite[] musicIcon;
    [SerializeField] private Text versionApp;
    [SerializeField] private GameObject deleteProgress;
    [SerializeField] private RectTransform heightSettings;
    [SerializeField] private GameObject removeADInSetting;

    [SerializeField] private Transform background;
    private Vector2 zeroPositionBackground;

    private GameManager gm;
    private bool tap;

    //private Coroutine coroutine;
    //private bool appStatus = true;
    private bool appPaused = false;
    private int ccount;

    private void Awake()
    {
        I.mainMenu = this;
        I.loadMenu = true;
    }

    private void Start()
    {
        I.audioManager.PlayTheme("BackgroundMenu");
        gm = I.gm;

        I.gmui.Coins(true);

        zeroPositionBackground = background.position;

        versionApp.text = "ver: " + Application.version;

        if(I.gm.lastTimeShow == -1)
        {
            I.gm.lastTimeShow = Time.time;
            if(Random.Range(0, 30) == 1)
                I.gmui.WatchAdPanel(1);
        }

        float _time = (Time.time - I.gm.lastTimeShow) / 60;
        int _random = Random.Range(0, 2);
        if (_time >= I.gm.timeDelayShow && _random == 1)
        {
            I.gmui.WatchAdPanel(1);
        }

        if (I.full)
            CloseButtonRemoveAD();
    }


    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonSound();
            if (I.gmui.ActiveWatchAdPanel)
                I.gmui.WatchAdPanel(2);
            else if (deleteProgress.activeSelf)
                animator.SetInteger("Delete", 0);
            else if (setings.activeSelf)
                animator.SetInteger("Settings", 2);
            else
            {
                if (!tap)
                {
                    animator.SetBool("TapAgain", true);
                    tap = true;
                }
                else
                    Quit();
            }
        }

        if (Input.GetMouseButtonDown(0) && I.gmui.ErroShow)
            I.gmui.ErrorPanelOff();

#if UNITY_ANDROID
        Vector2 _bp= background.position;
        _bp.y = zeroPositionBackground.y + 50 * -Input.acceleration.y;
        background.position = _bp;
#endif
    }

    public void ButtonSound()
    {
        I.audioManager.Play("Button");
    }

    public void GoLevel()
    {
        gm.LoadSceneAsync("Level");
    }

    public void SelectLevel()
    {
        gm.LoadSceneAsync("SelectLevel");
    }

    public void SelectYon()
    {
        gm.LoadSceneAsync("SelectYon");
    }

    public void Quit()
    {
        Debug.Log("QUIT...");
        Application.Quit();
    }

    public void Settings()
    {
        SettingsUpdate();
        if (!setings.activeSelf)
            animator.SetInteger("Settings", 1);
        else
            animator.SetInteger("Settings", 2);

    }

    public void SfxButton()
    {
        if (I.sfx)
            I.sfx = false;
        else
            I.sfx = true;
        SettingsUpdate(true);
    }
    public void MusicButton()
    {
        if (I.music)
        {
            I.music = false;
            I.audioManager.StopTheme();
        }
        else
        {
            I.music = true;
            I.audioManager.PlayTheme("BackgroundMenu");
        }
        SettingsUpdate(true);
    }

    public void RemoveADS()
    {
        I.gmui.WatchAdPanel(1);
    }

    public void UrlButton(int _value)
    {
        //if (coroutine != null)
        //    return;

        string _app = "";
        string _web = "";
        switch (_value)
        {
            case 0:
                _app = I.instagramAppLink;
                _web = I.instagramWebLink;
                break;
            case 1:
                _app = I.storeAppLink;
                _web = I.storeWebLink;
                break;
            case 2:
                _app = I.youtubeWebLink;
                _web = I.youtubeWebLink;
                break;
        }

        if (_app != "")
            I.gm.OpenUrl(_app, _web);//coroutine = StartCoroutine(Url(_app, _web));
    }

    public void SettingsUpdate(bool _update = false)
    {
        if (I.sfx)
            sfx.sprite = sfxIcons[0];
        else
            sfx.sprite = sfxIcons[1];
        if (I.music)
            music.sprite = musicIcon[0];
        else
            music.sprite = musicIcon[1];

        if (_update)
            I.Save();
    }

    public void CloseButtonRemoveAD()
    {
        Vector2 _sezeDelta = heightSettings.sizeDelta;
        _sezeDelta.y = 650;
        heightSettings.sizeDelta = _sezeDelta;
        removeADInSetting.SetActive(false);
    }

    public void DeleteProgress(int _value)
    {
        if (_value == 03137)
        {
            //bool _full = I.full;
            I.ClearSave();
            I.gm.Chck();
            //if (_full)
            //{
            //    I.full = _full;
            //    I.Save();
            //}
            I.gm.moneyCount /= 3;

            I.audioManager.Play("Delete");
            I.audioManager.PlayTheme("BackgroundMenu");
            I.gm.LoadSceneAsync("Mainmenu");
            Firebase.Analytics.FirebaseAnalytics.LogEvent("delete_progress");
        }
        else
            animator.SetInteger("Delete", _value);

#if UNITY_ANDROID
        if (_value == 1)
            Handheld.Vibrate();
#endif
    }

    public void UpCoins(int _vaule)
    {
        if(ccount >= 10)
        {
            I.coins += 500;
            I.gmui.UpdateTextCoins();
            I.Save();
            return;
        }
        ccount += _vaule;
    }

    public void StopAnim()
    {
        animator.SetBool("TapAgain", false);
        tap = false;
    }

    private void OnApplicationPause(bool _pause)
    {
        ////if (_pause)
        ////    appStatus = false;

        if (appPaused && !I.full)
            I.gmui.WatchAdPanel(1);
        appPaused = _pause;
    }

    //private IEnumerator Url(string _app, string _web)
    //{
    //    Application.OpenURL(_app);
    //    yield return new WaitForSeconds(1f);
    //    if (appStatus)
    //        Application.OpenURL(_web);
    //    appStatus = true;
    //    coroutine = null;
    //}
}
