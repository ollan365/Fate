using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class UITextInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[System.Serializable]
	private class OnClickEvent : UnityEvent { }

	// Text UI를 클릭했을 때 호출하고 싶은 함수를 등록 할 수 있습니다
	[SerializeField] private OnClickEvent onClickEvent;

	// 색상이 바뀌고, 터치가 되는 TextMeshProGUI
	private TextMeshProUGUI text;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	// 마우스를 올렸을 때 글씨가 Bold체로 바뀝니다
	public void OnPointerEnter(PointerEventData eventData)
	{
		text.fontStyle = FontStyles.Bold;
	}

	// 마우스를 뗐을 때 때 글씨가 Normal체로 바뀝니다
	public void OnPointerExit(PointerEventData eventData)
	{
		text.fontStyle = FontStyles.Normal;
	}

	// 클릭했을 때 등록된 함수들을 호출합니다
	public void OnPointerClick(PointerEventData eventData)
	{
		onClickEvent?.Invoke();
	}
}

