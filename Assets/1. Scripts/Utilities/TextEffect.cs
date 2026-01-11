using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using TMPro;
using System;


namespace Fate.Utilities
{
    public class TextEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
    	[System.Serializable]
    	private class OnClickEvent : UnityEvent { }

    	// Text UI를 클릭했을 때 호출하고 싶은 함수를 등록 할 수 있습니다
    	[SerializeField] private OnClickEvent onClickEvent;

        [SerializeField] private bool onPointEffect;

        // 랜덤 효과 (우연이 무섭게 말할 때 효과)
        [SerializeField] private bool randomEffect;
        [SerializeField] private float jitterAmount = 10.0f;
        [SerializeField] private float glitchSpeed = 5.0f;

        // 색상이 바뀌고, 터치가 되는 TextMeshProGUI
        private TextMeshProUGUI text;

    	private void Awake()
    	{
    		text = GetComponentInChildren<TextMeshProUGUI>();
    	}
        private void Update()
        {
            if (randomEffect)
            {
                text.ForceMeshUpdate(); // 텍스트 정보 최신화
                TMP_TextInfo textInfo = text.textInfo;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    var charInfo = textInfo.characterInfo[i];
                    if (!charInfo.isVisible)
                        continue;

                    int vertexIndex = charInfo.vertexIndex;
                    int materialIndex = charInfo.materialReferenceIndex;

                    Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                    // 랜덤 오프셋 생성
                    Vector3 offset = new Vector3(
                        Mathf.PerlinNoise(i, Time.time * glitchSpeed) - 0.5f,
                        Mathf.PerlinNoise(i + 100, Time.time * glitchSpeed) - 0.5f,
                        0f
                    ) * jitterAmount;

                    // 각 정점 위치에 오프셋 적용
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex + j] += offset;
                    }
                }

                // 변경된 정점 반영
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    text.UpdateGeometry(meshInfo.mesh, i);
                }
            }
        }

        // 마우스를 올렸을 때 글씨가 Bold체로 바뀝니다
        public void OnPointerEnter(PointerEventData eventData)
    	{
    		if(onPointEffect) text.fontStyle = FontStyles.Bold;
    	}

    	// 마우스를 뗐을 때 때 글씨가 Normal체로 바뀝니다
    	public void OnPointerExit(PointerEventData eventData)
    	{
            if (onPointEffect) text.fontStyle = FontStyles.Normal;
    	}

    	// 클릭했을 때 등록된 함수들을 호출합니다
    	public void OnPointerClick(PointerEventData eventData)
    	{
            if (onPointEffect) onClickEvent?.Invoke();
    	}

        public void Typing(char c)
        {
            if (randomEffect)
            {
                text.text += RandomTextTyping(c);
            }
            else
            {
                text.text += c.ToString();
            }
        }
        private string RandomTextTyping(char c)
        {
            string result = c.ToString();

            if (char.IsWhiteSpace(c))
            {
                return c.ToString();
            }

            System.Random rand = new System.Random();

            // 볼드 적용 여부 (25% 확률)
            if (rand.Next(100) < 25)
                result = $"<b>{result}</b>";

            // 이탤릭 적용 여부 (25% 확률)
            if (rand.Next(100) < 25)
                result = $"<i>{result}</i>";

            // 밑줄 적용 여부 (25% 확률)
            if (rand.Next(100) < 25)
                result = $"<u>{result}</u>";

            // 사이즈 선택 (80%, 90%, 100%, 110%, 120%)
            int sizeRoll = rand.Next(100);
            string sizeTag;

            if (sizeRoll < 40) sizeTag = "100%";     // 40%
            else if (sizeRoll < 55) sizeTag = "80%"; // 15%
            else if (sizeRoll < 70) sizeTag = "90%"; // 15%
            else if (sizeRoll < 85) sizeTag = "110%";// 15%
            else sizeTag = "120%";                   // 15%

            if (sizeTag != "100%") // 100%일 때는 태그 안 붙이기
                result = $"<size={sizeTag}>{result}</size>";

            return result;
        }
    }
}
