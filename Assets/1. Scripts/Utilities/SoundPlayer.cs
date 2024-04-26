using UnityEngine;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // ����� �÷��̾�
    [SerializeField] private AudioSource typingSoundPlayer; // Ÿ���� ȿ���� �÷��̾� (�ݺ�)
    [SerializeField] private AudioSource UISoundLoopPlayer; // UI ȿ���� �÷��̾� (�ݺ�)
    [SerializeField] private AudioSource[] UISoundPlayer; // UI ȿ���� �÷��̾��

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // �������
    [SerializeField] private AudioClip[] UISoundClip; // UI ȿ������
    [SerializeField] private AudioClip[] UISoundClip_LOOP; // UI ȿ������

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
            UISoundPlay(Sound_Click);
    }
    public void ChangeBGM(DialogueType dialogueType)
    {
        // ��� ������ �ٲ۴�
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
        // Ÿ������ ���
        if (num == Sound_Typing)
        {
            typingSoundPlayer.Play();
            return;
        }

        // ������ �������� ��� �������� ���
        if (num == Sound_LockerKeyMovement || num == Sound_ChairMovement)
            num = Random.Range(num, num + 2);
        
        // ����� ȿ���� ����
        UISoundPlayer[UISoundPlayerCursor].clip = UISoundClip[num];

        // �Ϻ� ȿ������ ��� ������ ����Ǵ� ��ġ�� �ٲ۴�
        UISoundPlayer[UISoundPlayerCursor].panStereo = SoundPosition();

        // ���� ���
        UISoundPlayer[UISoundPlayerCursor].Play();

        // ���� ȿ���� Player�� �ѱ��
        UISoundPlayerCursor = (UISoundPlayerCursor + 1) % UISoundPlayer.Length;
    }
    private float SoundPosition()
    {
        Vector3 clickPosition = Input.mousePosition;
        float normalizedX = (clickPosition.x / Screen.width - 0.5f) * 2f;
        return normalizedX;
    }
}
