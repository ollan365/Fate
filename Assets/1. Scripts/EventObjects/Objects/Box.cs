using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : EventObject, IResultExecutable
{
    // ************************* temporary members for open animation *************************
    [SerializeField] private Animator boxAnimator;
    // ********************************************************************************
    [SerializeField] private Image boxImage;
    [SerializeField] private Sprite closedBoxSprite;
    [SerializeField] private Sprite openBoxSprite;

    public List<GameObject> sideClosedBox;
    public List<GameObject> sideOpenBox;

    protected override void Awake()
    {
        base.Awake();
        boxImage = GetComponent<Image>();
    }

    private void Start()
    {
        RegisterWithResultManager();
    }

    private void OnEnable()
    {
        RegisterWithResultManager();
        UpdateImageState();
    }

    private void RegisterWithResultManager()
    {
        if (ResultManager.Instance != null)
            ResultManager.Instance.RegisterExecutable("Box", this);
    }

    protected override bool CanInteract()
    {
        return !GameManager.Instance.GetIsBusy();
    }

    public void ExecuteAction()
    {
        StartCoroutine(OpenBoxCoroutine());
    }

    // ************************* temporary methods for open animation *************************
    private IEnumerator OpenBoxCoroutine()
    {
        bool clockTimeCorrect = (bool)GameManager.Instance.GetVariable("ClockTimeCorrect");
        if (clockTimeCorrect)
        {
            RoomManager.Instance.SetIsInvestigating(true);
            boxAnimator.SetBool("open_Box", true);

            // 애니메이션이 끝날 때까지 대기
            yield return new WaitForSeconds(GetAnimationLength("open_Box"));

            GameManager.Instance.SetVariable("BoxOpened", true);
            UpdateImageState();
        }
    }

    private float GetAnimationLength(string animationName)
    {
        AnimationClip[] clips = boxAnimator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == animationName)
                return clip.length;
        }
        return 0f;
    }
    // *******************************************************************************

    private void UpdateImageState()
    {
        bool boxOpened = (bool)GameManager.Instance.GetVariable("BoxOpened");

        foreach (GameObject closedBox in sideClosedBox)
            closedBox.SetActive(!boxOpened);

        foreach (GameObject openBox in sideOpenBox)
            openBox.SetActive(boxOpened);

        // 확대 시점의 Box 이미지 상태 변경
        if (boxImage != null)
        {
            boxImage.sprite = boxOpened ? openBoxSprite : closedBoxSprite;
        }
    }

}