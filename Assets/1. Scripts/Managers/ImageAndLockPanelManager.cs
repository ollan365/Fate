using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageAndLockPanelManager : MonoBehaviour
{
    private GameObject objectImageGroup;
    private Image objectImageImageComponent;
    private RectTransform objectImageRectTransform;
    public CanvasGroup currentLockObjectCanvasGroup;

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

    private Dictionary<string, Sprite> imageDictionary;
    public Dictionary<string, GameObject> lockObjectDictionary;
    public Dictionary<string, GameObject[]> puzzleObjectDictionary;
    [SerializeField] public bool isImageActive = false;
    [SerializeField] public bool isLockObjectActive = false;
    public string currentEventObjectName = null; 
    public string currentLockObjectName = null;
    private float maxHeight = 550f;
    private float maxWidth = 890f;

    private void Awake() {
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
    }

    public void OnExitButtonClick() {
        if (isImageActive)
            SetObjectImageGroup(false);
        else if (isLockObjectActive)
            SetLockObject(false);

        RoomManager.Instance.SetIsInvestigating(isImageActive || isLockObjectActive);
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
            UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, true, true);
        }

        UIManager.Instance.SetUI(eUIGameObjectName.ObjectImageParentRoom, isTrue, true);
        
        if (!GetIsImageOrLockPanelActive())
            UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, false, true);
        currentEventObjectName = eventObjectName;

        GameManager.Instance.SetVariable("isImageActive", isImageActive);
        GameManager.Instance.SetVariable("currentEventObjectName", currentEventObjectName == null ? "NONE" : currentEventObjectName);

        SaveManager.Instance.SaveGameData();
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

        if (isTrue)
        {
            SetObjectImageGroup(false);

            if (!lockObjectDictionary.TryGetValue(lockObjectName, out var lockGO))
            {
                Debug.LogError($"[Lock] Unknown key: {lockObjectName}");
                return;
            }

            // 자식 포함 CanvasGroup 탐색 (비활성 자식까지)
            var cg = lockGO.GetComponentInChildren<CanvasGroup>(true);
            currentLockObjectCanvasGroup = cg;

            // 루트와 CG 보유 GO 모두 켜기
            lockGO.SetActive(true);
            cg.gameObject.SetActive(true);

            RoomManager.Instance.SetIsInvestigating(true);
            RoomManager.Instance.SetButtons();
            UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, true, true);
        }
        else
        {
            if (puzzleObjectDictionary.TryGetValue(currentLockObjectName, out var puzzleObjects))
            {
                foreach (var puzzleObject in puzzleObjects) UIManager.Instance.AnimateUI(puzzleObject, false, true);
                StartCoroutine(DeactivateLockObjectWithDelay(currentLockObjectName, UIManager.Instance.fadeAnimationDuration));
            }
            else if (lockObjectDictionary.TryGetValue(currentLockObjectName, out var lockGO))
            {
                var cg = lockGO.GetComponentInChildren<CanvasGroup>(true);
                if (cg != null) UIManager.Instance.AnimateUI(cg.gameObject, false, true);
                StartCoroutine(DeactivateLockObjectWithDelay(currentLockObjectName, UIManager.Instance.fadeAnimationDuration));
            }
            currentLockObjectCanvasGroup = null;

            if (!GetIsImageOrLockPanelActive())
                UIManager.Instance.SetUI(eUIGameObjectName.BlurImage, false, true);
        }
        currentLockObjectName = lockObjectName;

        GameManager.Instance.SetVariable("isLockObjectActive", isLockObjectActive);
        GameManager.Instance.SetVariable("currentLockObjectName", currentLockObjectName == null ? "NONE" : currentLockObjectName);

        SaveManager.Instance.SaveGameData();
    }
    
    private IEnumerator DeactivateLockObjectWithDelay(string lockObjectName, float delayTime = 0.5f) {
        yield return new WaitForSeconds(delayTime);
        lockObjectDictionary[lockObjectName].gameObject.SetActive(false);
    }

    public bool GetIsImageOrLockPanelActive() {
        return isImageActive || isLockObjectActive;
    }
}
