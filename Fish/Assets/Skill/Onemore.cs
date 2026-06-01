using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onemore : MonoBehaviour
{
    static public int Skillcoin = 15;

    public GameObject TT;
    public int TTNum;
    void Start()
    {
        TTNum++;
        TT.SetActive(true);
    }
}
