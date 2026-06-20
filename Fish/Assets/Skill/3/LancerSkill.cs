using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanserSkill : MonoBehaviour
{
    public static LanserSkill Instance { get; private set; }

    [Header("基础属性")]
    [SerializeField] private float baseCooldown = 12f;
    [SerializeField] private float baseWidth = 0.8f;            // BoxCollider 高度（激光粗细）
    [SerializeField] private float baseDuration = 4f;         // 持续时间
    [SerializeField] private GameObject laserEffectPrefab;    // 激光特效预制体

    // 可升级属性
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentWidth;
    [HideInInspector] public float currentDuration;

    private SkillButton boundButton;
    private bool isActive = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentCooldown = baseCooldown;
        currentWidth = baseWidth;
        currentDuration = baseDuration;
    }

    public void BindButton(SkillButton btn) => boundButton = btn;

    public void Use()
    {
        if (isActive || boundButton == null) return;
        isActive = true;
        boundButton.StartCooldown();   // 立即冷却
        StartCoroutine(LaserRoutine());
    }

    IEnumerator LaserRoutine()
    {
        float[] xPositions = { 0.25f, 0.5f, 0.75f };
        float randomX = xPositions[Random.Range(0, xPositions.Length)];
        Vector3 bottomCenter = Camera.main.ViewportToWorldPoint(new Vector3(randomX, 0f, 0));
        bottomCenter.z = 0;
        // 向下偏移（例如 1.5 单位，确保完全在屏幕外）
        float offsetBelow = 8f;
        Vector3 spawnPos = bottomCenter - new Vector3(0, offsetBelow, 0);
        GameObject laserObj = Instantiate(laserEffectPrefab, spawnPos, Quaternion.identity);

        // 实例化激光特效
        if (laserEffectPrefab != null)
        {
            //GameObject laserObj = Instantiate(laserEffectPrefab, spawnPos, Quaternion.identity);
            // 确保有 Rigidbody2D 和 Collider
            Rigidbody2D rb = laserObj.GetComponent<Rigidbody2D>();
            if (rb == null) rb = laserObj.AddComponent<Rigidbody2D>();
            rb.isKinematic = true;

            BoxCollider2D col = laserObj.GetComponent<BoxCollider2D>();
            if (col == null) col = laserObj.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(currentWidth, 10f);   // 激光长度10，粗细为当前宽度

            // 添加碰撞处理脚本
            LaserCollider lc = laserObj.AddComponent<LaserCollider>();
            lc.Init(currentDuration);   // 传入持续时间，用于命中记录清理（可选）
        }

        yield return new WaitForSeconds(currentDuration);

        if (laserObj != null) Destroy(laserObj);
       
        isActive = false;
    }

    // ----- 升级方法 -----
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        boundButton?.SetCooldownDuration(currentCooldown);
    }

    public void IncreaseWidth(float extra)
    {
        currentWidth += extra;
    }

    public void IncreaseDuration(float extraSeconds)
    {
        currentDuration += extraSeconds;
    }
}