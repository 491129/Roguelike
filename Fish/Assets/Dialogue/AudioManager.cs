using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("音效资源")]
    [SerializeField] private AudioClip shootClip;    // 开火音效
    [SerializeField] private AudioClip coinClip;     // 金币音效

    private AudioSource audioSource;
    [Header("音量设置")]
    [SerializeField][Range(0f, 1f)] private float sfxVolume = 1f;   // 音效音量，默认最大
    private const string SFX_VOLUME_KEY = "SFXVolume";
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);   // 跨场景保留

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    }
    public static void SetSFXVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.sfxVolume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, Instance.sfxVolume);
        }
    }
    public static float GetSFXVolume()
    {
        return Instance != null ? Instance.sfxVolume : 1f;
    }

    /// <summary>
    /// 播放开火音效（支持叠加）
    /// </summary>
    public static void PlayShoot()
    {
        if (Instance != null && Instance.shootClip != null)
            Instance.audioSource.PlayOneShot(Instance.shootClip);
    }

    /// <summary>
    /// 播放金币音效（支持叠加）
    /// </summary>
    public static void PlayCoin()
    {
        if (Instance != null && Instance.coinClip != null)
            Instance.audioSource.PlayOneShot(Instance.coinClip);
    }
}