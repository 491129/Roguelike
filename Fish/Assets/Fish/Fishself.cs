using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishself : MonoBehaviour
{
    private int speed;
    void Start()
    {
        speed = GetComponent<FishAttrbute>().fishSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right*speed * Time.deltaTime);
    }
}
