using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatApp : MonoBehaviour
{
    [SerializeField] private Transform conversationList;
    [SerializeField] private GameObject conversationPrefab;
    [SerializeField] private Transform messageList;
    [SerializeField] private GameObject messageParent;
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private GameObject messagePrefabLeft;
    [SerializeField] private GameObject messagePrefabRight;
    [SerializeField] private GameObject datePrefab;

    private readonly Dictionary<string, List<Message>> conversations = new Dictionary<string, List<Message>>();

    private string currentConversationKey;

    private void Start()
    {
        LoadConversations();
        DisplayConversationsList();
    }

    public void EnableExitButton()
    {
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, true);
    }
    
    public void DisableExitButton()
    {
        UIManager.Instance.SetUI(eUIGameObjectName.ExitButton, false);
    }

    public void OnEnable()
    {
        RoomManager.Instance.isLaptopOpen = true;
    }
    
    public void OnDisable()
    {
        RoomManager.Instance.isLaptopOpen = false;
    }

    private void LoadConversations()
    {
        messageParent.SetActive(false);

        var chatCsv = Resources.Load<TextAsset>("Datas/laptop_chat");

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
            if (string.IsNullOrWhiteSpace(lines[i])) continue; // Skip empty lines

            var fields = lines[i].Split(',');

            if (fields.Length < 6 || string.IsNullOrWhiteSpace(fields[5]))
            {
                // Debug.LogWarning($"Invalid or empty script line in CSV: {lines[i]}");
                continue;
            }

            var currentConversationID = fields[0].Trim();
            var currentMessage = new Message
            {
                Date = fields[2].Trim(),
                Time = fields[3].Trim(),
                SpeakerName = DialogueManager.Instance.scripts[fields[4].Trim()].GetScript().ProcessedText,
                ScriptContent = DialogueManager.Instance.scripts[fields[5].Trim()].GetScript().ProcessedText
            };

            if (string.IsNullOrWhiteSpace(currentConversationID)) currentConversationID = previousConversationID;
            if (string.IsNullOrWhiteSpace(currentMessage.Date)) currentMessage.Date = previousMessage.Date;
            if (string.IsNullOrWhiteSpace(currentMessage.Time)) currentMessage.Time = previousMessage.Time;
            if (string.IsNullOrWhiteSpace(currentMessage.SpeakerName)) currentMessage.SpeakerName = previousMessage.SpeakerName;

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
        // Order conversations by the latest message in each conversation (in ascending order)
        var orderedConversations = conversations
            .OrderByDescending(c => c.Value.Max(m => DateTime.ParseExact(m.Date + " " + m.Time,
                "yy.MM.dd. HH:mm",
                null)));

        foreach (var conversation in orderedConversations)
        {
            GameObject newConversation = Instantiate(conversationPrefab, conversationList);
            
            GameObject conversationContents = newConversation.transform.Find("contents").gameObject;
            
            TextMeshProUGUI conversationIDText = conversationContents.transform.Find("ConversationIDText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI dateText = conversationContents.transform.Find("DateText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI latestMessageText = conversationContents.transform.Find("LatestMessageText").GetComponent<TextMeshProUGUI>();

            // Display the SpeakerID if it's not "DialogueC_002"
            var displaySpeakerID = conversation.Value.FirstOrDefault(m => m.SpeakerName != "우연")?.SpeakerName;

            // Get the latest message in the conversation
            var latestMessage = conversation.Value.OrderByDescending(m => DateTime.ParseExact(m.Date + " " + m.Time,
                    "yy.MM.dd. HH:mm",
                    null))
                .First();

            conversationIDText.text = displaySpeakerID ?? conversation.Key;
            latestMessageText.text = latestMessage.ScriptContent;
            
            DateTime parsedDate = DateTime.ParseExact(latestMessage.Date, "yy.MM.dd.", null);
            string formattedDate = parsedDate.ToString("MMM dd");
            dateText.text = formattedDate;

            // Add a click listener to the conversation button
            Button conversationButton = newConversation.GetComponent<Button>();
            if (conversationButton != null)
            {
                string conversationKey = conversation.Key;
                conversationButton.onClick.AddListener(() => OnConversationClicked(conversationKey));
            }
        }
    }

    private void OnConversationClicked(string conversationKey)
    {
        if (!conversations.ContainsKey(conversationKey)) return;

        if (currentConversationKey == conversationKey) return;
        currentConversationKey = conversationKey;

        messageParent.SetActive(true);
        senderText.text = conversations[conversationKey]
                              .FirstOrDefault(m => m.SpeakerName != "우연")
                              ?.SpeakerName ??
                          conversationKey;
        
        // Clear previous messages
        foreach (Transform child in messageList)
        {
            Destroy(child.gameObject);
        }

        // Get the messages for the selected conversation and order by datetime ascending
        var messages = conversations[conversationKey]
            .OrderBy(m => DateTime.ParseExact(m.Date + " " + m.Time, "yy.MM.dd. HH:mm", null));

        string previousDate = null;

        foreach (var message in messages)
        {
            // Check if the date has changed and instantiate a date prefab if it has
            if (previousDate != message.Date)
            {
                GameObject newDate = Instantiate(datePrefab, messageList);
                TextMeshProUGUI dateText = newDate.GetComponentInChildren<TextMeshProUGUI>();

                DateTime parsedDate = DateTime.ParseExact(message.Date, "yy.MM.dd.", null);
                string formattedDate = parsedDate.ToString("MMM dd");

                dateText.text = formattedDate;

                previousDate = message.Date;
            }

            // Instantiate the appropriate message prefab (left or right)
            GameObject messagePrefab = message.SpeakerName == "우연" ? messagePrefabRight : messagePrefabLeft;
            GameObject newMessage = Instantiate(messagePrefab, messageList);
            // GameObject messageParentGameObject = newMessage.transform.Find("Message Parent").gameObject;
            GameObject messageMidParent = newMessage.transform.Find("Message Mid Parent").gameObject;
            GameObject messageBottomParent = newMessage.transform.Find("Message Bottom Parent").gameObject;
            // TextMeshProUGUI speakerText = messageMidParent.transform.Find("SpeakerText").GetComponent<TextMeshProUGUI>();
            GameObject messageMidBackgroundImage = messageMidParent.transform.Find("Background Image").gameObject;
            TextMeshProUGUI messageText = messageMidBackgroundImage.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = messageBottomParent.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();

            // speakerText.text = message.SpeakerName;
            messageText.text = message.ScriptContent;
            timeText.text = message.Time;
        }
        messageList.GetComponent<VerticalLayoutGroup>().spacing = 10;
    }
}

public class Message
{
    public string Date;
    public string Time;
    public string SpeakerName;
    public string ScriptContent;
}
