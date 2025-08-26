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

    public class DialogueData : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}
