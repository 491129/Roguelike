using System.Collections;
using UnityEngine;

public class DisDialogue : MonoBehaviour
{
    [SerializeField] private float delay = 3f;

    private void OnEnable()
    {
        Debug.Log($"{gameObject.name} 被激活，将在 {delay} 秒后消失");
        StartCoroutine(DestroyAfterDelay(delay));
    }

    private IEnumerator DestroyAfterDelay(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Debug.Log($"{gameObject.name} 时间到，即将禁用");
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject.name} 被禁用，协程停止");
        StopAllCoroutines();
    }
}