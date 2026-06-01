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
        WaitFor();
    }
    void WaitFor()
    {
        StartCoroutine(FreezeCoroutine(3f));
    }
    IEnumerator FreezeCoroutine(float duration)
    {
        Time.timeScale = 0f;
        Debug.Log("设定 timeScale=0，当前值：" + Time.timeScale);
        yield return new WaitForSecondsRealtime(duration);
        Debug.Log("恢复 timeScale=1，当前值：" + Time.timeScale);
        Time.timeScale = 1f;
    }
}
