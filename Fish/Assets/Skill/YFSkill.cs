using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YFSkill : MonoBehaviour
{
    static public int fishCoin;
    public int addCoin=8;
    //static public int Skillcoin=10;
    public GameObject fish;
    public static bool isUsed=false;
    void Start()
    {
        FishAttrbute fishAttr = fish.GetComponent<FishAttrbute>();
        fishCoin = fishAttr.goldNum + addCoin;
    }

}
