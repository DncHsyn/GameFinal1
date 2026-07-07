using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    public static BackgroundMusicPlayer Instance { get; private set; }

    [Header("Music")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.5f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool persistBetweenScenes = true;

    private AudioSource m_AudioSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        SetupAudioSource();

        if (persistBetweenScenes)
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (backgroundMusic == null || m_AudioSource == null)
            return;

        if (m_AudioSource.clip != backgroundMusic)
            m_AudioSource.clip = backgroundMusic;

        m_AudioSource.volume = volume;

        if (!m_AudioSource.isPlaying)
            m_AudioSource.Play();
    }

    public void Stop()
    {
        if (m_AudioSource != null)
            m_AudioSource.Stop();
    }

    public void SetVolume(float _volume)
    {
        volume = Mathf.Clamp01(_volume);

        if (m_AudioSource != null)
            m_AudioSource.volume = volume;
    }

    public void SetMusic(AudioClip _clip, bool _playImmediately = true)
    {
        backgroundMusic = _clip;

        if (m_AudioSource == null)
            return;

        m_AudioSource.clip = backgroundMusic;

        if (_playImmediately)
            Play();
    }

    private void SetupAudioSource()
    {
        m_AudioSource = GetComponent<AudioSource>();

        if (m_AudioSource == null)
            m_AudioSource = gameObject.AddComponent<AudioSource>();

        m_AudioSource.playOnAwake = false;
        m_AudioSource.loop = true;
        m_AudioSource.spatialBlend = 0f;
        m_AudioSource.volume = volume;
    }
}
