using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishself : MonoBehaviour
{
    private FishAttrbute attr;
    public static bool IsFrozen;

    void Start()
    {
        attr = GetComponent<FishAttrbute>();
    }

    void Update()
    {
        if (IsFrozen) return;
        if (attr != null && attr.isDead) return;
        float speed = attr.CurrentSpeed;
        //Debug.Log(speed);
        transform.Translate(Vector3.right * speed * Time.deltaTime);
       
    }
}
