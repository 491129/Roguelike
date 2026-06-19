using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [Header("UI 组件")]
    public Image iconImage;      // 显示技能图标的 Image
    [SerializeField] private Button button;        // 按钮组件
    public Sprite defaultGray;   // 未激活/冷却时的灰色图

    public Sprite activeSprite;                   // 激活后的技能图
    public Sprite ActiveSprite => activeSprite;
    private bool isActive = false;                 // 是否已激活（购买后）
  
    public System.Action OnSkillUsed;
    //public static float duration = 8f;
    //public static float changeDuration=1f;  //改变冷却时间
    [Header("禁用")]
    private bool isTemporarilyDisabled = false;  // 是否被 Boss 临时禁用
    private Sprite originalActiveSprite;         // 保存原始技能图标
    public bool IsActive => isActive;
    private bool isCooldown = false;               // 是否在冷却中
    private float cooldownTime = 8f;   // 冷却时间
    public static float changeDuration = 1f;  //改变冷却时间

    private void Start()
    {
        // 自动绑定按钮点击事件，避免手动遗漏
        button.onClick.AddListener(OnButtonClick);
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
    public void Activate(Sprite skillSprite, float customCooldown = -1f)
    {
        activeSprite = skillSprite;
        iconImage.sprite = activeSprite;
        isActive = true;
        button.interactable = true;
        if (customCooldown > 0) cooldownTime = customCooldown;
    }

    public void OnButtonClick()
    {
        if (!isActive || isCooldown) return;

        // 执行技能效果
        OnSkillUsed?.Invoke();

        // 开始冷却
        //StartCooldown();
    }
    public void StartCooldown()
    {
        if (isCooldown) return;
        StartCoroutine(CooldownRoutine());
        if (TotemManager.Instance.chaopin)
        {
            TotemManager.Instance?.TriggerEffectByName("超频核心");
        }
    }
    IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        button.interactable = false;
        iconImage.sprite = defaultGray;
        yield return new WaitForSeconds(cooldownTime*changeDuration);
        isCooldown = false;
        if (isActive)
        {
            button.interactable = true;
            iconImage.sprite = activeSprite;
        }
    }

    public void DisableByBoss()
    {
        if (!isActive || isTemporarilyDisabled) return;

        isTemporarilyDisabled = true;
        originalActiveSprite = activeSprite;    // 保存原图标
        button.interactable = false;
        iconImage.sprite = defaultGray;
    }
    public void RestoreByBoss()
    {
        if (!isTemporarilyDisabled) return;
        isTemporarilyDisabled = false;

        if (isActive)
        {
            button.interactable = true;
            iconImage.sprite = originalActiveSprite;   // 恢复原图标
        }
    }
    public void SetCooldownDuration(float newDuration)
    {
        cooldownTime = newDuration;
    }
}