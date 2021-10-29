using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private ParallaxLayer[] parallaxLayers;
    [SerializeField] private Transform camerat;
    [SerializeField] private GameObject[] clouds;
    private Vector3 startCameraPosition;
    private int step = 27;

    private void Start()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            parallaxLayers[i].startPosition = parallaxLayers[i].layer.position;
        }
        startCameraPosition = camerat.position;

        clouds[Random.Range(0, clouds.Length)].SetActive(true);
    }

    private void FixedUpdate()
    {
        Vector3 newPositionLayer = Vector2.zero;
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            newPositionLayer = parallaxLayers[i].layer.position;
            newPositionLayer.x = camerat.position.x * parallaxLayers[i].effect.x;
            newPositionLayer.y = camerat.position.y * parallaxLayers[i].effect.y;

            parallaxLayers[i].layer.position = newPositionLayer;

            for (int j = 0; j < parallaxLayers[i].sprites.Length; j++)
            {
                if (camerat.position.x - parallaxLayers[i].sprites[j].position.x > 15)
                    parallaxLayers[i].sprites[j].position += Vector3.right * step;
                else if (camerat.position.x - parallaxLayers[i].sprites[j].position.x < -15)
                    parallaxLayers[i].sprites[j].position += Vector3.left * step;
            }
        }
    }
}

[System.Serializable]
public class ParallaxLayer
{
    public string name;
    public Transform layer;
    public Vector2 effect;
    public Transform[] sprites = new Transform[3];
    [HideInInspector] public Vector2 startPosition;
}
