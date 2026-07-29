using System;

[Serializable]
public class ChatMessage
{
    public string username;

    public string message;

    public ViewerType viewerType;

    public bool isDonation;

    public int donationAmount;
}