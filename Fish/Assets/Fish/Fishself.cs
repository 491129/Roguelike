using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishself : MonoBehaviour
{
    private int speed;
    private FishAttrbute attr;
    public static bool IsFrozen;
    void Start()
    {
        attr = GetComponent<FishAttrbute>();
        speed = attr.fishSpeed;
    }

    void Update()
    {
        if (IsFrozen) return;
        //if (isDead) return;
        if (attr != null && attr.isDead) return;
        transform.Translate(Vector3.right*speed * Time.deltaTime);
       
    }
}
