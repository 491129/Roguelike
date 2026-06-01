using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [Header("UI 组件")]
    [SerializeField] private Image iconImage;      // 显示技能图标的 Image
    [SerializeField] private Button button;        // 按钮组件
    [SerializeField] private Sprite defaultGray;   // 未激活/冷却时的灰色图

    private Sprite activeSprite;                   // 激活后的技能图
    private bool isActive = false;                 // 是否已激活（购买后）
    private bool isCooldown = false;               // 是否在冷却中

    public System.Action OnSkillUsed;

    private void Start()
    {
        // 初始状态：灰色，不可交互
        button.interactable = false;
        iconImage.sprite = defaultGray;
    }

    public void Activate(Sprite skillSprite)
    {
        activeSprite = skillSprite;
        iconImage.sprite = activeSprite;
        isActive = true;
        button.interactable = true;
    }
    public void OnButtonClick()
    {
        if (!isActive || isCooldown) return;

        // 执行技能效果
        OnSkillUsed?.Invoke();

        // 开始冷却
        StartCoroutine(CooldownRoutine());
    }

    IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        button.interactable = false;
        iconImage.sprite = defaultGray;   // 变灰

        yield return new WaitForSeconds(8f);

        isCooldown = false;
        if (isActive)   // 如果技能没有被移除，恢复可点击状态
        {
            button.interactable = true;
            iconImage.sprite = activeSprite;
        }
    }
}