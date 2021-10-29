using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSelecetLevel : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject lockElement;
    [SerializeField] private GameObject unlockElement;
    [SerializeField] private Text textButtonLevel;
    [SerializeField] private GameObject[] starsLevel;
    private int _level = 0;
    public int level { get => _level; set => _level = value; }

    private void Start()
    {
        ButtonUpdate();
    }

    private void ButtonUpdate()
    {
        if(_level == 0 || I.dataLevels[_level].unlock)
        {
            lockElement.SetActive(false);
            unlockElement.SetActive(true);
            textButtonLevel.text = (_level + 1).ToString();
            for (int i = 0; i < I.dataLevels[_level].stars; i++)
            {
                starsLevel[i].SetActive(true);
            }
        }
        else
        {
            lockElement.SetActive(true);
            unlockElement.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        I.audioManager.Play("Button");
        I.selectLevel.GoLevel(_level);
    }

    public void UnlockButton()
    {
        lockElement.SetActive(false);
        unlockElement.SetActive(true);
        textButtonLevel.text = (_level + 1).ToString();
        for (int i = 0; i < I.dataLevels[_level].stars; i++)
        {
            starsLevel[i].SetActive(true);
        }
    }
}
