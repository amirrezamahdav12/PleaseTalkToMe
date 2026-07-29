using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class ChoiceData
{
    public string text;

    public string nextNodeID;


    [Header("Stats Effect")]

    public float hopeChange;

    public float stressChange;

    public float trustChange;



    [Header("Time Effect")]

    public float timeEffect;



    [Header("Chat Reaction")]

    public List<ChatMessageData> responseChat;
}