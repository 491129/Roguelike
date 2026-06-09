using UnityEngine;

public class SkillButtonManager : MonoBehaviour
{
    public static SkillButtonManager Instance { get; private set; }

    [Header("技能按钮（按顺序拖入，共4个）")]
    [SerializeField] private SkillButton[] skillButtons;   // 必须拖入4个，但第4个初始隐藏

    private int nextActivateIndex = 0;                     // 下一个待激活的索引
    private int maxActiveSlots = 3;                        // 当前最大激活槽位数（初始3）

    public bool AllActivated => nextActivateIndex >= maxActiveSlots;
    public bool CanPurchase => !AllActivated;
    public bool CanExpandSlot => maxActiveSlots < 4;       // 是否还能扩容

    private void Awake()
    {
        Instance = this;
        if (skillButtons.Length < 4)
        {
            Debug.LogError("SkillButtonManager 需要至少4个按钮！");
            return;
        }
        // 初始隐藏第4个按钮
        skillButtons[3].gameObject.SetActive(false);
    }

    /// <summary>
    /// 激活下一个技能按钮（购买普通技能时调用）
    /// </summary>
    public bool ActivateNextSkill(Sprite skillSprite, System.Action skillEffect)
    {
        if (AllActivated)
        {
            Debug.Log("所有技能槽已满");
            return false;
        }

        SkillButton btn = skillButtons[nextActivateIndex];
        btn.Activate(skillSprite);
        btn.OnSkillUsed = skillEffect;
        nextActivateIndex++;
        return true;
    }

    /// <summary>
    /// 扩展一个槽位（购买“换条大船”时调用）
    /// </summary>
    public bool ExpandSlot()
    {
        if (!CanExpandSlot)
        {
            Debug.Log("已经达到最大容量");
            return false;
        }

        maxActiveSlots++;
        // 显示第4个按钮（索引3）
        if (maxActiveSlots == 4)
        {
            skillButtons[3].gameObject.SetActive(true);
        }
        return true;
    }

}