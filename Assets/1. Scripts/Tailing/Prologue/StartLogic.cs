using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
public class StartLogic : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI nameCheckQuestion;
    [SerializeField] private TMP_Dropdown monthDropdown, dayDropdown;
    public void GoScene(int sceneNum)
    {
        SceneManager.LoadScene(sceneNum);
    }

    public void SetName()
    {
        Player.Instance.Name = nameInput.text == "" ? "필연" : nameInput.text;
        nameCheckQuestion.text = $"\"{Player.Instance.Name}\"으로 확정하시겠습니까?";
    }

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
        Player.Instance.Month = monthDropdown.value + 1;
        Player.Instance.Day = dayDropdown.value + 1;
    }
}
