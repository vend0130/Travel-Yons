using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("Gameplay panel")]
    [SerializeField] private GameObject gameplay;
    [SerializeField] private Text coinsInLevelText;
    [SerializeField] private GameObject[] starsInGame = new GameObject[3];
    [SerializeField] private Text buttonControllText;
    [Header("Pause panel")]
    [SerializeField] private GameObject pause;
    [SerializeField] private Text currentLevelPause;
    [Header("Completed panel")]
    [SerializeField] private Text currentLevelCompleted;
    [SerializeField] private GameObject[] strasCompleted = new GameObject[3];
    [SerializeField] private Text allCoinsCompleted;
    [SerializeField] private Text coinsLevelCompleted;
    [SerializeField] private GameObject[] textCompleted = new GameObject[2];
    [SerializeField] private GameObject buttonNext;
    [SerializeField] private GameObject c3s;
    private Coroutine effectMoneyCompleted;
    [Header("Revive panel")]
    [SerializeField] private GameObject revive;
    [SerializeField] private Image timeImage;
    [SerializeField] private Animator reviveAnimation;
    [SerializeField] private GameObject buttonWatchAd;
    [SerializeField] private GameObject buttonBuy;
    [SerializeField] private Text buyRevive;
    [SerializeField] private Text reviveCoins;
    [Header("Hint panel")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private GameObject[] hints;
    [SerializeField] private Animator hintAnimator;
    private int openIdHint;

    private GameManager gm;

    private void Awake()
    {
        if (!I.loadMenu)
            Mainmenu();
        I.levelUI = this;
    }

    private void Start()
    {
        Time.timeScale = 1;
        gm = I.gm;
        ButtonControllText();
        currentLevelCompleted.text = "LEVEL " + (I.currentLevel + 1).ToString();
        currentLevelPause.text = "LEVEL " + (I.currentLevel + 1).ToString();
        coinsInLevelText.text = "0";
        if (I.adMob.countStep == 0)
            I.adMob.countStep = Random.Range(3, 6);

        if (I.currentLevel == 0)
            animator.SetInteger("ButtonControll", 1);
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ButtonSound();
            if (hintPanel.activeSelf)
                Hint(-1);
            else if (revive.activeSelf)
                ReviveAnimatioSpeed();
            else if (pause.activeSelf)
                Pause(2);
            else if (!pause.activeSelf && gameplay.activeSelf && !I.c_Controller.death)
                Pause(1);
        }

        if (Input.GetMouseButtonDown(0) && I.gmui.ErroShow)
            I.gmui.ErrorPanelOff();
    }

    public void ButtonSound()
    {
        I.audioManager.Play("Button");
    }

    public void Pause(int _value)//1  Открыть; 2 закрыть
    {
        animator.SetInteger("Pause", _value);
        animator.SetInteger("Gameplay", _value);
        Time.timeScale = _value - 1;
    }
    private void OnApplicationPause(bool hideApp)
    {
        if(hideApp && !I.c_Controller.death && gameplay.activeSelf && !pause.activeSelf)
            Pause(1);
    }

    //public bool ShaowAD()
    //{
    //    if (I.adMob.step == I.adMob.countStep)
    //    {
    //        I.adMob.step = 0;
    //        I.adMob.countStep = Random.Range(3, 6);
    //        return true;
    //    }
    //    else
    //    {
    //        I.adMob.step += 1;
    //        return false;
    //    }
    //}

    public void Again(bool _b)
    {
        if (_b)
            I.adMob.LookInterstitialAD(WhoCalledAD.I_LevelUI_Again);
        else
            ReScene();
    }

    public void Next(bool _b = false)
    {
        if(_b)
            I.adMob.LookInterstitialAD(WhoCalledAD.I_LevelUI_Next);
        else
        {
            if (I.currentLevel + 1 < gm.levelsLength)
            {
                I.currentLevel++;
                ReScene();
            }
            else
            {
                gm.worldSelectLevel = 1;
                gm.LoadSceneAsync("SelectLevel");
            }
        }
    }

    public void ReScene()
    {
        gm.LoadSceneAsync("Level");
    }

    public void Mainmenu()
    {
        gm.LoadSceneAsync("Mainmenu");
    }

    public void Respawn()
    {
        if (I.levelManager.respawn)
        {
            Pause(2);
            I.c_Controller.Respawn();
            animator.SetInteger("Revive", 2);
            animator.SetInteger("Gamaplay", 2);
        }
    }

    public void UpdateCoinsInLevel()
    {
        coinsInLevelText.text = (I.levelManager.coinsInLevel).ToString();
    }

    public void UpdateStar()
    {
        if (I.levelManager.idStar < 3)
        {
            starsInGame[I.levelManager.idStar].SetActive(true);
        }
    }

    public void Completed(string type)
    {
        I.c_Controller.Stop(true);
        animator.SetInteger("Completed", 1);
        for (int i = 0; i < I.levelManager.idStar; i++)
        {
            strasCompleted[i].SetActive(true);
            if (I.levelManager.idStar > 3)
                Debug.LogError("Oops...  " + I.levelManager.idStar);
        }
        Firebase.Analytics.FirebaseAnalytics.LogEvent("level_" + (I.currentLevel + 1) + "_finish");
        if (I.levelManager.idStar == 3 && type == "Finish")
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("level_" + (I.currentLevel + 1) + "_completed");
            allCoinsCompleted.text = I.coins.ToString();
            coinsLevelCompleted.text = "+" + I.levelManager.coinsInLevel.ToString();
            textCompleted[0].SetActive(true);
            textCompleted[1].SetActive(false);
            c3s.SetActive(false);
            buttonNext.SetActive(true);
            if(I.levelManager.coinsInLevel > 0)
                effectMoneyCompleted = StartCoroutine(EffectMoneyCompleted(I.coins, I.levelManager.coinsInLevel));
        }
        else
        {
            I.levelManager.coinsInLevel = 0;
            allCoinsCompleted.text = I.coins.ToString();
            coinsLevelCompleted.text = "+0";
            textCompleted[0].SetActive(false);
            textCompleted[1].SetActive(true);
            c3s.SetActive(true);
            buttonNext.SetActive(false);
        }
        if (type == "Finish")
            gm.DataSave();
        I.levelManager.idStar = 0;
        I.levelManager.coinsInLevel = 0;
    }

    public void Death()//умираем -> ждем -> показываем DeathShowPanel
    {
        animator.SetInteger("Gameplay", 1);
        StartCoroutine(Delay());
    }
    public void DeathShowPanel()//если есть реклама -> коины -> game over
    {
        if (I.full)
        {
            ReviveEnd(true);
            return;
        }

        if(I.levelManager.respawn && (I.adMob.CheckRewarLoad() || I.coins >= I.gm.moneyCount))
        {
            reviveCoins.text = I.coins.ToString();
            reviveAnimation.speed = 1;
            animator.SetInteger("Revive", 1);
            buttonBuy.SetActive(false);
            buttonWatchAd.SetActive(false);
            if (I.adMob.CheckRewarLoad())
            {
                buttonWatchAd.SetActive(true);
            }
            if (I.coins >= I.gm.moneyCount)
            {
                buyRevive.text = I.gm.moneyCount + " COINS";
                buttonBuy.SetActive(true);
            }
        }
        else
        {
            Completed("Death");
        }
    }
    public void ReviveAnimatioSpeed()//ускоряем показ возможности возрадиться
    {
        reviveAnimation.speed *= 1.8f;
    }

    public void WatcAD()//кнопка просмотр рекламы
    {
        I.adMob.LookRewardAD(WhoCalledAD.R_Level);
    }

    public void BuyRevive()//тоже но коины
    {
        I.audioManager.Play("Buy");
        I.coins -= I.gm.moneyCount;
        I.Save();
        ReviveEnd(true);
    }

    public void EarnedReward(int er)//рекламу посмотрели
    {
        if (I.adMob.earnedReward)
            ReviveEnd(true);
        else
        {
            I.gmui.Error(er);
            ReviveEnd(false);
        }
    }

    public void ReviveEnd(bool _revive)//возродиться или нет
    {
        if(animator.GetInteger("Revive") == 1)
            animator.SetInteger("Revive", 2);
        if (!_revive)
        {
            Completed("Death");
        }
        else
        {
            Time.timeScale = 1;
            I.c_Controller.Respawn();
            animator.SetInteger("Gameplay", 2);
            ButtonControllText();
        }
    }

    public void Hint(int id)
    {
        if (id >= 0)
        {
            I.audioManager.Play("Hint");
            I.c_Controller.Stop(true);
            hintPanel.SetActive(true);
            animator.SetInteger("Gameplay", 1);
            I.c_Controller.dontMove = true;
            openIdHint = id;
            hintAnimator.SetInteger("id", id);
        }
        else
        {
            ButtonControllText();
            hints[openIdHint].SetActive(false);
            hintPanel.SetActive(false);
            animator.SetInteger("Gameplay", 2);
        }
    }

    public void ButtonControllText()
    {
        if (I.currentLevel == 0 && animator.GetInteger("ButtonControll") == 1)
            animator.SetInteger("ButtonControll", 2);

        if (I.c_Controller.directionX == 0)
        {
            animator.SetBool("Dir", true);
            buttonControllText.text = "RUN";
        }
        else
        {
            animator.SetBool("Dir", false);
            buttonControllText.text = "STOP";
        }
    }

    public void ChamgeCharacterDirection(int _value)
    {
        if (I.c_Controller.stateCharacter != StateCharacter.Idle)
            return;
        Vector3 _ls = I.c_Controller.transform.localScale;
        _ls.x = _value;
        I.c_Controller.transform.localScale = _ls;
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(.75f);
        DeathShowPanel();
    }

    private IEnumerator EffectMoneyCompleted(int _all, int _level)
    {
        yield return new WaitForSeconds(1f);
        int _lenght = _level;
        for (int i = 0; i < _lenght; i++)
        {
            if (i % 3 == 0)
                I.audioManager.Play("Coin");
            _all++;
            _level--;
            allCoinsCompleted.text = _all.ToString();
            coinsLevelCompleted.text = "+" + _level.ToString();
            yield return new WaitForSeconds(.025f);
        }
        coinsLevelCompleted.text = "";
    }
}
