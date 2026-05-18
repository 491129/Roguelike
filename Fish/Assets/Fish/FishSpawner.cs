using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [Header("配置")]
    [SerializeField] private FishData[] fishDatas;    // 所有鱼的数据（目前3个）
    [SerializeField] private GameObject fishPrefab;   // 通用鱼预制体
    [SerializeField] private int poolSize = 15;       // 对象池大小
    [SerializeField] private int maxOnScreen = 10;    // 最大同屏数量
    [SerializeField] private float minSpawnInterval = 1.5f;
    [SerializeField] private float maxSpawnInterval = 3f;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private int currentCount;

    void Start()
    {
        // 初始化对象池
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(fishPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        StartCoroutine(SpawnRoutine());
    }

    System.Collections.IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
            if (currentCount < maxOnScreen && fishDatas.Length > 0)
                SpawnRandomFish();
        }
    }

    void SpawnRandomFish()
    {
        // 随机选一种鱼数据
        FishData data = fishDatas[Random.Range(0, fishDatas.Length)];

        GameObject obj = GetPooledObject();
        if (obj == null) return;

        // 屏幕内随机位置生成
        obj.transform.position = GetRandomScreenPosition();
        obj.transform.rotation = Quaternion.identity;

        Fish fish = obj.GetComponent<Fish>();
        if (fish != null)
        {
            Vector2 dir = Random.insideUnitCircle.normalized; // 完全随机方向
            fish.Init(data, dir);
        }

        obj.SetActive(true);
        currentCount++;
    }

    Vector3 GetRandomScreenPosition()
    {
        Camera cam = Camera.main;
        float height = 2f * cam.orthographicSize;
        float width = height * cam.aspect;

        // 在屏幕内留出边距，避免鱼直接贴边
        float margin = 0.5f;
        float x = Random.Range(-width / 2f + margin, width / 2f - margin);
        float y = Random.Range(-height / 2f + margin, height / 2f - margin);
        return cam.transform.position + new Vector3(x, y, 0);
    }

    GameObject GetPooledObject()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        else
        {
            // 动态扩展
            GameObject obj = Instantiate(fishPrefab);
            obj.SetActive(false);
            return obj;
        }
    }

    public void ReturnFish(GameObject fish)
    {
        fish.SetActive(false);
        pool.Enqueue(fish);
        currentCount--;
    }
}