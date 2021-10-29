using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveGreen : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        animator.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if(!animator.enabled)
                animator.enabled = true;
            animator.Play("ActiveGrassTrue", -1, 0);
        }
    }
}
