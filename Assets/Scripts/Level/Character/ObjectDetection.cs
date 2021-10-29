using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetection : MonoBehaviour
{
    [SerializeField] private GameObject effectPrefab;
    private ParticleSystem[] effectPool = new ParticleSystem[10];
    private int currentEffect;
    private bool finish = false;

    private void Start()
    {
        for (int i = 0; i < effectPool.Length; i++)
        {
            effectPool[i] = Instantiate(effectPrefab, new Vector2(-111, -111), Quaternion.identity).GetComponent<ParticleSystem>();
            effectPool[i].Stop();
        }
    }

    private void Death()
    {
        if (!I.full)
        {
            I.audioManager.Volume(.6f);
            I.audioManager.Play("Loss");
        }
        else
            I.audioManager.Play("Death");
        I.c_Controller.Death();
        I.levelUI.Death();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (finish || I.c_Controller.death)
            return;
        if (collision.tag == "Coin")
        {
            I.audioManager.Play("Coin");
            I.levelManager.coinsInLevel++;
            collision.gameObject.SetActive(false);
            I.levelUI.UpdateCoinsInLevel();
            EffectDestroyCoinAndStar(collision.transform);
            I.levelManager.CoinAddInList(collision.name);
        }
        else if (collision.tag == "Spike")
        {
            Death();
        }
        else if(collision.tag == "Enemy")
        {
            if (transform.position.y - .41f > collision.transform.position.y && I.c_Controller.stateCharacter == StateCharacter.Fall)
            {
                I.audioManager.Play("BoostJump");
                collision.gameObject.SetActive(false);
                I.c_Controller.Jump(true);
            }
            else
                Death();
        }
        else if(collision.tag == "Star")
        {
            I.audioManager.Play("Star");
            I.levelUI.UpdateStar();
            collision.gameObject.SetActive(false);
            I.levelManager.idStar++;
            EffectDestroyCoinAndStar(collision.transform);
        }
        else if(collision.tag == "Finish")
        {
            I.audioManager.Volume(.85f);
            if(I.levelManager.idStar == 3)
                I.audioManager.Play("Win");
            else
                I.audioManager.Play("Loss");
            finish = true;
            I.c_Controller.dontMove = true;
            I.levelUI.Completed("Finish");
        }
        else if(collision.tag == "Respawn")
        {
            I.levelManager.CheckPoint(collision.gameObject, (int)transform.localScale.x);
        }
    }

    private void EffectDestroyCoinAndStar(Transform _transform)
    {
        effectPool[currentEffect].Stop();
        effectPool[currentEffect].transform.position = _transform.position;
        effectPool[currentEffect].Play();
        currentEffect++;
        if (currentEffect >= 10)
            currentEffect = 0;
    }


}
