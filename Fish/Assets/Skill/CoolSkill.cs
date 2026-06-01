using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CoolSkill : MonoBehaviour
{
    static public int Skillcoin = 20;
    static public int Duration;
    void Start()
    {
       
    }
    private void Update()
    {
        SkillManager.iscool = true;
        FreezeAllFish(3f);
    }
    public void FreezeAllFish(float duration)
    {
        StartCoroutine(FreezeCoroutine(duration));
    }

    IEnumerator FreezeCoroutine(float duration)
    {
        Fishself.IsFrozen = true;    // ∂≥Ω·À˘”–”„
        yield return new WaitForSecondsRealtime(duration);
        Fishself.IsFrozen = false;   // ª÷∏¥
    }
   
}
