using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueDatabase", menuName = "SwitchPrime/Dialogue Database")]
public class DialogueDatabase : ScriptableObject
{
    public List<DialogueNode> nodes = new();
}