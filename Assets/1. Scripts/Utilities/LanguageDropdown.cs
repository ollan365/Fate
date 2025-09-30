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
		if (LocalizationManager.Instance != null)
			LocalizationManager.Instance.OnLanguageChanged += SyncFromLanguage;

		if (LocalizationManager.Instance != null)
			SyncFromLanguage(LocalizationManager.Instance.GetLanguage());
	}

	private void OnDisable() {
		if (tmpDropdown != null)
			tmpDropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
		if (LocalizationManager.Instance != null)
			LocalizationManager.Instance.OnLanguageChanged -= SyncFromLanguage;
	}

	private void OnDropdownValueChanged(int optionIndex) {
		if (LocalizationManager.Instance != null)
			LocalizationManager.Instance.SetLanguage(optionIndex);
		else
			GameManager.Instance.SetVariable("Language", optionIndex);
	}

	private void SyncFromLanguage(int languageIndex)
	{
		if (tmpDropdown == null) return;
		int clamped = Mathf.Clamp(languageIndex, 0, tmpDropdown.options.Count - 1);
		tmpDropdown.SetValueWithoutNotify(clamped);
		tmpDropdown.RefreshShownValue();
	}
}


