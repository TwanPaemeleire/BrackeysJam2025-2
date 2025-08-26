using System.Collections.Generic;
using Assets.Dialogues;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights
{
    public class DialogueManager : MonoBehaviour
    {
        // Place for canvas variables

        private List<DialogueData> _possibleDialogues = new List<DialogueData>();
        private int _currentDialogueIndex = 0;

        public UnityEvent DialogueEndedEvent = new UnityEvent();

        private void Awake()
        {
            _possibleDialogues.Clear();

            // Load scriptable objects, excluding the one that is not used
        }

        private void StartDialogue()
        {

        }

        private void AdvanceDialogue()
        {

        }

        private void FinishDialogue()
        {
            DialogueEndedEvent?.Invoke();
        }
    }
}
