using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectYon : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Yons[] yons;

    [SerializeField] private Text minusPrice;

    [SerializeField] private GameObject buttonBuy;
    [SerializeField] private Text textButtonBuy;
    [SerializeField] private GameObject buttonSelect;
    [SerializeField] private GameObject buttonSelected;
    [SerializeField] private Transform background;
    private Vector2 zeroPositionBackground;
    private GameObject activeButton;

    private int select;

    private void Awake()
    {
        if (I.yons == null || I.yons.Length < yons.Length)
        {
            Array.Resize(ref I.yons, yons.Length);
            I.yons[0] = true;

            I.Save();
        }
    }

    private void Start()
    {
        activeButton = buttonSelected;
        for (int i = 0; i < I.yons.Length; i++)
        {
            if(I.yons[i] == true)
            {
                UnlockYon(i);
            }
        }

        Choice(I.yon);
        SwitchAnimationYon();
        I.gmui.UpdateTextCoins();
        Firebase.Analytics.FirebaseAnalytics.LogEvent("scene_select_character");
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !I.gm.loadPanel)
        {
            if (I.gmui.ActiveWatchAdPanel)
                I.gmui.WatchAdPanel(2);
            else
                Back();
            ButtonSound();
        }

        if (Input.GetMouseButtonDown(0) && I.gmui.ErroShow)
            I.gmui.ErrorPanelOff();

#if UNITY_ANDROID
        Vector2 _bp = background.transform.position;
        _bp.y = zeroPositionBackground.y + 50 * -Input.acceleration.y;
        background.transform.position = _bp;
#endif
    }

    public void Back()
    {
        if (I.gm.loadPanel)
            return;
        I.gm.LoadSceneAsync("Mainmenu");
    }

    public void ButtonSound()
    {
        I.audioManager.Play("Button");
    }

    public void Choice(int _value)
    {
        yons[select].ramka.SetActive(false);
        select = _value;
        yons[select].ramka.SetActive(true);

        UpdateButtons();

    }

    private void UpdateButtons()
    {
        activeButton.SetActive(false);

        if (I.yon == select)
        {
            activeButton = buttonSelected;
            buttonSelected.SetActive(true);
        }
        else if (I.yons[select])
        {
            activeButton = buttonSelect;
            buttonSelect.SetActive(true);
        }
        else
        {
            activeButton = buttonBuy;
            textButtonBuy.text = yons[select].price.ToString();
            buttonBuy.SetActive(true);
        }
    }

    public void ButtonBuy()
    {
        if(yons[select].price <= I.coins)
        {
            I.gmui.BuyEffect(yons[select].price);
            I.coins -= yons[select].price;
            I.yons[select] = true;
            I.Save();
            I.gmui.UpdateTextCoins();
            UnlockYon(select);
            ButtonSelect();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("buy_character");
        }
        else
        {
            I.gmui.WatchAdPanel(1);
        }
    }

    public void ButtonSelect()
    {
        I.yon = select;
        I.Save();
        UpdateButtons();
        SwitchAnimationYon();
        I.audioManager.Play("Select");
    }

    public void StopBuyEffect()
    {
        animator.SetBool("BuyEffect", false);
    }

    private void SwitchAnimationYon()
    {
        switch (I.yon)
        {
            case 0:
                animator.SetInteger("Yon", 0);
                break;
            case 1:
                animator.SetInteger("Yon", 1);
                break;
        }
    }

    private void UnlockYon(int value)
    {
        yons[value].yon.SetActive(true);
        yons[value].lockSprite.SetActive(false);
    }
}

[System.Serializable]
public class Yons
{
    public string name;
    public GameObject ramka;
    public int price;
    public GameObject yon;
    public GameObject lockSprite;
}
