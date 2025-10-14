using System;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
	public static LocalizationManager Instance { get; private set; }

	public event Action<int> OnLanguageChanged;

	[SerializeField] private int currentLanguage;
	public bool IsInitialized { get; private set; }

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		currentLanguage = (int)GameManager.Instance.GetVariable("Language");
		IsInitialized = true;
		OnLanguageChanged?.Invoke(currentLanguage);
	}

	public int GetLanguage() {
		return currentLanguage;
	}

	public void SetLanguage(int languageIndex) {
		if (currentLanguage == languageIndex)
			return;

		currentLanguage = languageIndex;
		GameManager.Instance.SetVariable("Language", currentLanguage);
		if (SaveManager.Instance != null)
			SaveManager.Instance.SaveVariable("Language");
		OnLanguageChanged?.Invoke(currentLanguage);
	}

	public void InitializeLanguageFromDeviceIfUnset() {
		int langValue = (int)GameManager.Instance.GetVariable("Language");
		if (langValue == -1)
			SetLanguage(Application.systemLanguage == SystemLanguage.Korean ? 1 : 0);
		else
			SetLanguage(langValue);
	}
}


