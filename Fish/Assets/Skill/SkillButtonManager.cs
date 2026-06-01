using UnityEngine;

public class SkillButtonManager : MonoBehaviour
{
    public static SkillButtonManager Instance { get; private set; }

    [Header("技能按钮（按顺序拖入）")]
    [SerializeField] private SkillButton[] skillButtons;   // 长度为 4

    private int nextActivateIndex = 0;                     // 下一个待激活的索引
    public bool AllActivated => nextActivateIndex >= skillButtons.Length;

    private void Awake() => Instance = this;

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

    public bool CanPurchase => !AllActivated;
}