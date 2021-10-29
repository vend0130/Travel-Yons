using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBall : MonoBehaviour
{
    [SerializeField] private Transform enemy;
    [SerializeField] private Transform[] points = new Transform[2];
    [SerializeField] private float speed = 1f;
    private int targetPoint = 0;

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        enemy.transform.position = Vector2.MoveTowards(enemy.transform.position, points[targetPoint].position, speed * Time.fixedDeltaTime);
        if (points[targetPoint].position.x == enemy.position.x)
            Flip();
    }

    private void Flip()
    {
        Vector3 _scale = enemy.transform.localScale;
        _scale.x *= -1;
        enemy.transform.localScale = _scale;
        if (targetPoint == 0)
            targetPoint = 1;
        else
            targetPoint = 0;
    }
}
