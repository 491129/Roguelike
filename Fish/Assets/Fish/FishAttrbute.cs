using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAttrbute : MonoBehaviour
{
    public int fishNumber = 0;
    public int fishSpeed = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            Destroy(gameObject);
        }
        if (collision.tag == "Bullet")
        {
            Destroy(gameObject);
        }

    }
}
