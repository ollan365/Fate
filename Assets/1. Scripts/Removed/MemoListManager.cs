using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MemoListManager : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform scrollViewContent;

    void Start()
    {
        PopulateMemoList();
    }

    void PopulateMemoList()
    {
        foreach (var memoID in MemoManager.Instance.allMemo.Keys)
        {
            GameObject buttonObj = Instantiate(buttonPrefab, scrollViewContent);
            buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = memoID;
            buttonObj.GetComponent<Button>().onClick.AddListener(() => MemoManager.Instance.AddMemo(memoID));
        }
    }
}