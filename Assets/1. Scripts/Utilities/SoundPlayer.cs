using UnityEngine;
using System.Collections;
using static Constants;
using System;

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
    private Coroutine bgmFadeCoroutine;

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
        bgmFadeCoroutine = StartCoroutine(ChangeBGMFade(BGM_OPENING));
    }
    
    private void Update()
    {
        // 마우스 클릭 시 효과음 발생
        if (Input.GetMouseButtonDown(0))
            UISoundPlay(Sound_Click);
    }

    public void ChangeVolume(bool isChangeBGM, float targetVolume) {
        if (isChangeBGM) {
            bgmVolume = targetVolume;
            bgmPlayer.volume = targetVolume;
        } else {
            typingSoundPlayer.volume = targetVolume;
            foreach (AudioSource audioSource in UISoundLoopPlayer) 
                audioSource.volume = targetVolume;
            foreach (AudioSource audioSource in UISoundPlayer) 
                audioSource.volume = targetVolume;
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
        float fadeDuration = 2f;
        if (bgmFadeCoroutine != null)
        {
            StopCoroutine(bgmFadeCoroutine);
            bgmFadeCoroutine = null;
        }

        // BGM 정지 요청 시 BGM fade out
        if (bgm == BGM_STOP) {
            yield return FadeTo(0f, fadeDuration);
            bgmPlayer.Stop();
            bgmCoroutine = null;
            yield break;
        }

        // 다른 BGM으로 전환 시 현재 곡을 먼저 fade out
        if (bgmPlayer.isPlaying && bgmPlayer.volume > 0f) {
            yield return FadeTo(0f, fadeDuration);
        }

        // 새 클립 할당 후 재생
        if (bgm >= 0 && bgm < bgmClip.Length) {
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.Play();
        }

        // 새로운 BGM 페이드 인
        yield return FadeTo(bgmVolume, fadeDuration * 0.5f);

        bgmCoroutine = null;
    }

    public void SetMuteBGM(bool isMute)
    {
        if (bgmPlayer == null) return;

        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        float target = isMute ? 0f : Mathf.Clamp01(bgmVolume);
        bgmFadeCoroutine = StartCoroutine(FadeTo(target, 1));
    }

    private IEnumerator FadeTo(float targetVolume, float duration)
    {
        // 켜질 목표(>0)인데 재생 중이 아니면 먼저 재생 시작
        if (targetVolume > 0f && !bgmPlayer.isPlaying)
            bgmPlayer.Play();

        float start = bgmPlayer.volume;
        float t = 0f;

        // unscaledDeltaTime을 쓰면 게임 일시정지 중에도 페이드 가능
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float u = Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));
            bgmPlayer.volume = Mathf.Lerp(start, targetVolume, u);
            yield return null;
        }

        bgmPlayer.volume = targetVolume;

        bgmFadeCoroutine = null;
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
            num = UnityEngine.Random.Range(num, num + 2);
        
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
