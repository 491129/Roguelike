using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishself : MonoBehaviour
{
    private int speed;
    private FishAttrbute attr;
    void Start()
    {
        attr = GetComponent<FishAttrbute>();
        speed = attr.fishSpeed;
    }

    void Update()
    {
        if (attr != null && attr.isDead) return;
        transform.Translate(Vector3.right*speed * Time.deltaTime);
    }
}
