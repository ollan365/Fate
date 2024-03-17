using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class Player : MonoBehaviour
{
    [SerializeField] private string playerName;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    [SerializeField] private TMP_Dropdown monthDropdown, dayDropdown;
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
    private int month, day;
    public int Month { get => month; }
    public int Day { get => day; }
    public void ChangeDayOption()
    {
        List<int> optionList = new();
        for (int i = 1; i <= 29; i++)
            optionList.Add(i);

        switch (monthDropdown.value + 1)
        {
            case 2:
                break;
            case 4:
            case 6:
            case 9:
            case 11:
                optionList.Add(30);
                break;
            default:
                optionList.Add(30);
                optionList.Add(31);
                break;
        }
    }
    public void SetBirth()
    {
        month = monthDropdown.value + 1;
        day = dayDropdown.value + 1;
    }
}
