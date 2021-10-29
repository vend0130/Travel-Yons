using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretRoom : MonoBehaviour
{
    [SerializeField] private Transform opacityObject;

    private float currentOpacity;
    private List<SpriteRenderer> sprites;
    private float timeEffect = .15f;
    private Coroutine coroutine;

    private void Start()
    {
        sprites= new List<SpriteRenderer>();
        foreach (Transform sr in opacityObject)
        {
            sprites.Add(sr.GetComponent<SpriteRenderer>());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(Opacity());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(Opacity());
        }
    }

    IEnumerator Opacity()
    {
        float fraction = 0;
        float i_StartTime = Time.realtimeSinceStartup;
        Color current = sprites[0].color;
        Color target = sprites[0].color;
        if (current.a == 1)
        {
            target.a = 0;
        }
        else
        {
            target.a = 1;
        }

        while (fraction < 1)
        {
            fraction = Mathf.Clamp01((Time.realtimeSinceStartup - i_StartTime) / timeEffect);
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].color = Color.Lerp(current, target, fraction);
            }
            yield return null;
        }
    }
}
