using UnityEngine;

namespace Assets.Dialogues
{
    [System.Serializable]
    public struct DialogueLine
    {
        public Sprite characterPortrait;
        public string characterName;
        [TextArea] public string text;
    }

    [CreateAssetMenu(menuName = "Dialogue/DialogueData")]
    public class DialogueData : ScriptableObject
    {
        [SerializeField] private string dialogueID;

        public string DialogueID => dialogueID;

        public DialogueLine[] lines;
    }
}
