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

    private const string BGM_VOLUME_KEY = "BGMVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const float DEFAULT_VOLUME = 0.5f;

    private float bgmVolume = DEFAULT_VOLUME;
    private float sfxVolume = DEFAULT_VOLUME;
    private Coroutine bgmCoroutine;
    private Coroutine bgmFadeCoroutine;

    // 동시에 여러 UI 효과음들이 플레이 될 수도 있으므로 여러 플레이어를 두고 순차적으로 실행하기 위한 변수
    private int uiSoundPlayerCursor;

    public enum SfxPriority { Low = 0, High = 1 }
    [SerializeField] private int reservedHighPriorityChannels = 1;
    // UISoundPlayer 배열의 [0..reserved-1] 는 High 전용, [reserved..end] 는 Low 전용
    private float lastClickTime;
    [SerializeField] private float clickMinInterval = 0.05f; // 50ms 디바운스

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolumeSettings();
        }
        else
            Destroy(gameObject);
    }

    private void Start() {
        bgmPlayer.volume = 0f;
        bgmFadeCoroutine = StartCoroutine(ChangeBGMFade(BGM_OPENING));
    }

    private void LoadVolumeSettings() {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOLUME_KEY, DEFAULT_VOLUME);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_VOLUME);

        typingSoundPlayer.volume = sfxVolume;
        foreach (AudioSource audioSource in UISoundLoopPlayer)
            audioSource.volume = sfxVolume;
        foreach (AudioSource audioSource in UISoundPlayer)
            audioSource.volume = sfxVolume;
    }

    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
    
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
            PlayerPrefs.SetFloat(BGM_VOLUME_KEY, targetVolume);
        } else {
            sfxVolume = targetVolume;
            typingSoundPlayer.volume = targetVolume;
            foreach (AudioSource audioSource in UISoundLoopPlayer)
                audioSource.volume = targetVolume;
            foreach (AudioSource audioSource in UISoundPlayer)
                audioSource.volume = targetVolume;
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, targetVolume);
        }
        PlayerPrefs.Save();
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

    // 우선순위 버전의 재생
    public void UISoundPlay(int num, SfxPriority prio = SfxPriority.Low)
    {
        // 0) 타이핑인 경우 루프
        if (num == Sound_Typing) { typingSoundPlayer.Play(); return; }

        // 1) 클릭 디바운스 : 너무 빠른 연속 클릭으로 인한 효과음 재생이 풀을 잠식하는 걸 막음.
        if (num == Sound_Click && Time.unscaledTime - lastClickTime < clickMinInterval) return;
        if (num == Sound_Click) lastClickTime = Time.unscaledTime;

        // 2) 랜덤 후보 처리 (의자/락커 움직임)
        if (num == Sound_LockerKeyMovement || num == Sound_ChairMovement)
            num = UnityEngine.Random.Range(num, num + 2);

        // 3) 우선순위 구간 결정
        int start = (prio == SfxPriority.High) ? 0 : reservedHighPriorityChannels;
        int end = UISoundPlayer.Length;
        int len = Mathf.Max(0, end - start);
        if (len <= 0) { Debug.LogWarning("[SoundPlayer] No channels available for this priority range."); return; }

        // 4) 라운드로빈 시작점(공평하게 분산)
        int begin = start + (uiSoundPlayerCursor % Mathf.Max(1, len));

        // 5) 빈 채널 우선 탐색 (begin부터 한 바퀴)
        for (int k = 0; k < len; k++)
        {
            int i = start + ((begin - start + k) % len);
            var src = UISoundPlayer[i];
            if (!src.isPlaying)
            {
                PlayOnUISound(src, num);             // clip, pan, Play()
                uiSoundPlayerCursor++;        // 다음 시작점을 옮겨 공평 분산
                return;
            }
        }

        // 6) 빈 채널이 없고 High면 → Low 영역에서 보이스 스틸링
        if (prio == SfxPriority.High)
        {
            int lowStart = Mathf.Clamp(reservedHighPriorityChannels, 0, UISoundPlayer.Length);
            int lowLen = Mathf.Max(0, end - lowStart);
            if (lowLen > 0)
            {
                int stealBegin = lowStart + (uiSoundPlayerCursor % lowLen);
                for (int k = 0; k < lowLen; k++)
                {
                    int i = lowStart + ((stealBegin - lowStart + k) % lowLen);
                    var src = UISoundPlayer[i];
                    if (src.isPlaying)
                    {
                        src.Stop();               // 낮은 우선순위 재생 중지
                        PlayOnUISound(src, num);         // High를 꽂아 넣음
                        uiSoundPlayerCursor++;    // 커서 전진
                        return;
                    }
                }
            }
        }
    }

    private void PlayOnUISound(AudioSource src, int num)
    {
        // 재생할 효과음 변경
        src.clip = UISoundClip[num];
        src.panStereo = SoundPosition(); // 일부 효과음의 경우  클릭 위치 기반 PAN
        src.Play(); // 음악 재생
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
