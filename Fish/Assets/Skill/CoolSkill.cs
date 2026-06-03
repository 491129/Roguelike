using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CoolSkill : MonoBehaviour
{
    static public int Skillcoin = 20;
    public static bool isUsed = false;
    void Start()
    {
        SkillManager.iscool = true;
       
    }
    private void Update()
    {
        
    }
    public static void FreezeAllFish()
    {
        SkillShopManager.Instance.StartCoroutine(FreezeCoroutine(3f));
    }

    private static IEnumerator FreezeCoroutine(float duration)
    {
        Fishself.IsFrozen = true;    // ∂≥Ω·À˘”–”„
        yield return new WaitForSecondsRealtime(duration);
        Fishself.IsFrozen = false;   // ª÷∏¥
    }
   
}
