using UnityEngine;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // 배경음 플레이어
    [SerializeField] private AudioSource[] UISoundLoopPlayer; // UI 효과음 플레이어들 (반복)
    [SerializeField] private AudioSource[] UISoundPlayer; // UI 효과음 플레이어들

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // 배경음들
    [SerializeField] private AudioClip[] UISoundClip_ROOM_OBJECT; // UI 효과음들
    [SerializeField] private AudioClip[] UISoundClip_FOLLOW_OBJECT; // UI 효과음들
    [SerializeField] private AudioClip[] UISoundClip_LOOP; // UI 효과음들
    [SerializeField] private AudioClip[] UISoundClip_ETC; // UI 효과음들

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
            UISoundPlay(SoundType.ETC, Sound_Click);
    }
    public void ChangeBGM(DialogueType dialogueType, bool stopBGM)
    {
        if (stopBGM) { bgmPlayer.Stop(); return; }
        // 배경 음악을 바꾼다
        bgmPlayer.clip = bgmClip[dialogueType.ToInt()];
        bgmPlayer.Play();
    }
    public void UISoundPlay_LOOP(int num, bool play)
    {
        if (num == 0)
        {
            if (play) UISoundLoopPlayer[0].Play();
            else UISoundLoopPlayer[0].Stop();
        }
        else
        {
            if (play)
            {
                switch (num)
                {
                    case Sound_FootStep: num = Random.Range(num, num + 6); break;
                }
                UISoundLoopPlayer[1].clip = UISoundClip_LOOP[num];
                UISoundLoopPlayer[1].Play();
            }
            else UISoundLoopPlayer[1].Stop();
        }
        return;
    }
    public void UISoundPlay(SoundType type, int num)
    {
        // 음악이 여러개인 경우 랜덤으로 재생
        switch (type)
        {
            case SoundType.ROOM_OBJECT:
                if (num == Sound_LockerKeyMovement || num == Sound_ChairMovement)
                    num = Random.Range(num, num + 2);
                break;
        }

        // 재생할 음악 변경
        switch (type)
        {
            case SoundType.ROOM_OBJECT:
                UISoundPlayer[UISoundPlayerCursor].clip = UISoundClip_ROOM_OBJECT[num];
                break;
            case SoundType.FOLLOW_OBJECT:
                UISoundPlayer[UISoundPlayerCursor].clip = UISoundClip_FOLLOW_OBJECT[num];
                break;
            case SoundType.ETC:
                UISoundPlayer[UISoundPlayerCursor].clip = UISoundClip_ETC[num];
                break;
            default: return;
        }
        

        // 음악 재생
        UISoundPlayer[UISoundPlayerCursor].Play();

        // 다음 효과음 Player로 넘긴다
        UISoundPlayerCursor = (UISoundPlayerCursor + 1) % UISoundPlayer.Length;
    }
}
