using UnityEngine;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }
    [SerializeField] private AudioSource bgmPlayer; // ����� �÷��̾�
    [SerializeField] private AudioClip[] bgmClip; // �������
    [SerializeField] private AudioSource[] UISoundPlayer; // UI ȿ���� �÷��̾��
    [SerializeField] private AudioClip[] UISoundClip; // UI ȿ������

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

    public void UISoundPlay(int type)
    {
        // ������ �������� ��� �������� ���
        if (type == Sound_LockerKeyMovement || type == Sound_ChairMovement)
            type = Random.Range(type, type + 2);

        // ����� ���� ����
        UISoundPlayer[UISoundPlayerCursor].clip = UISoundClip[type];

        // ���� ���
        UISoundPlayer[UISoundPlayerCursor].Play();

        // ���� ȿ���� Player�� �ѱ��
        UISoundPlayerCursor = (UISoundPlayerCursor + 1) % UISoundPlayer.Length;
    }
}
