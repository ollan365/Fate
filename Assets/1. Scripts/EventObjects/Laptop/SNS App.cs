using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SNSApp : MonoBehaviour
{
    [SerializeField] private Transform contentPanel; // The content panel of the scroll view
    [SerializeField] private GameObject postPrefab; // The post prefab

    private readonly List<SnsPost> snsPosts = new List<SnsPost>();

    private void Start()
    {
        LoadSnsContents();
        DisplayPosts();
    }

    private void LoadSnsContents()
    {
        var snsCsv = Resources.Load<TextAsset>("Datas/laptop_SNS");
        if (snsCsv == null)
        {
            Debug.LogError("Failed to load laptop_SNS CSV file");
            return;
        }

        var lines = snsCsv.text.Split('\n');

        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var fields = lines[i].Split(',');

            if (fields.Length < 4)
            {
                Debug.LogWarning($"Invalid line in CSV: {lines[i]}\nlength: {fields.Length}");
                continue;
            }

            var post = new SnsPost
            {
                AppID = fields[0].Trim(),
                Description = fields[1].Trim(),
                Date = fields[2].Trim(),
                ScriptID = fields[3].Trim()
            };

            snsPosts.Add(post);
        }
    }

    private void DisplayPosts()
    {
        foreach (var post in snsPosts)
        {
            GameObject newPost = Instantiate(postPrefab, contentPanel);
            // newPost.transform.Find("AppIDText").GetComponent<TextMeshProUGUI>().text = post.AppID;
            // newPost.transform.Find("DescriptionText").GetComponent<TextMeshProUGUI>().text = post.Description;
            newPost.transform.Find("DateText").GetComponent<TextMeshProUGUI>().text += post.Date;
            
            // Assuming DialogueManager.Instance.scripts[post.ScriptID].GetScript() returns the script content
            if (DialogueManager.Instance.scripts.ContainsKey(post.ScriptID))
            {
                string scriptContent = DialogueManager.Instance.scripts[post.ScriptID].GetScript();
                newPost.transform.Find("ScriptText").GetComponent<TextMeshProUGUI>().text = scriptContent;
            }
            else
            {
                Debug.LogWarning($"Script ID '{post.ScriptID}' not found in DialogueManager scripts");
                newPost.transform.Find("ScriptText").GetComponent<TextMeshProUGUI>().text = "Script not found";
            }
        }
    }

    private class SnsPost
    {
        public string AppID;
        public string Description;
        public string Date;
        public string ScriptID;
    }
}
