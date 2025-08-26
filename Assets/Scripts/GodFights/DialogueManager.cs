using Assets.Dialogues;
using Assets.Scripts.General;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Scripts.GodFights
{
    public class DialogueManager : MonoSingleton<DialogueManager>
    {
        // Place for canvas variables
        [SerializeField] private GameObject _dialogueAssetParent;
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private TextMeshProUGUI _mainText;

        
        [SerializeField] private InputActionReference _advanceAction;
        [SerializeField] private PlayerInput _playerInput;

        private Dictionary<string, DialogueData> _dialogueMap;

        private DialogueData _currentDialogue;
        private int _dialogueLineIndex = 0;

        public UnityEvent DialogueEndedEvent = new UnityEvent();

        private void Awake()
        {
            _dialogueMap = new();
            foreach (var d in Resources.LoadAll<DialogueData>("Dialogues"))
            {
                _dialogueMap[d.DialogueID] = d;
            }

            //StartDialogue("Test");
        }

        //private void Start()
        //{
        //    _playerInput.SwitchCurrentActionMap("Dialogue");
        //}

        private void OnEnable()
        {
            if (_advanceAction != null)
            {
                _advanceAction.action.performed += OnAdvance;
            }
        }

        private void OnDisable()
        {
            if (_advanceAction != null)
            {
                _advanceAction.action.performed -= OnAdvance;
            }
        }

        private void OnAdvance(InputAction.CallbackContext ctx)
        {
            AdvanceDialogue();
        }

        private void StartDialogue(string dialogueId)
        {
            _dialogueAssetParent.SetActive(true);

            _currentDialogue = _dialogueMap[dialogueId];

            SetDialogueLineData();
        }

        private void AdvanceDialogue()
        {
            _dialogueLineIndex++;

            if (_dialogueLineIndex >= _currentDialogue.lines.Length)
            {
                FinishDialogue();
                return;
            }

            SetDialogueLineData();
        }

        private void FinishDialogue()
        {
            _dialogueAssetParent.SetActive(false);
            //_playerInput.SwitchCurrentActionMap("Gameplay");
            DialogueEndedEvent?.Invoke();
        }

        private void SetDialogueLineData()
        {
            var currentLine = _currentDialogue.lines[_dialogueLineIndex];

            _portrait.sprite = currentLine.characterPortrait;
            _characterName.text = currentLine.characterName;
            _mainText.text = currentLine.text;
        }
    }
}
