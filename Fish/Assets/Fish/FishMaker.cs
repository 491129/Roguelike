using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMaker : MonoBehaviour
{
    public Transform fishHolder;
    public Transform[] genPosition;
    public GameObject[] fishPrefabs;

    void Start()
    {
        
    }

    // Update is called once per frame
    void MakeFishes()
    {
        int genPosIndex=Random.Range(0, genPosition.Length);
        int fishPreIndex=Random.Range(0, fishPrefabs.Length);   
        int maxNum = fishPrefabs[fishPreIndex].GetComponent<FishAttr>().maxNum;
        int maxSpeed = fishPrefabs[fishPreIndex].GetComponent<FishAttr>().maxSpeed;
        int num = Random.Range((maxNum / 2) + 1, maxNum);
        int speed = Random.Range(maxSpeed / 2, maxSpeed);
        int moveType = Random.Range(0, 2);
        int angOffset;
        int angSppeed;
        
        if(moveType==0)
        {
            angOffset = Random.Range(-22, 22);
           // StartCoroutine(GenStraightFish(genPosIndex, fishPreIndex,num,speed,angOffset));
        }
        else
        {

        }
    }

    IEnumerator GenStraightFish(int genPosIndex, int fishPreIndex, int num, int speed, int angOffset)
    {
        //for (int i = 0; i < num; i++)
        //{
        //    GameObject fish = Instantiate(fishPrefabs[fishPreIndex]);
        //    fish.transform.SetParent(fishHolder, false);
        //    fish.transform.localPosition = genPositions[genPosIndex].localPosition;
        //    fish.transform.Rotate(0, 0, angOffset);
        //    fish.GetComponent<SpriteRenderer>(). sortingOrder += i;
        //    fish.AddComponent<Ef_AutoMlove>().speed = speed;
        yield return 0; //new WaitForSeconds(fishGenWaitTime);
        //}
    }

}
