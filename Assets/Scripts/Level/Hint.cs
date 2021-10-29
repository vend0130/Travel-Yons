using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] private int idHint;
    private bool show = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !show)
        {
            I.levelUI.Hint(idHint);
            show = true;
        }
    }
}
