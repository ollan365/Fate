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

    // Volume
    private float bgmVolume = 0.5f;

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
        this.bgmVolume = bgmVolume;
        bgmPlayer.volume = bgmVolume;

        typingSoundPlayer.volume = soundEffectVolume;
        foreach (AudioSource audio in UISoundLoopPlayer) audio.volume = soundEffectVolume;
        foreach (AudioSource audio in UISoundPlayer) audio.volume = soundEffectVolume;
    }
    public void ChangeBGM(int bgm, bool play)
    {
        StartCoroutine(ChangeBGMFade(bgm, play));
    }
    private IEnumerator ChangeBGMFade (int bgm, bool play)
    {
        float fadeDuration = 2f;
        float currentTime = 0f;

        if (!play)
        {
            // ������ ���̱�
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                bgmPlayer.volume = Mathf.Lerp(bgmVolume, 0, currentTime / fadeDuration);
                yield return null;
            }

            // ���� ������ 0���� ����
            bgmPlayer.volume = 0;
            bgmPlayer.Stop();
        }
        if (play)
        {
            // ���ο� BGM ���� �� ���
            bgmPlayer.clip = bgmClip[bgm];
            bgmPlayer.Play();

            currentTime = 0f; // currentTime �ʱ�ȭ

            // ������ �ٽ� Ű���
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                bgmPlayer.volume = Mathf.Lerp(0, bgmVolume, currentTime / fadeDuration);
                yield return null;
            }

            // ���� ���� ����
            bgmPlayer.volume = bgmVolume;
        }
    }

    public void UISoundPlay_LOOP(int num, bool play)
    {
        if (play)
        {
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
