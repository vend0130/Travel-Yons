using UnityEngine;
using UnityEngine.UI;

public class SelectLevel : MonoBehaviour
{
    [SerializeField] private GameObject prefabButton;
    [SerializeField] private Transform parentButtons;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject unlockPanel;
    [SerializeField] private Text textCurrentLevel;
    [SerializeField] private Text textPreviousLevel;
    [SerializeField] private Text textCoins;

    [SerializeField] private GameObject[] stars;

    private GameManager gm;
    private int selectLevel = 0;
    private ButtonSelecetLevel[] buttons;

    private void Awake()
    {
        I.selectLevel = this;
        gm = I.gm;
        if (!I.loadMenu)
            Back();

        InstantiateButtons();
    }

    private void Start()
    {
        I.audioManager.PlayTheme("BackgroundMenu");
        Firebase.Analytics.FirebaseAnalytics.LogEvent("scene_select_level");
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !I.gm.loadPanel)
        {
            if (I.gmui.ActiveWatchAdPanel)
                I.gmui.WatchAdPanel(2);
            else if (unlockPanel.activeSelf)
                animator.SetInteger("Unlock", 2);
            else
                Back();
            ButtonSound();
        }

        if (Input.GetMouseButtonDown(0) && I.gmui.ErroShow)
            I.gmui.ErrorPanelOff();
    }

    private void InstantiateButtons()
    {
        buttons = new ButtonSelecetLevel[I.dataLevels.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            GameObject _b = Instantiate(prefabButton);
            _b.transform.SetParent(parentButtons, false);
            buttons[i] = _b.GetComponent<ButtonSelecetLevel>();
            buttons[i].level = i;
        }
    }

    public void ButtonSound()
    {
        I.audioManager.Play("Button");
    }

    public void Back()
    {
        if (I.gm.loadPanel)
            return;
        gm.LoadSceneAsync("Mainmenu");
    }

    public void GoLevel(int level)
    {
        selectLevel = level;
        if (!I.dataLevels[level].unlock)
        {
            UnlockLevelActive(1);
        }
        else
        {
            if (level > 1)//3...
                I.adMob.LookInterstitialAD(WhoCalledAD.I_SelectLevel);
            else
                GoLevelDublicat();
        }
    }

    public void GoLevelDublicat()
    {
        I.currentLevel = selectLevel;
        gm.LoadSceneAsync("Level");
    }

    public void UnlockLevelActive(int _value)
    {
        if(_value == 1)
        {
            textCurrentLevel.text = "LEVEL " + (selectLevel + 1) + " LOCK";
            textPreviousLevel.text = "COLLECT 3 STARS ON " + selectLevel + " LEVEL";
            textCoins.text = I.gm.countMoneyUnlockLevel.ToString();
        }

        animator.SetInteger("Unlock", _value);
    }

    public void UnlockLevelYes()
    {
        if(I.coins >= I.gm.countMoneyUnlockLevel)
        {
            I.gmui.BuyEffect(I.gm.countMoneyUnlockLevel);
            I.audioManager.Play("Buy");
            I.coins -= I.gm.countMoneyUnlockLevel;
            I.dataLevels[selectLevel].unlock = true;
            buttons[selectLevel].UnlockButton();
            UnlockLevelActive(2);
            I.gmui.UpdateTextCoins();
            I.Save();
            Firebase.Analytics.FirebaseAnalytics.LogEvent("unlock_level");
        }
        else
        {
            I.gmui.WatchAdPanel(1);
        }
    }

    public void Rate(int _value)
    {
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < _value)
                stars[i].SetActive(true);
            else
                stars[i].SetActive(false);
        }
    }

    public void GoRate()
    {
        I.gm.OpenUrl(I.storeAppLink, I.storeWebLink);
    }
}
