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

    [Header("Volume")]
    public float bgmVolume;
    public float soundEffectVolume;

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
    }
    private void Update()
    {
        // 마우스 클릭 시 효과음 발생
        if (Input.GetMouseButtonDown(0))
            UISoundPlay(Sound_Click);
    }

    public void ChangeVolume(float bgmVolume, float soundEffectVolume)
    {
        bgmPlayer.volume = bgmVolume;

        typingSoundPlayer.volume = soundEffectVolume;
        foreach (AudioSource audio in UISoundLoopPlayer) audio.volume = soundEffectVolume;
        foreach (AudioSource audio in UISoundPlayer) audio.volume = soundEffectVolume;
    }
    public void ChangeBGM(int bgm)
    {
        bool stop = false;
        if (bgmPlayer.clip == bgmClip[bgm]) stop = true;

        switch (bgm)
        {
            case BGM_ROOM:
                StartCoroutine(ChangeBGMFade(bgm, stop, 0.5f));
                break;
            case BGM_FOLLOW:
                StartCoroutine(ChangeBGMFade(bgm, stop, 0.5f));
                break;
            case BGM_MINIGAME:
                StartCoroutine(ChangeBGMFade(bgm, stop, 0.5f));
                break;
        }
    }
    private IEnumerator ChangeBGMFade (int bgm, bool stop, float time)
    {
        if (stop)
        {
            float currentTime = 0.0f;

            while (currentTime < time)
            {
                currentTime += Time.deltaTime;
                bgmPlayer.volume = Mathf.Lerp(1, 0, currentTime / time);
                yield return null;
            }
            bgmPlayer.Stop();
        }
        else
        {
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.volume = 1;
            bgmPlayer.Play();
        }
    }
    public void UISoundPlay_LOOP(int num, bool play, float speed)
    {
        if (play) // loop를 쓰는게 발걸음 밖에 없고 뒤로도 없을 거 같아서 간단하게 만듦
        {
            UISoundLoopPlayer[num].pitch = speed;
            UISoundLoopPlayer[num].clip = UISoundClip_LOOP[num];
            UISoundLoopPlayer[num].Play();
        }
        else
        {
            UISoundLoopPlayer[num].Stop();
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
    private float SoundPosition()
    {
        Vector3 clickPosition = Input.mousePosition;
        float normalizedX = (clickPosition.x / Screen.width - 0.5f) * 2f;
        return normalizedX;
    }
}
