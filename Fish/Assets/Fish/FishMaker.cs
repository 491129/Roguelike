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
        
        if(moveType==0)
        {
            angOffset = Random.Range(-22, 22);
           
        }
      
    }
}
