using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ImageAndLockPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject blockingPanel;
    [SerializeField] private GameObject objectImageGroup;
    [SerializeField] private GameObject objectImage;
    private Image objectImageImageComponent;
    private RectTransform objectImageRectTransform;
    
    [Header("이벤트 오브젝트 확대 이미지")]
    [SerializeField] private Sprite amuletImage;
    [SerializeField] private Sprite carpetPaperImage;
    [SerializeField] private Sprite clockImage;
    [SerializeField] private Sprite keysImage;
    [SerializeField] private Sprite knifeImage;
    [SerializeField] private Sprite posterImage;
    [SerializeField] private Sprite liquorAndPillsImage;
    [SerializeField] private Sprite letterOfResignationImage;
    [SerializeField] private Sprite photoInsideBoxImage;

    [Header("잠금 장치들")]
    [SerializeField] private GameObject laptopGameObject; 
    [SerializeField] private GameObject clockGameObject;
    [SerializeField] private GameObject diaryGameObject;
    [SerializeField] private GameObject calendarGameObject;

    private Dictionary<string, Sprite> imageDictionary;
    private Dictionary<string, GameObject> lockObjectDictionary;
    [SerializeField] private bool isImageActive = false;
    [SerializeField] private bool isLockObjectActive = false;
    private string currentLockObjectName = null;

    private void Awake()
    {
        objectImageImageComponent = objectImage.GetComponent<Image>();
        objectImageRectTransform = objectImage.GetComponent<RectTransform>();
        
        imageDictionary = new Dictionary<string, Sprite>()  // imageDictionary 초기화
        {
            {"amulet", amuletImage},
            {"carpetPaper", carpetPaperImage},
            {"clock", clockImage},
            {"keys", keysImage },
            {"knife", knifeImage },
            {"poster", posterImage },
            {"liquorAndPills", liquorAndPillsImage },
            {"letterOfResignation", letterOfResignationImage},
            {"photoInsideBox", photoInsideBoxImage},
        };

        lockObjectDictionary = new Dictionary<string, GameObject>()
        {
            { "laptop", laptopGameObject },
            { "clock", clockGameObject },
            { "diary", diaryGameObject },
            { "calendar", calendarGameObject },
        };
    }

    public void OnExitButtonClick()
    {
        if (isImageActive)
        {
            SetObjectImageGroup(false);
            
            bool isImageOrLockActive = isImageActive || isLockObjectActive;
            RoomManager.Instance.SetIsInvestigating(isImageOrLockActive);
            
            return;
        }

        if (isLockObjectActive)
        {
            SetLockObject(false);
            
            bool isImageOrLockActive = isImageActive || isLockObjectActive;
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
            float maxDimension = 900f;

            float rawHeight = rawSprite.rect.height;
            float rawWidth = rawSprite.rect.width;
            float preferredHeight;
            float preferredWidth;
            float multiplier;

            if (rawHeight > rawWidth)
            {
                multiplier = maxDimension / rawHeight;
                preferredHeight = maxDimension;
                preferredWidth = rawWidth * multiplier;
            }
            else
            {
                multiplier = maxDimension / rawWidth;
                preferredWidth = maxDimension;
                preferredHeight = rawHeight * multiplier;
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
    
    private void SetBlockingPanel()
    {
        bool isImageOrLockActive = isImageActive || isLockObjectActive;
        blockingPanel.SetActive(isImageOrLockActive);
    }
}
