using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatApp : MonoBehaviour
{
    [SerializeField] private Transform conversationList; // Assign the conversation list GameObject in the inspector
    [SerializeField] private GameObject conversationPrefab; // Assign the conversation prefab in the inspector

    private readonly Dictionary<string, List<Message>> conversations = new Dictionary<string, List<Message>>();

    private void Start()
    {
        LoadConversations();
        DisplayConversationsList();
    }

    private void LoadConversations()
    {
        var chatCsv = Resources.Load<TextAsset>("Datas/laptop_Chat");

        if (chatCsv == null)
        {
            Debug.LogError("Failed to load laptop_Chat CSV file");
            return;
        }

        var lines = chatCsv.text.Split('\n');
        
        var previousConversationID = "";
        var previousMessage = new Message();
        
        for (var i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var fields = lines[i].Split(',');

            if (fields.Length < 6)
            {
                Debug.LogWarning($"Invalid line in CSV: {lines[i]}\nlength: {fields.Length}");
                continue;
            }

            var currentConversationID = fields[0].Trim();
            var currentMessage = new Message
            {
                Date = fields[2].Trim(),
                Time = fields[3].Trim(),
                SpeakerID = fields[4].Trim(),
                ScriptID = fields[5].Trim()
            };

            if (string.IsNullOrWhiteSpace(currentConversationID)) currentConversationID = previousConversationID;
            if (string.IsNullOrWhiteSpace(currentMessage.Date)) currentMessage.Date = previousMessage.Date;
            if (string.IsNullOrWhiteSpace(currentMessage.Time)) currentMessage.Time = previousMessage.Time;
            if (string.IsNullOrWhiteSpace(currentMessage.SpeakerID)) currentMessage.SpeakerID = previousMessage.SpeakerID;

            // add message to conversation
            if (conversations.ContainsKey(currentConversationID))
                conversations[currentConversationID].Add(currentMessage);
            else
                conversations.Add(currentConversationID, new List<Message> { currentMessage });

            previousConversationID = currentConversationID;
            previousMessage = currentMessage;
        }
    }

    private void DisplayConversationsList()
    {
        // Flatten the dictionary and order messages by date and time in ascending order
        var orderedConversations = conversations.OrderBy(c => c.Value.Max(m => DateTime.ParseExact(m.Date + " " + m.Time, "yy.MM.dd. HH:mm", null)));

        foreach (var conversation in orderedConversations)
        {
            GameObject newConversation = Instantiate(conversationPrefab, conversationList);
            TextMeshProUGUI conversationIDText = newConversation.transform.Find("ConversationIDText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateText = newConversation.transform.Find("DateText").GetComponent<TextMeshProUGUI>();

            // Display the SpeakerID if it's not "DialogueC_002"
            var displaySpeakerID = conversation.Value.FirstOrDefault(m => m.SpeakerID != "DialogueC_002")?.SpeakerID;

            conversationIDText.text = displaySpeakerID ?? conversation.Key;
            dateText.text = conversation.Value.Count > 0 ? conversation.Value[0].Date : "Unknown Date";

            // Log the conversation for debugging purposes
            // Debug.Log($"Conversation ID: {conversation.Key}, Display Speaker: {conversationIDText.text}, Date: {dateText.text}");
            // foreach (var message in conversation.Value)
            // {
            //     Debug.Log($"  Date: {message.Date}, Time: {message.Time}, Speaker: {message.SpeakerID}, Script ID: {message.ScriptID}");
            // }
        }
    }

}

public class Message
{
    public string Date;
    public string Time;
    public string SpeakerID;
    public string ScriptID;
}
