using System.Collections;
using UnityEngine;

public class WaveStagger : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] waveParticles;  // 按顺序拖入你的海浪粒子
    [SerializeField] private float staggerDelay = 0.8f;       // 每个浪之间的间隔

    IEnumerator Start()
    {
        for (int i = 0; i < waveParticles.Length; i++)
        {
            if (waveParticles[i] != null)
            {
                waveParticles[i].gameObject.SetActive(true);
                waveParticles[i].Play();
            }
            yield return new WaitForSeconds(staggerDelay);
        }
    }
}