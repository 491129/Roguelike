using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCoin : MonoBehaviour
{
    static public int fishCoin;
    public GameObject fish;
    private void start()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Bullet")
        {
            FishAttrbute fishAttr = fish.GetComponent<FishAttrbute>();
        fishCoin = fishAttr.goldNum;
        if (SkillShopManager.YFskill)
        {
            GameManager.AddCoin(YFSkill.fishCoin);
        }
        else
            GameManager.AddCoin(fishAttr.goldNum);
        }
    }
}


