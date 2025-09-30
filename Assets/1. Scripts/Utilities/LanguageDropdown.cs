using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageDropdown : MonoBehaviour
{
	[SerializeField] private TMP_Dropdown tmpDropdown;

	private void Awake() {
		if (tmpDropdown == null)
			tmpDropdown = GetComponent<TMP_Dropdown>();
	}

	private void OnEnable() {
		if (tmpDropdown != null)
			tmpDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

		int current = LocalizationManager.Instance != null ? LocalizationManager.Instance.GetLanguage() : 0;
		if (tmpDropdown != null)
			tmpDropdown.SetValueWithoutNotify(Mathf.Clamp(current, 0, tmpDropdown.options.Count - 1));
	}

	private void OnDisable() {
		if (tmpDropdown != null)
			tmpDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
	}

	private void OnDropdownValueChanged(int optionIndex) {
		if (LocalizationManager.Instance != null)
			LocalizationManager.Instance.SetLanguage(optionIndex);
		else
			GameManager.Instance.SetVariable("Language", optionIndex);
	}
}


