using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "捕鱼/FishData")]
public class FishData : ScriptableObject
{
    [Header("基本信息")]
    public string fishName;                     // 名字
    public int maxHealth = 1;                   // 生命值
    public int price = 5;                       // 击杀获得金币
    public Sprite sprite;                       // 外观图片
    public RuntimeAnimatorController animator;  // 可选动画控制器

    [Header("移动")]
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.2f;
}