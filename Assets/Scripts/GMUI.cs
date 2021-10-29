using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Purchasing;

public class GMUI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [Header("FOR COINS")]
    [SerializeField] private GameObject allCoinsPanel;
    [SerializeField] private Text textAllCoins;
    [SerializeField] private Text textAllCoins2;
    [SerializeField] private Text countPlusCoinsWatchAD;
    [SerializeField] private Text effectMinusCoins;
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private Text coinsCountBuy;
    [SerializeField] private RectTransform heightPanel;
    [Header("Error")]
    [SerializeField] private Text texter;
    [SerializeField] private Text texter2; 

    private bool _errorShow;
    [HideInInspector] public bool ErroShow => _errorShow;

    private bool _activeWatchAdPanel;
    [HideInInspector] public bool ActiveWatchAdPanel => _activeWatchAdPanel;

    private int whoPlus;

    private void Awake()
    {
        if (I.gmui != null)
        {
            Destroy(gameObject);
            return;
        }
        I.gmui = this;
    }

    public void OnPurchaseComplete()
    {
        I.full = true;
        I.Save();
        I.audioManager.Play("Win");
        I.gm.moneyCount *= 3;
        WatchAdPanel(0);
        whoPlus = I.gm.countCoinsBuy;
        animator.SetBool("PlusCoins", true);
        if (I.mainMenu != null)
            I.mainMenu.CloseButtonRemoveAD();
    }

    public void BuyPanel()
    {
        bool _show = false;
        if (!I.full)
            _show = true;

        if (!_show)
        {
            buyPanel.SetActive(false);
            Vector2 _sezeDelta = heightPanel.sizeDelta;
            _sezeDelta.y = 350;
            heightPanel.sizeDelta = _sezeDelta;
        }
        else
        {
            buyPanel.SetActive(true);
            Vector2 _sezeDelta = heightPanel.sizeDelta;
            _sezeDelta.y = 800;
            heightPanel.sizeDelta = _sezeDelta;
            coinsCountBuy.text = I.gm.countCoinsBuy.ToString();
        }
        countPlusCoinsWatchAD.text = "x" + I.gm.moneyCount;
    }

    //public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    //{
    //    Debug.LogError("Purchase of product " + product.definition.id + " failed because " + reason);
    //    Error(0);
    //}

    public void ButtonSound()
    {
        I.audioManager.Play("Button");
    }

    public void Anima(string name, bool value)
    {
        if(name == "Transition")
            I.gm.loadAnim = false;
        animator.SetBool(name, value);
    }

    public void EndAnimTransition()
    {
        I.gm.loadAnim = true;
        I.gm.LoadScene();
    }

    public void Error(int er)//er 2 не готово er 1 закрыли er 0 ощибка покупки
    {
        switch (er)
        {
            case 0:
                texter.text = "ERROR" + "\n\n" + "Purchase failed";
                texter2.text = "ERROR" + "\n\n" + "Purchase failed";
                break;
            case 1:
                texter.text = "ERROR" + "\n\n" + "reward is not received";
                texter2.text = "ERROR" + "\n\n" + "reward is not received";
                break;
            case 2:
                texter.text = "NO VIDEO AVAILABLE" + "\n\n" + "please try again later";
                texter2.text = "NO VIDEO AVAILABLE" + "\n\n" + "please try again later";
                break;
        }

        _errorShow = true;
        Anima("Error", true);
    }

    public void ErrorPanelOff()
    {
        if (_errorShow)
            EndAnimError();
    }


    public void EndAnimError()
    {
        _errorShow = false;
        Anima("Error", false);
    }

    public void Coins(bool _value)
    {
        allCoinsPanel.SetActive(_value);
        if (_value)
        {
            UpdateTextCoins();
        }
    }

    public void WatchAdPanel(int value)
    {
        animator.SetInteger("WatchAdPanel", value);
        if (value == 1)
        {
            BuyPanel();
            countPlusCoinsWatchAD.text = "x" + I.gm.moneyCount.ToString();
            _activeWatchAdPanel = true;
            I.gm.timeDelayShow = 5;
            I.gm.lastTimeShow = Time.time;
            Firebase.Analytics.FirebaseAnalytics.LogEvent("more_coins");
        }
        else
            _activeWatchAdPanel = false;
    }

    public void WatchAD()
    {
        I.adMob.LookRewardAD(WhoCalledAD.R_gmui);
    }
    public void EarnedReward(int er)
    {
        if (I.adMob.earnedReward)
        {
            whoPlus = I.gm.moneyCount;
            animator.SetBool("PlusCoins", true);
        }
        else
        {
            Error(er);
        }
    }
    public void PlusCoins()
    {
        I.audioManager.Play("Coin");
        I.coins += (int)(whoPlus / 5);
        UpdateTextCoins();
    }
    public void StopPlusCoins()
    {
        animator.SetBool("PlusCoins", false);
        I.Save();
    }

    public void UpdateTextCoins()
    {
        textAllCoins.text = I.coins.ToString();
        textAllCoins2.text = I.coins.ToString();
    }

    public void BuyEffect(int _value)
    {
        effectMinusCoins.text = "-" + _value.ToString();
        animator.SetBool("Buy", true);
        I.audioManager.Play("Buy");
    }

    public void StopBuyEffect()
    {
        animator.SetBool("Buy", false);
    }
}
