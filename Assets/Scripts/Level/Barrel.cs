using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] private GameObject staticImage;
    [SerializeField] private GameObject animationImage;
    [SerializeField] private Transform maxPointBattel;
    [SerializeField] private BoxCollider2D boxCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && collision.transform.position.y - .41f > transform.position.y && I.c_Controller.stateCharacter == StateCharacter.Fall)
        {
            I.audioManager.Play("BoostJump");
            staticImage.SetActive(false);
            animationImage.SetActive(true);
            boxCollider.enabled = false;
            I.c_Controller.Jump(true);
            I.levelManager.coinsInLevel += 5;
            I.levelUI.UpdateCoinsInLevel();
        }
    }
}
