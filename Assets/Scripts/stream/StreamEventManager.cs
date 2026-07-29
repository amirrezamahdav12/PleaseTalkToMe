using UnityEngine;

public class StreamEventManager : MonoBehaviour
{
    [SerializeField]
    private StreamStats stats;

    [SerializeField]
    private NotificationUI notification;


    public void NewFollower(string username)
    {
        stats.AddFollower(1);
        stats.AddViewer(1);


        notification.Show(
            "NEW FOLLOWER",
            username + " started following"
        );
    }
}