using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;

public class UITextInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[System.Serializable]
	private class OnClickEvent : UnityEvent { }

	// Text UI�� Ŭ������ �� ȣ���ϰ� ���� �Լ��� ��� �� �� �ֽ��ϴ�
	[SerializeField] private OnClickEvent onClickEvent;

	// ������ �ٲ��, ��ġ�� �Ǵ� TextMeshProGUI
	private TextMeshProUGUI text;

	private void Awake()
	{
		text = GetComponent<TextMeshProUGUI>();
	}

	// ���콺�� �÷��� �� �۾��� Boldü�� �ٲ�ϴ�
	public void OnPointerEnter(PointerEventData eventData)
	{
		text.fontStyle = FontStyles.Bold;
	}

	// ���콺�� ���� �� �� �۾��� Normalü�� �ٲ�ϴ�
	public void OnPointerExit(PointerEventData eventData)
	{
		text.fontStyle = FontStyles.Normal;
	}

	// Ŭ������ �� ��ϵ� �Լ����� ȣ���մϴ�
	public void OnPointerClick(PointerEventData eventData)
	{
		onClickEvent?.Invoke();
	}
}

