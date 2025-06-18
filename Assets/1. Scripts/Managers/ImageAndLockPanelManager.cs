using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageAndLockPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private GameObject TutorialBlockingPanel;
    private GameObject objectImageGroup;
    private Image objectImageImageComponent;
    private RectTransform objectImageRectTransform;

    [Header("방탈출 다회차 이벤트 오브젝트 확대 이미지")]
    [SerializeField] private Sprite newTeddyBearImage;

    [Header("방탈출1 이벤트 오브젝트 확대 이미지")]
    [SerializeField] private Sprite amuletImage;
    [SerializeField] private Sprite clockImage;
    [SerializeField] private Sprite keysImage;
    [SerializeField] private Sprite knifeImage;
    [SerializeField] private Sprite posterImage;
    [SerializeField] private Sprite liquorAndPillsImage;
    [SerializeField] private Sprite letterOfResignationImage;
    [SerializeField] private Sprite photoInsideBoxImage;
    [SerializeField] private Sprite cafePintInBagImage;
    [SerializeField] private Sprite posterOversideImage;

    [Header("방탈출2 이벤트 오브젝트 확대 이미지")]
    [SerializeField] private Sprite tinCaseImage;
    [SerializeField] private Sprite sewingBoxImage;
    [SerializeField] private Sprite threadImage;
    [SerializeField] private Sprite hospitalPrintImage;
    [SerializeField] private Sprite poster2Image;
    [SerializeField] private Sprite starStickerImage;
    [SerializeField] private Sprite shoppingBag2Image;
    [SerializeField] private Sprite medicine2Image;
    [SerializeField] private Sprite ticketImage;
    [SerializeField] private Sprite book2CoverImage;
    [SerializeField] private Sprite closetKey2Image;

    [Header("잠금 장치들")]
    [SerializeField] private GameObject laptopGameObject;
    [SerializeField] private GameObject clockGameObject;
    [SerializeField] private GameObject diaryGameObject;
    [SerializeField] private GameObject calendarGameObject;
    [SerializeField] private GameObject tinCaseGameObject;
    [SerializeField] private GameObject sewingBoxGameObject;
    [SerializeField] private GameObject diary2GameObject;
    [SerializeField] private GameObject book2GameObject;
    [SerializeField] private GameObject dreamDiaryGameObject;
    
    [Header("퍼즐 오브젝트들")]
    [SerializeField] private GameObject laptopPuzzleObject;
    [SerializeField] private GameObject clockPuzzleObject;
    [SerializeField] private GameObject diaryPuzzleObjectClosed;
    [SerializeField] private GameObject diaryPuzzleObjectOpen;
    [SerializeField] private GameObject calendarPuzzleObject;
    [SerializeField] private GameObject tinCasePuzzleObject;
    [SerializeField] private GameObject sewingBoxPuzzleObject;
    [SerializeField] private GameObject book2PuzzleObject;
    [SerializeField] private GameObject dreamDiaryPuzzleObject;

    [Header("튜토리얼 강조 이미지들")]
    [SerializeField] private GameObject LeftMoveButton;
    [SerializeField] private GameObject RightMoveButton;
    [SerializeField] private GameObject Chair;
    [SerializeField] private GameObject Carpet;
    [SerializeField] private GameObject CarpetPaper;
    [SerializeField] private GameObject CarpetOpen;
    [SerializeField] private GameObject MemoButtonImage;

    private Dictionary<string, Sprite> imageDictionary;
    private Dictionary<string, GameObject> lockObjectDictionary;
    private Dictionary<string, GameObject[]> puzzleObjectDictionary;
    private Dictionary<string, GameObject> TutorialimageDictionary;
    [SerializeField] public bool isImageActive = false;
    [SerializeField] public bool isLockObjectActive = false;
    [SerializeField] private bool isTutorialObjectActive = false;
    private string currentLockObjectName = null;
    private float maxHeight = 550f;
    private float maxWidth = 890f;

    private void Awake()
    {
        objectImageGroup = UIManager.Instance.objectImageParentRoom;
        GameObject objectImage = UIManager.Instance.objectImageRoom;
        objectImageImageComponent = objectImage.GetComponent<Image>();
        objectImageRectTransform = objectImage.GetComponent<RectTransform>();

        imageDictionary = new Dictionary<string, Sprite>()  // imageDictionary 초기화
        {
            // 방탈출 다회차 Object 확대 이미지
            {"newTeddyBear", newTeddyBearImage},

            // 방탈출1 확대 이미지
            {"amulet", amuletImage},
            {"clock", clockImage},
            {"keys", keysImage },
            {"knife", knifeImage },
            {"poster", posterImage },
            {"liquorAndPills", liquorAndPillsImage },
            {"letterOfResignation", letterOfResignationImage},
            {"photoInsideBox", photoInsideBoxImage},
            {"cafePintInBagImage", cafePintInBagImage},
            {"posterOverside", posterOversideImage},

            // 방탈출2 확대 이미지
            {"tinCase", tinCaseImage},
            {"sewingBox", sewingBoxImage},
            {"thread", threadImage},
            {"hospitalPrint", hospitalPrintImage},
            {"poster2", poster2Image},
            {"starSticker", starStickerImage},
            {"shoppingBag2", shoppingBag2Image},
            {"medicine2", medicine2Image},
            {"ticket",ticketImage},
            {"book2Cover", book2CoverImage},
            {"closetKey2", closetKey2Image},
        };

        lockObjectDictionary = new Dictionary<string, GameObject>() {
            { "laptop", laptopGameObject },
            { "clock", clockGameObject },
            { "diary", diaryGameObject },
            { "calendar", calendarGameObject },
            { "tinCase", tinCaseGameObject },
            { "sewingBox", sewingBoxGameObject},
            { "diary2", diary2GameObject },
            { "book2", book2GameObject },
            { "dreamDiary",dreamDiaryGameObject},
        };

        puzzleObjectDictionary = new Dictionary<string, GameObject[]>() {
            { "laptop", new[] { laptopPuzzleObject } },
            { "clock", new[] { clockPuzzleObject } },
            { "diary", new[] { diaryPuzzleObjectClosed, diaryPuzzleObjectOpen } },
            { "calendar", new[] { calendarPuzzleObject } },
            { "tinCase", new[] { tinCasePuzzleObject } },
            { "sewingBox", new[] { sewingBoxPuzzleObject } },
            { "book2", new[] { book2PuzzleObject } },
            { "dreamDiary", new[] { dreamDiaryPuzzleObject } }
        };

        TutorialimageDictionary = new Dictionary<string, GameObject>()
        {
            {"TutorialLeftMoveButton", LeftMoveButton},
            {"TutorialRightMoveButton", RightMoveButton},
            {"TutorialChair", Chair},
            {"TutorialCarpet", Carpet},
            {"TutorialCarpetPaper", CarpetPaper},
            {"TutorialCarpetOpen", CarpetOpen},
            {"TutorialMemoButton", MemoButtonImage}
        };
    }

    public bool GetIsTutorialObjectActive() {
        return isTutorialObjectActive;
    }

    public void OnExitButtonClick() {
        if (isImageActive) {
            SetObjectImageGroup(false);

            bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);

            return;
        }

        if (isLockObjectActive) {
            SetLockObject(false);

            bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);
        }

        if (isTutorialObjectActive) {
            if (TutorialBlockingPanel.activeSelf && currentLockObjectName == null)
                SetTutorialBlockingPanel(false);
            else
                SetTutorialImageObject(false);

            bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);
        }
    }

    public void SetObjectImageGroup(bool isTrue, string eventObjectName = null)
    {
        if (isTrue && eventObjectName == null) {
            Debug.Log("eventObjectName must be a correct value!");
            return;
        }

        isImageActive = isTrue;

        if (isTrue) {
            Sprite rawSprite = imageDictionary[eventObjectName];
            float rawHeight = rawSprite.rect.height;
            float rawWidth = rawSprite.rect.width;

            float multiplier = maxHeight / rawHeight;;
            float preferredHeight = maxHeight;
            float preferredWidth = rawWidth * multiplier;

            if (preferredWidth > maxWidth)
            {
                multiplier = maxWidth / preferredWidth;;
                preferredWidth = maxWidth;
                preferredHeight *= multiplier;
            }

            objectImageImageComponent.sprite = rawSprite;
            objectImageRectTransform.sizeDelta = new Vector2(preferredWidth, preferredHeight);

            RoomManager.Instance.SetIsInvestigating(true);
            RoomManager.Instance.SetButtons();
        }

        UIManager.Instance.SetUI(eUIGameObjectName.ObjectImageParentRoom, isTrue, true);
    }

    public IEnumerator SetObjectImageGroupCoroutine(bool isTrue, string eventObjectName = null, float delayTime = 0.1f) {
        yield return new WaitForSeconds(delayTime);
        SetObjectImageGroup(isTrue, eventObjectName);
    }

    public void SetLockObject(bool isTrue, string lockObjectName = null) {
        if (isTrue && lockObjectName == null) {
            Debug.Log("lockObjectName must be a correct value!");
            return;
        }

        isLockObjectActive = isTrue;

        if (isTrue) {
            SetObjectImageGroup(false);  // 이미지 켜져있을 때 Lock object activate하면 이미지 숨기기
            lockObjectDictionary[lockObjectName].gameObject.SetActive(true);

            RoomManager.Instance.SetIsInvestigating(true);
            RoomManager.Instance.SetButtons();
        } else if (puzzleObjectDictionary.TryGetValue(currentLockObjectName, out var puzzleObjects)) {
            foreach (var puzzleObject in puzzleObjects)
                UIManager.Instance.AnimateUI(puzzleObject, false, true);
            StartCoroutine(DeactivateLockObjectWithDelay(currentLockObjectName, UIManager.Instance.fadeAnimationDuration));
        } else
            lockObjectDictionary[currentLockObjectName].gameObject.SetActive(false);

        currentLockObjectName = lockObjectName;
    }
    
    private IEnumerator DeactivateLockObjectWithDelay(string lockObjectName, float delayTime = 0.5f) {
        yield return new WaitForSeconds(delayTime);
        lockObjectDictionary[lockObjectName].gameObject.SetActive(false);
    }

    public void SetTutorialImageObject(bool isTrue, string tutorialImageObjectName = null) {
        if (isTrue && tutorialImageObjectName == null) {
            Debug.Log("lockObjectName must be a correct value!");
            return;
        }

        isTutorialObjectActive = isTrue;

        if (isTrue) {
            SetObjectImageGroup(false);  // 이미지 켜져있을 때 Lock object activate하면 이미지 숨기기

            if (tutorialImageObjectName == "TutorialMoveButton")
                SetTutorialMoveButtonForce(isTrue);
            else
                TutorialimageDictionary[tutorialImageObjectName].gameObject.SetActive(true);

            RoomManager.Instance.SetIsInvestigating(true);
        }
        else {
            if (currentLockObjectName == "TutorialMoveButton") {
                TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(false);
                TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(false);
            } else
                TutorialimageDictionary[currentLockObjectName].gameObject.SetActive(false);
        }
        currentLockObjectName = tutorialImageObjectName;

        SetTutorialBlockingPanel();
    }

    private void SetTutorialBlockingPanel() {
        bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
        TutorialBlockingPanel.SetActive(isImageOrLockActive);
    }

    public void SetTutorialBlockingPanel(bool isTrue) {
        isTutorialObjectActive = isTrue;
        RoomManager.Instance.SetIsInvestigating(true);

        bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
        TutorialBlockingPanel.SetActive(isImageOrLockActive);
    }

    private void SetTutorialMoveButtonForce(bool isTrue) {
        int notSeenSide = RoomManager.Instance.tutorialManager.getSeenSideStateFalse();

        TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(isTrue);
        TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(isTrue);

        if (RoomManager.Instance.currentSideIndex == 0) {
            switch (notSeenSide) // 아직 안 둘러본 방쪽의 이동 버튼 강조
            {
                case 0:
                    return;

                case 1:
                    TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(false);
                    return;

                case 2:
                    TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(false);
                    return;
            }
        }
        else if (RoomManager.Instance.currentSideIndex == 1) 
            TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(false);
        else if (RoomManager.Instance.currentSideIndex == 2) 
            TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(false);
    }
}
