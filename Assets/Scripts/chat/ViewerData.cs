using UnityEngine;

[CreateAssetMenu(menuName = "SwitchPrime/Viewer")]
public class ViewerData : ScriptableObject
{
    public string viewerName;

    public Sprite avatar;

    public ViewerType viewerType;
}