using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstruction : MonoBehaviour
{
    public FollowFinishMiniGame followFinishMiniGame;

    void Update()
    {
        transform.position += Vector3.down * Time.deltaTime * 5;
        if(followFinishMiniGame.isGameOver || transform.position.y < -30) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(followFinishMiniGame.OnHit());
        }
    }
}
