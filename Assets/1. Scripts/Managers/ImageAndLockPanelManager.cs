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
    [SerializeField] private GameObject objectImageGroup;
    [SerializeField] private GameObject objectImage;
    private Image objectImageImageComponent;
    private RectTransform objectImageRectTransform;

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

    [Header("방탈출2 이벤트 오브젝트 확대 이미지")]
    [SerializeField] private Sprite tinCaseImage;
    [SerializeField] private Sprite sewingBoxImage;
    [SerializeField] private Sprite threadImage;
    [SerializeField] private Sprite hospitalPrintImage;
    [SerializeField] private Sprite poster2Image;
    [SerializeField] private Sprite starStickerImage;
    [SerializeField] private Sprite shoppingBag2Image;


    [Header("잠금 장치들")]
    [SerializeField] private GameObject laptopGameObject;
    [SerializeField] private GameObject clockGameObject;
    [SerializeField] private GameObject diaryGameObject;
    [SerializeField] private GameObject calendarGameObject;
    [SerializeField] private GameObject tinCaseGameObject;
    [SerializeField] private GameObject sewingBoxGameObject;

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
    private Dictionary<string, GameObject> TutorialimageDictionary;
    [SerializeField] public bool isImageActive = false;
    [SerializeField] private bool isLockObjectActive = false;
    [SerializeField] private bool isTutorialObjectActive = false;
    private string currentLockObjectName = null;

    private void Awake()
    {
        objectImageImageComponent = objectImage.GetComponent<Image>();
        objectImageRectTransform = objectImage.GetComponent<RectTransform>();

        imageDictionary = new Dictionary<string, Sprite>()  // imageDictionary 초기화
        {
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
            // 방탈출2 확대 이미지
            {"tinCase", tinCaseImage},
            {"sewingBox", sewingBoxImage},
            {"thread", threadImage},
            {"hospitalPrint", hospitalPrintImage},
            {"poster2", poster2Image},
            {"starSticker", starStickerImage},
            {"shoppingBag2", shoppingBag2Image},
        };

        lockObjectDictionary = new Dictionary<string, GameObject>()
        {
            { "laptop", laptopGameObject },
            { "clock", clockGameObject },
            { "diary", diaryGameObject },
            { "calendar", calendarGameObject },
            { "tinCase", tinCaseGameObject },
            {"sewingBox", sewingBoxGameObject},
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

    public bool GetIsTutorialObjectActive()
    {
        return isTutorialObjectActive;
    }

    public void OnExitButtonClick()
    {
        if (isImageActive)
        {
            SetObjectImageGroup(false);

            bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);

            return;
        }

        if (isLockObjectActive)
        {
            SetLockObject(false);

            bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);
        }

        if (isTutorialObjectActive)
        {
            if (TutorialBlockingPanel.activeSelf && currentLockObjectName == null)
            {
                SetTutorialBlockingPanel(false);
            }
            else
            {
                SetTutorialImageObject(false);
            }

            bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);
        }
    }

    public void SetObjectImageGroup(bool isTrue, string eventObjectName = null)
    {
        if (isTrue && eventObjectName == null)
        {
            Debug.Log("eventObjectName must be a correct value!");
            return;
        }

        isImageActive = isTrue;

        if (isTrue)
        {
            Sprite rawSprite = imageDictionary[eventObjectName];
            float maxHeight = 1200f;
            float maxWidth = 1900f;

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

        objectImageGroup.SetActive(isTrue);
        SetBlockingPanel();
    }


    public void SetLockObject(bool isTrue, string lockObjectName = null)
    {
        if (isTrue && lockObjectName == null)
        {
            Debug.Log("lockObjectName must be a correct value!");
            return;
        }

        isLockObjectActive = isTrue;

        if (isTrue)
        {
            SetObjectImageGroup(false);  // 이미지 켜져있을 때 Lock object activate하면 이미지 숨기기
            lockObjectDictionary[lockObjectName].gameObject.SetActive(true);

            RoomManager.Instance.SetIsInvestigating(true);
            RoomManager.Instance.SetButtons();
        }
        else lockObjectDictionary[currentLockObjectName].gameObject.SetActive(false);
        currentLockObjectName = lockObjectName;

        SetBlockingPanel();
    }

    public void SetTutorialImageObject(bool isTrue, string tutorialImageObjectName = null)
    {
        if (isTrue && tutorialImageObjectName == null)
        {
            Debug.Log("lockObjectName must be a correct value!");
            return;
        }

        isTutorialObjectActive = isTrue;

        if (isTrue)
        {
            SetObjectImageGroup(false);  // 이미지 켜져있을 때 Lock object activate하면 이미지 숨기기

            if (tutorialImageObjectName == "TutorialMoveButton")
            {
                TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(isTrue);
                TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(isTrue);

                if (RoomManager.Instance.currentSideIndex == 1) TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(false);
                else if (RoomManager.Instance.currentSideIndex == 2) TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(false);
            }
            else
            {
                TutorialimageDictionary[tutorialImageObjectName].gameObject.SetActive(true);
            }

            RoomManager.Instance.SetIsInvestigating(true);
            //RoomManager.Instance.SetButtons();
        }
        else
        {
            if (currentLockObjectName == "TutorialMoveButton")
            {
                TutorialimageDictionary["TutorialRightMoveButton"].gameObject.SetActive(false);
                TutorialimageDictionary["TutorialLeftMoveButton"].gameObject.SetActive(false);
            }
            else
            {
                TutorialimageDictionary[currentLockObjectName].gameObject.SetActive(false);
            }
        }
        currentLockObjectName = tutorialImageObjectName;

        SetTutorialBlockingPanel();
    }

    public void SetBlockingPanel()
    {
        bool isTutorial = (bool)GameManager.Instance.GetVariable("isTutorial");

        bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorial;
        blockingPanel.SetActive(isImageOrLockActive);

        // 튜토리얼이 아니면 BlockingPanel 알파값 조절 가능
        if (!(bool)GameManager.Instance.GetVariable("isTutorial"))
        {
            float alpha = isImageOrLockActive ? 0.7f : 0;  // rgba의 알파값(투명도)
            blockingPanel.GetComponent<Image>().color = new Color(0, 0, 0, alpha);
        }
    }

    public void SetTutorialBlockingPanel()
    {
        bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
        TutorialBlockingPanel.SetActive(isImageOrLockActive);
    }

    public void SetTutorialBlockingPanel(bool isTrue)
    {
        isTutorialObjectActive = isTrue;
        RoomManager.Instance.SetIsInvestigating(true);
        //RoomManager.Instance.SetButtons();

        bool isImageOrLockActive = isImageActive || isLockObjectActive || isTutorialObjectActive;
        TutorialBlockingPanel.SetActive(isImageOrLockActive);
    }
}
