using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "SwitchPrime/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Header("General")]
    public string nodeID;

    [HideInInspector] public Vector2 nodePosition = new(100, 100);

    public string notificationText;

    public string speaker;

    [TextArea(3, 10)]
    public string dialogue;

    public OmidMood mood = OmidMood.Natural;

    [Header("Node Settings")]
    public bool isEndingNode;

    [TextArea]
    public string endingMessage;

    [Header("Chat")]
    public List<ChatMessageData> chatMessages = new();

    [Header("Choices")]
    public List<ChoiceData> choices = new();

    [Header("Events")]
    public string eventID;
}