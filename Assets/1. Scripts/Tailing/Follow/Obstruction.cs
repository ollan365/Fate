using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstruction : MonoBehaviour
{
    public FollowFinishMiniGame followFinishMiniGame;
    public float speed;

    void Update()
    {
        transform.position += Vector3.down * speed * 0.01f;
        if(transform.position.y < -10) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(followFinishMiniGame.OnHit());
        }
    }
}
