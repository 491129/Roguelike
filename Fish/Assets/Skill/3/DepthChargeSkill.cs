using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
//using UnityEditor.Overlays;
using UnityEngine;

public class DepthChargeSkill : MonoBehaviour
{
    public static DepthChargeSkill Instance { get; private set; }

    [Header("基础属性")]
    [SerializeField] private float baseCooldown = 10f;
    [SerializeField] private float baseRadius = 3f;
    [SerializeField] private GameObject explosionEffectPrefab;   // 爆炸特效预制体
    [SerializeField] private GameObject dotZonePrefab;            // 持续伤害区域预制体

    // 可升级属性
    [HideInInspector] public float currentCooldown;
    [HideInInspector] public float currentRadius;
    [HideInInspector] public int currentExplosionCount = 1;
    [HideInInspector] public bool hasDotZone = false;
    [HideInInspector] public float dotDuration = 5f;

    private SkillButton boundButton;   // 绑定的技能按钮

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        currentCooldown = baseCooldown;
        currentRadius = baseRadius;
    }

    public void BindButton(SkillButton btn) => boundButton = btn;

    /// <summary>
    /// 按钮点击时调用
    /// </summary>
    public void Use()
    {
        if (boundButton == null){ Debug.Log("NULL深水炸弹"); return; }
        StartCoroutine(BombSequence());
    }

    IEnumerator BombSequence()
    {
        for (int i = 0; i < currentExplosionCount; i++)
        {
            // 在屏幕中心附近随机位置生成爆炸
            Vector3 bombPos = GetRandomNearCenterPos();
            SpawnExplosion(bombPos);
            if (i < currentExplosionCount - 1)
                yield return new WaitForSeconds(0.3f); // 多次爆炸间隔
        }

        // 持续伤害区域
        if (hasDotZone && dotZonePrefab != null)
        {
            Vector3 zonePos = GetRandomNearCenterPos();
            GameObject zone = Instantiate(dotZonePrefab, zonePos, Quaternion.identity);
            DotZone dot = zone.GetComponent<DotZone>();
            if (dot != null) dot.Init(dotDuration, currentRadius);
            else Destroy(zone, dotDuration);
        }

        // 按钮开始冷却
        boundButton.StartCooldown();
    }

    void SpawnExplosion(Vector3 position)
    {
        if (explosionEffectPrefab == null) return;

        GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
        // 调整特效子物体碰撞体半径
        Transform bulletChild = effect.transform.Find("Bullet"); // 子物体名字必须为 "Bullet"
        if (bulletChild != null)
        {
            CircleCollider2D col = bulletChild.GetComponent<CircleCollider2D>();
            if (col != null) col.radius = currentRadius;
            // 子物体上的脚本（如有）不需要额外处理，碰撞会直接触发 OnTriggerEnter2D
        }
        Destroy(effect, 2f); // 特效播放后销毁
    }

    Vector3 GetRandomNearCenterPos()
    {
        Vector3 center = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
        center.z = 0;
        Vector2 offset = Random.insideUnitCircle * 4f; // 半径4范围内
        return center + new Vector3(offset.x, offset.y, 0);
    }

    // ----- 升级方法 -----
    public void ReduceCooldown(float seconds)
    {
        currentCooldown = Mathf.Max(1f, currentCooldown - seconds);
        boundButton?.SetCooldownDuration(currentCooldown);
    }

    public void IncreaseRadius(float multiplier)
    {
        currentRadius *= multiplier;
    }

    public void IncreaseExplosionCount()
    {
        if (currentExplosionCount < 4)
            currentExplosionCount++;
    }

    public void EnableDotZone()
    {
        hasDotZone = true;
    }
}