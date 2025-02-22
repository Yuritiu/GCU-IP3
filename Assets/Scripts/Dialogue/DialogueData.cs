using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data", order = 1)]
public class DialogueData : ScriptableObject
{
    public string characterName;
    public string[] textLines;
    public Sprite[] linePortraits;
}
