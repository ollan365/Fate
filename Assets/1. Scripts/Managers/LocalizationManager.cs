using System;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
	public static LocalizationManager Instance { get; private set; }

	public event Action<int> OnLanguageChanged;

	[SerializeField] private int currentLanguage;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	private void Start() {
		object langObj = GameManager.Instance.GetVariable("Language");
		currentLanguage = langObj is int ? (int)langObj : 1; // default to Korean index used in project
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
			SaveManager.Instance.SaveGameData();
		OnLanguageChanged?.Invoke(currentLanguage);
	}

	public void InitializeLanguageFromDeviceIfUnset() {
		int langValue = (int)GameManager.Instance.GetVariable("Language");
		if (langValue == -1) {
			Debug.Log("langValue is -1, systemLanguage: " + Application.systemLanguage);
			SetLanguage(Application.systemLanguage == SystemLanguage.Korean ? 1 : 0);
		}
		else
			SetLanguage(langValue);
	}
}


