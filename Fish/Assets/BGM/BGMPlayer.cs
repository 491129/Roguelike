using System.Collections;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    private static BGMPlayer instance;

    [Header("音乐资源")]
    [SerializeField] private AudioClip normalBGM;
    [SerializeField] private AudioClip bossBGM;

    [Header("淡入淡出设置")]
    [SerializeField] private float fadeDuration = 1.5f;   // 渐变时长（秒）
    [SerializeField] private float maxVolume = 1f;       // 正常音量

    private AudioSource audioSource;
    private Coroutine fadeRoutine;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.volume = maxVolume;

        // 默认播放常规音乐
        PlayMusic(normalBGM);
    }

    /// <summary>
    /// 播放常规音乐（带淡入）
    /// </summary>
    public static void PlayNormal()
    {
        if (instance != null)
            instance.PlayMusic(instance.normalBGM);
    }

    /// <summary>
    /// 播放 Boss 战斗音乐（带淡入）
    /// </summary>
    public static void PlayBoss()
    {
        if (instance != null)
            instance.PlayMusic(instance.bossBGM);
    }

    /// <summary>
    /// 切换到目标音乐，执行淡出旧曲 -> 淡入新曲
    /// </summary>
    private void PlayMusic(AudioClip newClip)
    {
        if (audioSource.clip == newClip && audioSource.isPlaying) return;

        // 停止之前的切换协程（如果有）
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CrossfadeMusic(newClip));
    }

    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        // ---- 1. 淡出 ----
        if (audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
                yield return null;
            }
            audioSource.volume = 0f;
        }

        // ---- 2. 切换曲子 ----
        audioSource.clip = newClip;
        audioSource.Play();

        // ---- 3. 淡入 ----
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(0f, maxVolume, t / fadeDuration);
            yield return null;
        }
        audioSource.volume = maxVolume;
    }
}