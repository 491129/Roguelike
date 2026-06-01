using UnityEngine;

public class SkillButtonManager : MonoBehaviour
{
    public static SkillButtonManager Instance { get; private set; }

    [Header("技能按钮（按顺序拖入）")]
    [SerializeField] private SkillButton[] skillButtons;   // 长度为 4

    private int nextActivateIndex = 0;                     // 下一个待激活的索引
    public bool AllActivated => nextActivateIndex >= skillButtons.Length;

    private void Awake() => Instance = this;

    /// <summary>
    /// 激活下一个技能按钮（由商店调用）
    /// </summary>
    public bool ActivateNextSkill(Sprite skillSprite, System.Action skillEffect)
    {
        if (AllActivated)
        {
            Debug.Log("所有技能已激活");
            return false;
        }

        SkillButton btn = skillButtons[nextActivateIndex];
        btn.Activate(skillSprite);
        btn.OnSkillUsed = skillEffect;   // 绑定技能效果

        nextActivateIndex++;
        return true;
    }

    /// <summary>
    /// 供商店检查是否可以购买技能
    /// </summary>
    public bool CanPurchase => !AllActivated;
}