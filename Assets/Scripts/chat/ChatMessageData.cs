using System;
using UnityEngine;

[Serializable]
public class ChatMessageData
{
    public string username;

    [TextArea(1, 3)]
    public string message;

    public ViewerType viewerType;

    public float delay;

    public bool newViewer;
}