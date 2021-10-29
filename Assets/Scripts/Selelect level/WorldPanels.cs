using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WorldPanels : MonoBehaviour
{
    [SerializeField] private GameObject[] currentWorldIcons;
    [SerializeField] private Image[] arrows;
    [SerializeField] private RectTransform panel;

    private float width = 1080;
    private bool isTouch;
    private Vector2 firstPositionPanel;
    private float lastMousePosition;
    private int currentWorld;
    private bool isMovePanel;

    private void Start()
    {
        StartWorldPanel();
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
            Down();
        if (Input.GetMouseButtonUp(0) && isTouch)
            Up();
    }

    private void FixedUpdate()
    {
        if (isTouch || isMovePanel)
        {
            CheckCurrentWorld();
        }
        if (isMovePanel)
        {
            StepPanel();
        }
    }

    private void CheckCurrentWorld()
    {
        for (int i = 0; i < currentWorldIcons.Length; i++)
        {
            if (Mathf.Abs(panel.anchoredPosition.x - (-width * i)) < 540)
            {
                currentWorldIcons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
                currentWorldIcons[i].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                if (i == 0)
                {
                    arrows[0].color = new Color(1, 1, 1, .5f);
                    arrows[1].color = new Color(1, 1, 1, 1);
                }
                else if (i == currentWorldIcons.Length - 1)
                {
                    arrows[0].color = new Color(1, 1, 1, 1);
                    arrows[1].color = new Color(1, 1, 1, .5f);
                }
                else
                {
                    arrows[0].color = new Color(1, 1, 1, 1);
                    arrows[1].color = new Color(1, 1, 1, 1);
                }
            }
            else
            {
                currentWorldIcons[i].GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
                currentWorldIcons[i].GetComponent<Image>().color = new Color(1, 1, 1, .5f);
            }
        }
    }

    private void StepPanel()
    {
        float targetPosition = panel.anchoredPosition.x;
        targetPosition = -width * currentWorld;
        Vector2 newPanelPosition = panel.anchoredPosition;
        newPanelPosition.x = Mathf.SmoothStep(panel.anchoredPosition.x, targetPosition, 15 * Time.fixedDeltaTime);
        panel.anchoredPosition = newPanelPosition;

        if (Mathf.Round(panel.anchoredPosition.x) == targetPosition)
        {
            Vector3 def = panel.anchoredPosition;
            def.x = Mathf.Round(panel.anchoredPosition.x);
            panel.anchoredPosition = def;
            isMovePanel = false;
        }
    }

    private void StartWorldPanel()
    {
        if (I.gm.worldSelectLevel == 1)
        {
            currentWorld = 1;
            isMovePanel = true;
            I.gm.worldSelectLevel = 0;
        }
    }

    public void Arrow(int dir)
    {
        currentWorld += dir;
        if (currentWorld < 0)
            currentWorld = 0;
        else if (currentWorld >= currentWorldIcons.Length)
            currentWorld = currentWorldIcons.Length - 1;
        isMovePanel = true;
    }

    public void Down()
    {
        firstPositionPanel = panel.anchoredPosition;
        lastMousePosition = Input.mousePosition.x;
        isTouch = true;
        isMovePanel = false;
    }

    public void Up()
    {
        if (firstPositionPanel != panel.anchoredPosition)
        {
            float percent = (panel.anchoredPosition.x - firstPositionPanel.x) * 100 / -width;
            if (Mathf.Abs(percent) > 10)//сдвигаем но 10% и изменяем текущий world
            {
                currentWorld += (int)Mathf.Sign(percent);
            }
        }
        isMovePanel = true;
        isTouch = false;
    }
}
