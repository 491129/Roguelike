using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisDialogue : MonoBehaviour
{
    [SerializeField] private float autoHideDelay = 3f;     // 自动消失时间（仅对临时消息有效）
    [SerializeField] private float typingSpeed = 0.05f;    // 每个字的间隔秒数，越小越快
    [SerializeField] private Text dialogueText;            // 对话框内的 Text 组件

    private Coroutine typingCoroutine;
    private Coroutine autoHideCoroutine;

    private void OnEnable()
    {
        // 激活时不自动做任何事，由外部调用 ShowXxx 控制
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    /// <summary>
    /// 显示临时消息，打字完毕后等待 autoHideDelay 秒自动消失
    /// </summary>
    public void ShowTemporary(string message, float customDuration = -1f)
    {
        StartTyping(message);
        // 重新启动自动隐藏协程
        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);
        float duration = customDuration > 0 ? customDuration : autoHideDelay;
        autoHideCoroutine = StartCoroutine(AutoHideAfterTyping(duration));
    }

    /// <summary>
    /// 显示持久消息（鼠标悬停时用），不自动消失
    /// </summary>
    public void ShowMessage(string message)
    {
        // 停止自动隐藏，然后打字
        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);
        StartTyping(message);
    }

    /// <summary>
    /// 立即隐藏对话框，停止所有文字输出
    /// </summary>
    public void HideMessage()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    // 统一开始打字的方法
    private void StartTyping(string message)
    {
        // 确保对话框是激活的
        gameObject.SetActive(true);
        // 停止上一次的打字协程
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(message));
    }

    // 逐字输出
    private IEnumerator TypeText(string message)
    {
        if (dialogueText == null) yield break;

        dialogueText.text = "";
        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    // 等待打字完成后，再延迟指定时间自动隐藏
    private IEnumerator AutoHideAfterTyping(float delay)
    {
        // 等待当前打字协程结束
        if (typingCoroutine != null) yield return typingCoroutine;
        yield return new WaitForSecondsRealtime(delay);
        gameObject.SetActive(false);
    }
}