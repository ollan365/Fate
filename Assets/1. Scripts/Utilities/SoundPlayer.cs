using UnityEngine;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // ����� �÷��̾�
    [SerializeField] private AudioSource[] UISoundLoopPlayer; // UI ȿ���� �÷��̾�� (�ݺ�)
    [SerializeField] private AudioSource[] UISoundPlayer; // UI ȿ���� �÷��̾��

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // �������
    [SerializeField] private AudioClip[] UISoundClip_ROOM_OBJECT; // UI ȿ������
    [SerializeField] private AudioClip[] UISoundClip_FOLLOW_OBJECT; // UI ȿ������
    [SerializeField] private AudioClip[] UISoundClip_LOOP; // UI ȿ������
    [SerializeField] private AudioClip[] UISoundClip_ETC; // UI ȿ������

    // ���ÿ� ���� UI ȿ�������� �÷��� �� ���� �����Ƿ� ���� �÷��̾ �ΰ� ���������� �����ϱ� ���� ����
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
        // ���콺 Ŭ�� �� ȿ���� �߻�
        if (Input.GetMouseButtonDown(0))
            UISoundPlay(SoundType.ETC, Sound_Click);
    }
    public void ChangeBGM(DialogueType dialogueType, bool stopBGM)
    {
        if (stopBGM) { bgmPlayer.Stop(); return; }
        // ��� ������ �ٲ۴�
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
        // ������ �������� ��� �������� ���
        switch (type)
        {
            case SoundType.ROOM_OBJECT:
                if (num == Sound_LockerKeyMovement || num == Sound_ChairMovement)
                    num = Random.Range(num, num + 2);
                break;
        }

        // ����� ���� ����
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
        

        // ���� ���
        UISoundPlayer[UISoundPlayerCursor].Play();

        // ���� ȿ���� Player�� �ѱ��
        UISoundPlayerCursor = (UISoundPlayerCursor + 1) % UISoundPlayer.Length;
    }
}
