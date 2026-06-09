using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishShadow : MonoBehaviour
{
    public GameObject shadow;
    Material shadowMat;
    private float lastAngle;

    void Start()
    {
        //if(!shadow) return;
        shadowMat=shadow.GetComponent<SpriteRenderer>().material;
        lastAngle = float.NaN;
        float angle = transform.rotation.eulerAngles.z;
        if (angle > 180f) angle -= 360f;
        bool shouldFlip = Mathf.Abs(angle) > 90f;
        shadowMat.SetFloat("_VerticalFlip", shouldFlip? 1 : 0);
        //Debug.Log(GetComponent<SpriteRenderer>().flipX);
        //var FishTex=GetComponent<SpriteRenderer>().sprite.texture;
        //shadowMat.SetTexture("_FishTex", FishTex);
    }

    // Update is called once per frame
    void Update()
    {
        Texture FishTex = GetComponent<SpriteRenderer>().sprite.texture;
        //shadowMat.SetFloat("_VerticalFlip", GetComponent<SpriteRenderer>().flipX ? 1 : 0);
        shadowMat.SetTexture("_FishTex", FishTex);
    }
}