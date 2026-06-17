using UnityEngine;

public class FishnetManager : MonoBehaviour
{
    public static FishnetManager Instance { get; private set; }

    [Header("渔网基础半径（对应炮台0~4级）")]
    [SerializeField] private float[] baseRadii = { 2f, 2.8f, 3.6f, 4.4f, 5f };

    [Header("广域渔网加成倍率")]
    [SerializeField] private float rangeMultiplier = 1.3f;   // 与图腾一致

    private int currentLevel = 0;       // 当前渔网等级（0~4）
    private bool hasRangeBonus = false; // 是否购买了广域渔网

    void Awake() => Instance = this;

    /// <summary>
    /// 获取当前渔网碰撞体半径
    /// </summary>
    public float GetCurrentRadius()
    {
        int level = Mathf.Clamp(currentLevel, 0, baseRadii.Length - 1);
        float radius = baseRadii[level];
        if (hasRangeBonus) radius *= rangeMultiplier;
        return radius;
    }

    /// <summary>
    /// 升级渔网等级（广域渔网图腾调用）
    /// </summary>
    public void UpgradeRange()
    {
        hasRangeBonus = true;
        Debug.Log($"渔网范围已升级，当前半径：{GetCurrentRadius()}");
    }

    /// <summary>
    /// 设置渔网基础等级（由炮台升级调用？或者独立？）
    /// </summary>
    public void SetLevel(int level)
    {
        currentLevel = level;
    }
}