using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
    public Text NoCoin;
    public int actualCost;

    public static Cannon Instance;
    private void Awake() => Instance = this;

    private int shotCount = 0;            // 开火计数
    private const int chargeInterval = 6; // 每6炮触发蓄力
    void Start()
    {
        baseFireRate = fireRate;
        mainCam = Camera.main;
        pool = ObjectPool.Instance;
        currentAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        AimAtMouse();
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime
            && !EventSystem.current.IsPointerOverGameObject())
        {
            Fire();
            nextFireTime = Time.time + fireRate;
            Debug.Log("fireRate:"+fireRate+ "attackSpeedMultiplier" + attackSpeedMultiplier);
        }

    }

    void AimAtMouse()
    {
        float mouseXPercent = Mathf.Clamp01(mainCam.ScreenToViewportPoint(Input.mousePosition).x);
        float angle = Mathf.Lerp(80f, -80f, mouseXPercent);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Fire()
    {
        int originalCost = ALLCannon.levelCosts[ALLCannon.currentLevel];
        float costMultiplier = TotemManager.Instance != null ? TotemManager.Instance.CostMultiplier : 1f;
        actualCost = Mathf.RoundToInt(originalCost * costMultiplier);
        //int cost = ALLCannon.levelCosts[ALLCannon.currentLevel];
        if (!GameManager.SpendCoin(actualCost))
        {
            NoCoin.text = "金币不足!无法发射!";
            return;
        }
        else
        {
            NoCoin.text = " ";
        }
        if ( TotemManager.Instance.xuli)
        {
            shotCount++;
            if (shotCount >= chargeInterval)
            {
                //FishAttrbute.escapeChance *= 0.8f;//111111111111111111111111111
                shotCount = 0;   // 重置计数
            }
        }
        GameObject bullet = pool.GetBullet();
        if (bullet != null)
        {
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
        }

    }
    public void ApplyDebuff(string bossType)
    {
        switch (bossType)
        {
            case "Freeze":
                attackSpeedMultiplier *= 0.6f;
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
                attackSpeedMultiplier /= 0.6f;
                break;
        }
        fireRate = baseFireRate / attackSpeedMultiplier;
    }
    public void ApplyDebuffCAnnon()
    {
        attackSpeedMultiplier *= 1.2f;
        fireRate = baseFireRate / attackSpeedMultiplier;
    }
    public int GetActualCost()
    {
        int baseCost = ALLCannon.levelCosts[ALLCannon.currentLevel];
        float multiplier = TotemManager.Instance != null ? TotemManager.Instance.CostMultiplier : 1f;
        return Mathf.RoundToInt(baseCost * multiplier);
    }
}