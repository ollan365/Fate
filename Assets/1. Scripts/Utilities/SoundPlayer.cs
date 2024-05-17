using UnityEngine;
using System.Collections;
using static Constants;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource bgmPlayer; // ����� �÷��̾�
    [SerializeField] private AudioSource typingSoundPlayer; // Ÿ���� ȿ���� �÷��̾� (�ݺ�)
    [SerializeField] private AudioSource[] UISoundLoopPlayer; // UI ȿ���� �÷��̾� (�ݺ�)
    [SerializeField] private AudioSource[] UISoundPlayer; // UI ȿ���� �÷��̾��

    [Header("AudioClip")]
    [SerializeField] private AudioClip[] bgmClip; // �������
    [SerializeField] private AudioClip[] UISoundClip; // UI ȿ������
    [SerializeField] private AudioClip[] UISoundClip_LOOP; // UI ȿ������

    [Header("Volume")]
    public float bgmVolume;
    public float soundEffectVolume;

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
        if (play) // loop�� ���°� �߰��� �ۿ� ���� �ڷε� ���� �� ���Ƽ� �����ϰ� ����
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
