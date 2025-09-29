using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LocalizedImage : MonoBehaviour
{
	[SerializeField] private Sprite englishSprite;
	[SerializeField] private Sprite koreanSprite;
	[SerializeField] private Sprite japaneseMaleSprite;
	[SerializeField] private Sprite japaneseFemaleSprite;

	private Image targetImage;

	private void Awake()
	{
		targetImage = GetComponent<Image>();
	}

	private void OnEnable()
	{
		if (LocalizationManager.Instance != null)
		{
			LocalizationManager.Instance.OnLanguageChanged += Apply;
			Apply(LocalizationManager.Instance.GetLanguage());
		}
	}

	private void OnDisable()
	{
		if (LocalizationManager.Instance != null)
		{
			LocalizationManager.Instance.OnLanguageChanged -= Apply;
		}
	}

	private void Apply(int languageIndex)
	{
		Sprite next = null;
		switch (languageIndex)
		{
			case 0: next = englishSprite; break;
			case 1: next = koreanSprite; break;
			case 2: next = japaneseMaleSprite; break;
			case 3: next = japaneseFemaleSprite; break;
			default: next = koreanSprite; break;
		}

		if (next != null)
			targetImage.sprite = next;
	}
}


