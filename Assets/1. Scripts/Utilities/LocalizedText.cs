using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedText : MonoBehaviour
{
	[SerializeField] private string scriptId;
	[SerializeField] private TextMeshProUGUI tmpText;

	private void Awake() {
		if (tmpText == null)
			tmpText = GetComponent<TextMeshProUGUI>();
	}

    private void OnEnable()	{
		if (LocalizationManager.Instance != null) {
			LocalizationManager.Instance.OnLanguageChanged += Apply;
			if (LocalizationManager.Instance.IsInitialized)
				Apply(LocalizationManager.Instance.GetLanguage());
		}
    }

	private void OnDisable() {
		if (LocalizationManager.Instance != null)
			LocalizationManager.Instance.OnLanguageChanged -= Apply;
	}

	public void SetScriptId(string newScriptId) {
		scriptId = newScriptId;
		int lang = LocalizationManager.Instance != null ? LocalizationManager.Instance.GetLanguage() : 0;
		Apply(lang);
	}

	private void Apply(int languageIndex) {
		if (string.IsNullOrEmpty(scriptId))
			return;

		if (DialogueManager.Instance == null || DialogueManager.Instance.scripts == null)
			return;

		if (DialogueManager.Instance.scripts.ContainsKey(scriptId) == false) {
			Debug.LogWarning($"[{gameObject.name}] LocalizedText: Script ID '{scriptId}' not found.");
			return;
		}

		string sentence = DialogueManager.Instance.scripts[scriptId].GetScript(languageIndex).ProcessedText;

		if (tmpText != null)
			tmpText.text = sentence;
	}
}


