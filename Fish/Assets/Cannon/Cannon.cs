using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class Cannon : MonoBehaviour
{
    [Header("炮台设置")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.8f;

    [Header("Debuff")]
    public float attackSpeedMultiplier = 1f;  
    private float baseFireRate;

    private float nextFireTime;
    private Camera mainCam;
    private ObjectPool pool;
    private float currentAngle;
   // public Text NoCoin;
    public int actualCost;

    public static Cannon Instance;
    private void Awake() => Instance = this;

    private int shotCount = 0;            // 开火计数
    private const int chargeInterval = 6; // 每6炮触发蓄力

    public GameObject NoCoin;
    void OnEnable()
    {
        mainCam = Camera.main;
        pool = ObjectPool.Instance;   // 对象池也建议重取，避免意外
    }
    void Start()
    {
        baseFireRate = fireRate;
        currentAngle = transform.eulerAngles.z;
        // 原本的 mainCam 和 pool 初始化可移入 OnEnable，也可保留一份在 Start 作为备用
        if (mainCam == null) mainCam = Camera.main;
        if (pool == null) pool = ObjectPool.Instance;
    }

    void Update()
    {
        if (LockSkill.Instance != null && LockSkill.Instance.IsLockModeActive && LockSkill.Instance.LockedTarget == null)
        {
            return;
        }

        AimAtMouse();
        bool lockedButNoTarget = LockSkill.Instance != null
                             && LockSkill.Instance.IsLockModeActive
                             && LockSkill.Instance.LockedTarget == null;
        if (Input.GetMouseButton(0) && !lockedButNoTarget && Time.time >= nextFireTime
        && !EventSystem.current.IsPointerOverGameObject())
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }

    void AimAtMouse()
    {
        // 直接使用 Camera.main，不依赖任何缓存，并保护空引用
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError($"[{gameObject.name}] Camera.main 不存在！炮台无法旋转");
            return;
        }

        Vector3 mouseViewport = cam.ScreenToViewportPoint(Input.mousePosition);
        float mouseXPercent = Mathf.Clamp01(mouseViewport.x);

        // 调试日志：每2秒输出一次，避免刷屏
        if (Time.frameCount % 120 == 0)
            Debug.Log($"[{gameObject.name}] mouseX={mouseXPercent:F2}, angle={Mathf.Lerp(80f, -80f, mouseXPercent):F1}");

        float angle = Mathf.Lerp(80f, -80f, mouseXPercent);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
        int originalCost = ALLCannon.levelCosts[ALLCannon.currentLevel];
        float costMultiplier = TotemManager.Instance != null ? TotemManager.Instance.CostMultiplier : 1f;

        actualCost = Mathf.RoundToInt(originalCost * costMultiplier);
        if (!GameManager.SpendCoin(actualCost))
        {
            NoCoin.SetActive(true);
            return;
        }
        else
        {
            NoCoin.SetActive (false);
        }
        if ( TotemManager.Instance.xuli)
        {
            shotCount++;
            if (shotCount >= chargeInterval)
            {
                FishAttrbute.CatchRateMultiplier *= 1.2f;//111111111111111111111111111
                shotCount = 0;   // 重置计数
                // 生成蓄力子弹后
                TotemManager.Instance?.TriggerEffectByName("蓄力");
            }
        }
        if(TotemManager.Instance.kuaisu)
        {
            TotemManager.Instance?.TriggerEffectByName("快速装填");
        }
        if (TotemManager.Instance.qianghua)
        {
            TotemManager.Instance?.TriggerEffectByName("强化炮管");
        }
        if (FishnetManager.Instance.hasRangeBonus)
        {
            TotemManager.Instance?.TriggerEffectByName("广域渔网");
        }
        if (TotemManager.Instance.shengqian)
        {
            TotemManager.Instance?.TriggerEffectByName("省钱达人");
        }
            GameObject bullet = pool.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);
            // 播放开火音效
            AudioManager.PlayShoot();
        }

    }
    public void ApplyDebuff(string bossType)
    {
        switch (bossType)
        {
            case "Freeze":
                attackSpeedMultiplier *= 0.85f;
                Debug.Log("冰冻中" + attackSpeedMultiplier);
                break;
                // 其他类型如攻速不影响，但可保留扩展
        }
        fireRate = baseFireRate / attackSpeedMultiplier;
    }

    public void RemoveDebuff(string bossType)
    {
        switch (bossType)
        {
            case "Freeze":
                attackSpeedMultiplier /= 0.85f;
                break;
        }
        fireRate = baseFireRate / attackSpeedMultiplier;
    }
    public void ApplyDebuffCAnnon()
    {
        attackSpeedMultiplier *= 1.15f;
        fireRate = baseFireRate / attackSpeedMultiplier;
    }
    public int GetActualCost()
    {
        int baseCost = ALLCannon.levelCosts[ALLCannon.currentLevel];
        float multiplier = TotemManager.Instance != null ? TotemManager.Instance.CostMultiplier : 1f;
        return Mathf.RoundToInt(baseCost * multiplier);
    }
    public void ApplyTempSpeedDebuff(float multiplier, float duration)
    {
        attackSpeedMultiplier *= multiplier;
        StartCoroutine(RemoveTempSpeedDebuff(multiplier, duration));
    }
    IEnumerator RemoveTempSpeedDebuff(float multiplier, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        attackSpeedMultiplier /= multiplier;
    }
}