using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private InputField nameInput;
    [SerializeField] private Text nameCheckQuestion;
    public string Name
    {
        get => playerName;
        set
        {
            playerName = nameInput.text == "" ? "�ʿ�" : nameInput.text;
            nameCheckQuestion.text = $"\"{playerName}\"���� Ȯ���Ͻðڽ��ϱ�?";
        }
    }
    private int language;
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
