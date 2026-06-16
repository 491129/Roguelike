using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class fish : MonoBehaviour
{
    public static fish _instance;
    public static fish instance => _instance;

    public GameObject[] fishPres;      // 12个预制体，索引0~11对应鱼1~12
    public Transform[] fishMakers;
    public Transform fishParent;

    private void Awake() => _instance = this;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            var (count, interval) = GameProgressManager.Instance.GetCurrentWaveParams();
            yield return StartCoroutine(SpawnWave(count));
            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator SpawnWave(int totalCount)
    {
        for (int i = 0; i < totalCount; i++)
        {
            SpawnRandomFish();
            yield return new WaitForSeconds(0.2f);   // 每条鱼之间的生成间隔（可调）
        }
    }

    void SpawnRandomFish()
    {
        // 权重随机选择鱼种
        int fishIndex = GetWeightedRandomFishIndex();
        int makerIndex = Random.Range(0, fishMakers.Length);
        float angle = Random.Range(-45, 45);

        GameObject fishPrefab = fishPres[fishIndex];
        int fishNum = fishPrefab.GetComponent<FishAttrbute>().fishNumber;  // 一批生成几条（原有逻辑保留）
        StartCoroutine(MakeOneFish(fishIndex, makerIndex, fishNum, angle));
    }

    int GetWeightedRandomFishIndex()
    {
        int totalWeight = 0;
        foreach (int w in FishDataConfig.SpawnWeights) totalWeight += w;
        int rand = Random.Range(0, totalWeight);
        int cumulative = 0;
        for (int i = 0; i < FishDataConfig.SpawnWeights.Length; i++)
        {
            cumulative += FishDataConfig.SpawnWeights[i];
            if (rand < cumulative) return i;
        }
        return 0;
    }

    IEnumerator MakeOneFish(int fishPre, int fishPoint, int fishNum, float angle)
    {
        for (int i = 0; i < fishNum; i++)
        {
            GameObject fishObj = Instantiate(fishPres[fishPre]);
            fishObj.transform.position = fishMakers[fishPoint].position;
            fishObj.transform.SetParent(fishParent, false);
            fishObj.GetComponent<SpriteRenderer>().sortingOrder += i;
            fishObj.transform.rotation = Quaternion.Euler(0, 0, fishMakers[fishPoint].eulerAngles.z + angle);

            float eulerZ = fishObj.transform.eulerAngles.z;
            float normalizedZ = eulerZ > 180 ? eulerZ - 360 : eulerZ;
            fishObj.GetComponent<SpriteRenderer>().flipY = !(normalizedZ > -90 && normalizedZ < 90);

            yield return new WaitForSeconds(0.7f);
        }
    }
}