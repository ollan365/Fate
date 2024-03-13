using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Player : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    public string Name
    {
        get => playerName;
        set
        {
            playerName = nameInput.text == "" ? "필연" : nameInput.text;
            nameCheckQuestion.text = $"\"{playerName}\"으로 확정하시겠습니까?";
        }
    }
    private int language = 1;
    public int Language
    {
        get => language;
        set => language = value;
    }
    private int gender;
    public int Gender
    {
        get => gender;
        set
        {
            gender = value;
            if (language == 2 && gender == 1) language++;
        }
    }
}
