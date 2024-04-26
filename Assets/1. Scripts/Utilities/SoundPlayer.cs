using UnityEngine;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // 배경음 플레이어
    [SerializeField] private AudioSource typingSoundPlayer; // 타이핑 효과음 플레이어 (반복)
    [SerializeField] private AudioSource UISoundLoopPlayer; // UI 효과음 플레이어 (반복)
    [SerializeField] private AudioSource[] UISoundPlayer; // UI 효과음 플레이어들

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // 배경음들
    [SerializeField] private AudioClip[] UISoundClip; // UI 효과음들
    [SerializeField] private AudioClip[] UISoundClip_LOOP; // UI 효과음들

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
    public void ChangeBGM(DialogueType dialogueType)
    {
        // 배경 음악을 바꾼다
        bgmPlayer.clip = bgmClip[dialogueType.ToInt()];
        bgmPlayer.Play();
    }
    public void UISoundPlay_LOOP(int num, bool play)
    {
        if (play)
        {
            switch (num)
            {
                case Sound_FootStep: num = Random.Range(num, num + 6); break;
            }
            UISoundLoopPlayer.clip = UISoundClip_LOOP[num];
            UISoundLoopPlayer.Play();
        }
        else UISoundLoopPlayer.Stop();
        
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
