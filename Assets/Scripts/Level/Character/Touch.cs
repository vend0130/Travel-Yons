using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Touch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (I.c_Controller.death)
            return;
        if (I.c_Controller.stateCharacter == StateCharacter.Idle)
        {
            I.c_Controller.stateCharacter = StateCharacter.Run;
            if(I.c_Controller.transform.localScale.x == 1)
                I.c_Controller.directionX = 1;
            else
                I.c_Controller.directionX = -1;
            I.levelUI.ButtonControllText();
        }
        else
            I.c_Controller.jump = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        I.c_Controller.jump = false;
    }

    public void Button()
    {
        if (I.c_Controller.directionX == 0)
            I.c_Controller.Run();
        else
            I.c_Controller.Stop();

        I.levelUI.ButtonControllText();
    }
}
