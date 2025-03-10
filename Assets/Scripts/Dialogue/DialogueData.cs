using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data", order = 1)]
public class DialogueData : ScriptableObject
{
    public string initialCharacterName; 
    public Sprite initialSprite;

    public string[] characterLinesName;
    public string[] textLines;
    public Sprite[] linePortraits;
    public string[] cameraLinesTarget;
}
