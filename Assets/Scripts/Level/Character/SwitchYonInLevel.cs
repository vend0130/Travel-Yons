using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchYonInLevel : MonoBehaviour
{
    [SerializeField] private Yon[] yons;

    private void Start()
    {
        for (int i = 0; i < yons.Length; i++)
        {
            yons[i].yon.SetActive(false);
        }
        yons[I.yon].yon.SetActive(true);
        I.c_Controller.animator = yons[I.yon].yon.GetComponent<Animator>();
        yons[I.yon].particalDeath.Stop();
        yons[I.yon].particalDeath.gameObject.SetActive(true);
    }

    public void Death(Vector2 _positionCharacter)
    {
        yons[I.yon].particalDeath.transform.position = _positionCharacter;
        yons[I.yon].particalDeath.Play();
    }
}

[System.Serializable]
public class Yon
{
    public string name;
    public GameObject yon;
    public ParticleSystem particalDeath;
}
