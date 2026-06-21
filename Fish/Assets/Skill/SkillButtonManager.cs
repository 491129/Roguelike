using UnityEngine;

public class SkillButtonManager : MonoBehaviour
{
    public static SkillButtonManager Instance { get; private set; }

    [Header("技能按钮（按顺序拖入，共4个）")]
    public SkillButton[] skillButtons;   // 必须拖入4个，但第4个初始隐藏

    private int nextActivateIndex = 0;                     // 下一个待激活的索引
    public int maxActiveSlots = 3;                        // 当前最大激活槽位数（初始3）

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
    public bool ActivateNextSkill(Sprite skillSprite, System.Action skillEffect, string skillID)
    {
        if (AllActivated)
        {
            Debug.Log("所有技能槽已满");
            return false;
        }

        SkillButton btn = skillButtons[nextActivateIndex];
        btn.Activate(skillSprite);
        btn.OnSkillUsed = skillEffect;
        btn.skillID = skillID;
        nextActivateIndex++;
        return true;
    }
    public SkillButton GetLastActivatedButton()
    {
        if (nextActivateIndex > 0 && nextActivateIndex <= skillButtons.Length)
            return skillButtons[nextActivateIndex - 1];
        return null;
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
    public System.Action<SkillButton> OnSkillRemoved;

    public void RemoveSkill(SkillButton btn)
    {
        if (!btn.IsActive) return;

        // 找到该按钮在数组中的索引
        int removeIndex = -1;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillButtons[i] == btn)
            {
                removeIndex = i;
                break;
            }
        }
        if (removeIndex < 0) return;

        // 清除按钮状态
        btn.ClearSkill();

        // 通知外部，传递按钮以便获取主技能ID
        OnSkillRemoved?.Invoke(btn);

        // 从移除位置开始，将后面的激活按钮向前移动
        for (int i = removeIndex; i < skillButtons.Length - 1; i++)
        {
            // 如果下一个按钮是激活的，则把它的数据复制到当前按钮
            if (skillButtons[i + 1].IsActive)
            {
                // 复制精灵和回调
                skillButtons[i].Activate(skillButtons[i + 1].activeSprite);
                skillButtons[i].OnSkillUsed = skillButtons[i + 1].OnSkillUsed;
                skillButtons[i + 1].ClearSkill();
            }
            else
            {
                // 后面的未激活，直接清除当前按钮即可
                skillButtons[i].ClearSkill();
            }
        }

        // 更新 nextActivateIndex
        int activeCount = 0;
        for (int i = 0; i < skillButtons.Length; i++)
        {
            if (skillButtons[i].IsActive) activeCount++;
        }
        nextActivateIndex = activeCount;

        // 隐藏超出容量的按钮（如果第4个按钮之前被激活但现在不需要了）
        if (maxActiveSlots == 4 && activeCount < 4 && skillButtons[3].gameObject.activeSelf)
        {
            // 这里不立即隐藏，因为扩容是永久的，但槽位可以留空。如果希望严格对应容量，可以保留显示。
        }
    }

}