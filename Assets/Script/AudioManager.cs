
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip decisionSE;
    [SerializeField] private AudioClip decisionSE2;
    [SerializeField] private AudioClip cancelSE;
    [SerializeField] private AudioClip windSE;
    [SerializeField] private AudioClip switchSE;
    [SerializeField] private AudioClip clearSE;


    [Header("音量")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private AudioSource windSource;
    [SerializeField] private float windStartHeight = 10f;
    [SerializeField] private float windFullVolumeHeight = 20f;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlayBGM();
        // 保存されている音量を読み込む
        float bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        float seVolume = PlayerPrefs.GetFloat("SEVolume", 1f);

        audioMixer.SetFloat(
            "BGMVolume",
            Mathf.Log10(Mathf.Max(bgmVolume, 0.0001f)) * 20f
        );

        audioMixer.SetFloat(
             "SEVolume",
             Mathf.Log10(Mathf.Max(seVolume, 0.0001f)) * 20f
         );
    }
    public void PlayBGM()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlayDecisionSE()
    {
        PlaySE(decisionSE);
    }

    public void PlayDecisionSE2()
    {
        PlaySE(decisionSE2);
    }

    public void PlayCancelSE()
    {
        PlaySE(cancelSE);
    }

    public void PlaySwitchSE()
    {
        PlaySE(switchSE);
    }

    public void PlayClearSE()
    {
        PlaySE(clearSE);
    }

    private void PlaySE(AudioClip clip)
    {
        if (seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    
    public void UpdateWindByHeight(float playerY)
    {
        if (windSource == null || windSE == null)
            return;

        // 開始高度から最大音量高度までの割合を計算
        float volume = Mathf.InverseLerp(
            windStartHeight,
            windFullVolumeHeight,
            playerY
        );

        windSource.volume = volume;

        // 聞こえる高さならループ再生
        if (volume > 0f)
        {
            if (!windSource.isPlaying)
            {
                windSource.clip = windSE;
                windSource.loop = true;
                windSource.Play();
            }
        }
        else
        {
            if (windSource.isPlaying)
            {
                windSource.Stop();
            }
        }
    }


}
