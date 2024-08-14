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
    private int UISoundPlayerCursor;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
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
        this.bgmVolume = bgmVolume;
        bgmPlayer.volume = bgmVolume;

        typingSoundPlayer.volume = soundEffectVolume;
        foreach (AudioSource audio in UISoundLoopPlayer) audio.volume = soundEffectVolume;
        foreach (AudioSource audio in UISoundPlayer) audio.volume = soundEffectVolume;
    }
    public void ChangeBGM(int bgm)
    {
        StopCoroutine(bgmCoroutine);
        bgmCoroutine = StartCoroutine(ChangeBGMFade(bgm));
    }
    private IEnumerator ChangeBGMFade(int bgm)
    {
        float fadeDuration = 2f;
        float currentTime = 0f;

        // 새로운 BGM 설정 및 재생
        if (bgm != BGM_STOP)
        {
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.Play();
        }

        // 볼륨을 다시 키우기
        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            if (bgm != BGM_STOP) bgmPlayer.volume = Mathf.Lerp(0, bgmVolume, currentTime / fadeDuration);
            else bgmPlayer.volume = Mathf.Lerp(bgmVolume, 0, currentTime / fadeDuration);

            yield return null;
        }

        // 최종 볼륨 설정
        if (bgm != BGM_STOP) bgmPlayer.volume = bgmVolume;
    }

    public void UISoundPlay_LOOP(int num, bool play)
    {
        if (play)
        {
            // 이미 재생 중이면 return
            foreach(AudioSource audio in UISoundLoopPlayer)
                if (audio.clip == UISoundClip_LOOP[num]) return;

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
        if (num == Sound_Typing)
        {
            typingSoundPlayer.Play();
            return;
        }

        // 음악이 여러개인 경우 랜덤으로 재생
        if (num == Sound_LockerKeyMovement || num == Sound_ChairMovement)
            num = Random.Range(num, num + 2);
        
        // 재생할 효과음 변경
        UISoundPlayer[UISoundPlayerCursor].clip = UISoundClip[num];

        // 일부 효과음의 경우 음악이 재생되는 위치를 바꾼다
        UISoundPlayer[UISoundPlayerCursor].panStereo = SoundPosition();

        // 음악 재생
        UISoundPlayer[UISoundPlayerCursor].Play();

        // 다음 효과음 Player로 넘긴다
        UISoundPlayerCursor = (UISoundPlayerCursor + 1) % UISoundPlayer.Length;
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
