using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishShadow : MonoBehaviour
{
    public GameObject shadow;
    Material shadowMat;

    void Start()
    {
        //if(!shadow) return;
        shadowMat=shadow.GetComponent<SpriteRenderer>().material;
        float angle = transform.rotation.eulerAngles.z;
        if (angle > 180f) angle -= 360f;
        bool shouldFlip = Mathf.Abs(angle) > 90f;
        shadowMat.SetInt("_VerticalFlip", shouldFlip? 1 : 0);
    }

    // Update is called once per frame
    void Update()
    {
        Texture FishTex = GetComponent<SpriteRenderer>().sprite.texture;
        shadowMat.SetTexture("_FishTex", FishTex);
    }
}