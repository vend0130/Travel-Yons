using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Range(0, 100)] [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset; // 1, 2.5, 10

    private float duration = .15f;
    private float magnitude = .3f;

    private void Awake()
    {
        I.camera = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            StartCameraShake();
    }

    private void FixedUpdate()
    {
        if (!I.c_Controller.death)
            Follow();
    }

    private void Follow()
    {
        Vector3 newPosition = offset;
        newPosition.x *= I.c_Controller.transform.localScale.x;
        newPosition += I.c_Controller.transform.position;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, newPosition, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }

    public void StartCameraShake()
    {
        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        Vector3 original = transform.position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(original.x + x, original.y + y, original.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
