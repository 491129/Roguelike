using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CoolSkill : MonoBehaviour
{
    public static CoolSkill Instance { get; private set; }
    static public int Skillcoin = 20;
    public static bool isUsed = false;
    public static float duration=3f;
    private SkillButton DJButton;   // 绑定的技能按钮
    [HideInInspector] public float currentCooldown;
    [Header("特效")]
    [SerializeField] private GameObject freezeEffect;   // 拖入冰冻持续特效预制体或场景中的物体
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 初始隐藏特效（如果它一开始在场景中是激活的）
        if (freezeEffect != null)
            freezeEffect.SetActive(false);
    }
    void Start()
    {
        SkillManager.iscool = true;
       
    }
    private void Update()
    {
        
    }
    public void BindButton(SkillButton button)
    {
        DJButton = button;
    }
    public void FreezeAllFish()
    {
        SkillShopManager.Instance.StartCoroutine(FreezeCoroutine(duration));
        //DJButton.StartCooldown();
    }
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        DJButton?.SetCooldownDuration(currentCooldown);
        Debug.Log($"冰冻冷却减少 {seconds} 秒，当前冷却：{currentCooldown}");
    }

    private IEnumerator FreezeCoroutine(float duration)
    {
        Debug.Log("111");
        // 开启特效
        if (freezeEffect != null)
            freezeEffect.SetActive(true);
        Debug.Log("222");
        Fishself.IsFrozen = true;    // 冻结所有鱼
        yield return new WaitForSecondsRealtime(duration);
        Fishself.IsFrozen = false;   // 恢复
        Debug.Log("333");
        // 关闭特效
        if (freezeEffect != null)
            freezeEffect.SetActive(false);
    }
   
}
