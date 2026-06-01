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
        SkillManager.iscool = true;
        WaitFor();
    }
    private void Update()
    {
        
    }
    void WaitFor()
    {
        Debug.Log("1111");
        StartCoroutine(FreezeCoroutine(3f));
    }
    IEnumerator FreezeCoroutine(float duration)
    {
        Time.timeScale = 0f;
        Debug.Log("222");
        yield return new WaitForSecondsRealtime(duration);
        Debug.Log("333");
        Time.timeScale = 1f;
    }
}
