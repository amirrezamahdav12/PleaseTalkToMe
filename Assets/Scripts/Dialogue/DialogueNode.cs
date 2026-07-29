using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueNode", menuName = "SwitchPrime/Dialogue Node")]
public class DialogueNode : ScriptableObject
{
    [Header("General")]

    public string nodeID;

    public string speaker;

    [TextArea(3, 10)]
    public string dialogue;

    public Sprite portrait;

    [Space]

    public List<ChoiceData> choices = new();

    public enum Emotion
    {
        Neutral,
        Happy,
        Thinking,
        Sad,
        Crying,
        Angry,
        Tired,
        Hopeless
    }

    public Emotion emotion;

    public string eventID;
}