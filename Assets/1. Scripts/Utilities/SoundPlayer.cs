using UnityEngine;
using System.Collections;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // 배경음 플레이어
    [SerializeField] private AudioSource typingSoundPlayer; // 타이핑 효과음 플레이어 (반복)
    [SerializeField] private AudioSource[] UISoundLoopPlayer; // UI 효과음 플레이어 (반복)
    [SerializeField] private AudioSource[] UISoundPlayer; // UI 효과음 플레이어들

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // 배경음들
    [SerializeField] private AudioClip[] UISoundClip; // UI 효과음들
    [SerializeField] private AudioClip[] UISoundClip_LOOP; // UI 효과음들

    
    private float bgmVolume = 0.5f;
    private Coroutine bgmCoroutine;

    // 동시에 여러 UI 효과음들이 플레이 될 수도 있으므로 여러 플레이어를 두고 순차적으로 실행하기 위한 변수
    private int uiSoundPlayerCursor;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        bgmPlayer.volume = 0f;
        bgmCoroutine = StartCoroutine(ChangeBGMFade(BGM_OPENING));
    }
    
    private void Update()
    {
        // 마우스 클릭 시 효과음 발생
        if (Input.GetMouseButtonDown(0))
            UISoundPlay(Sound_Click);
    }

    public void ChangeVolume(float bgmVolume, float soundEffectVolume)
    {
        if (bgmVolume != -1)
        {
            this.bgmVolume = bgmVolume;
            bgmPlayer.volume = bgmVolume;
        }

        if (soundEffectVolume != -1)
        {
            typingSoundPlayer.volume = soundEffectVolume;
            foreach (AudioSource audio in UISoundLoopPlayer) audio.volume = soundEffectVolume;
            foreach (AudioSource audio in UISoundPlayer) audio.volume = soundEffectVolume;
        }
    }
    
    public void ChangeBGM(int bgm)
    {
        if (bgmCoroutine != null) {
            StopCoroutine(bgmCoroutine);
            bgmCoroutine = null;
        }
        bgmCoroutine = StartCoroutine(ChangeBGMFade(bgm));
    }
    
    private IEnumerator ChangeBGMFade(int bgm) {
        const float fadeDuration = 2f;

        if (bgm == BGM_STOP) {
            float startVol = bgmPlayer.volume;
            float t = 0f;
            while (t < fadeDuration) {
                t += Time.unscaledDeltaTime;
                bgmPlayer.volume = Mathf.Lerp(startVol, 0f, t / fadeDuration);
                yield return null;
            }
            bgmPlayer.volume = 0f;
            bgmPlayer.Stop();
            yield break;
        }

        if (bgmPlayer.isPlaying && bgmPlayer.volume > 0f) {
            float startVol = bgmPlayer.volume;
            float t = 0f;
            while (t < fadeDuration * 0.5f) {
                t += Time.unscaledDeltaTime;
                bgmPlayer.volume = Mathf.Lerp(startVol, 0f, t / (fadeDuration * 0.5f));
                yield return null;
            }
            bgmPlayer.volume = 0f;
        }

        if (bgm >= 0 && bgm < bgmClip.Length) {
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.Play();
        }

        {
            float t = 0f;
            while (t < fadeDuration * 0.5f) {
                t += Time.unscaledDeltaTime;
                bgmPlayer.volume = Mathf.Lerp(0f, bgmVolume, t / (fadeDuration * 0.5f));
                yield return null;
            }
            bgmPlayer.volume = bgmVolume;
        }

        bgmCoroutine = null;
    }

    public void UISoundPlay_LOOP(int num, bool play)
    {
        if (play)
        {
            // 이미 재생 중이면 return
            foreach(AudioSource audio in UISoundLoopPlayer)
                if (audio.clip == UISoundClip_LOOP[num] && audio.isPlaying) return;

            int index = 0;
            if (UISoundLoopPlayer[index].isPlaying) index++;

            UISoundLoopPlayer[index].clip = UISoundClip_LOOP[num];
            UISoundLoopPlayer[index].Play();
        }
        else
        {
            foreach(AudioSource audio in UISoundLoopPlayer)
            {
                if(audio.isPlaying && audio.clip == UISoundClip_LOOP[num])
                {
                    audio.Stop();
                    break;
                }
            }
        }
        
        return;
    }
    public void UISoundPlay(int num)
    {
        // 타이핑인 경우
        if (num == Sound_Typing) {
            typingSoundPlayer.Play();
            return;
        }

        // 음악이 여러개인 경우 랜덤으로 재생
        if (num == Sound_LockerKeyMovement || num == Sound_ChairMovement)
            num = Random.Range(num, num + 2);
        
        // 재생할 효과음 변경
        UISoundPlayer[uiSoundPlayerCursor].clip = UISoundClip[num];

        // 일부 효과음의 경우 음악이 재생되는 위치를 바꾼다
        UISoundPlayer[uiSoundPlayerCursor].panStereo = SoundPosition();

        // 음악 재생
        UISoundPlayer[uiSoundPlayerCursor].Play();

        // 다음 효과음 Player로 넘긴다
        uiSoundPlayerCursor = (uiSoundPlayerCursor + 1) % UISoundPlayer.Length;
    }


    // -1을 넣으면 모든 효과음 종료 (타이핑, 루프 효과음, 배경음 제외)
    public void UISoundStop(int num)
    {
        if (num == Sound_Typing)
        {
            typingSoundPlayer.Stop();
            return;
        }

        foreach (AudioSource audio in UISoundPlayer)
        {
            if (num == -1) { audio.Stop(); continue; }
            if (audio.clip == UISoundClip[num]) audio.Stop();
        }
    }
    private float SoundPosition()
    {
        Vector3 clickPosition = Input.mousePosition;
        float normalizedX = (clickPosition.x / Screen.width - 0.5f) * 2f;
        return normalizedX;
    }
}
