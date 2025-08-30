using System.Collections;
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
        [SerializeField] private FadeInUIHandler _dialogueAssetParent;
        [SerializeField] private Image _portrait;
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private TextMeshProUGUI _mainText;

        [SerializeField] private GameObject _blurCanvas;

        [SerializeField] private InputActionReference _advanceAction;
        [SerializeField] private PlayerInput _playerInput;

        [SerializeField] private AudioClip _advanceAudioClip;
        [SerializeField] private float _advanceAudioClipVolume;

        private Dictionary<string, DialogueData> _dialogueMap;

        private DialogueData _currentDialogue;
        private int _dialogueLineIndex = 0;

        public UnityEvent DialogueEndedEvent = new UnityEvent();

        [SerializeField] private float _delay = 0.04f;
        private bool _isTyping = false;

        private void Awake()
        {
            _dialogueMap = new();
            foreach (var d in Resources.LoadAll<DialogueData>("Dialogues"))
            {
                _dialogueMap[d.DialogueID] = d;
            }
        }

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
            if (_isTyping)
            {
                StopAllCoroutines();
                var currentLine = _currentDialogue.lines[_dialogueLineIndex];
                _mainText.text = currentLine.text;
                _isTyping = false;
            }
            else
            {
                AdvanceDialogue();
            }
        }

        public void StartDialogue(string dialogueId)
        {
            if (!_dialogueMap.ContainsKey(dialogueId))
            {
                Debug.LogWarning("There is no dialogue called: " + dialogueId);
                return;
            }

            _blurCanvas.SetActive(true);

            _dialogueLineIndex = 0;

            _playerInput.SwitchCurrentActionMap("Dialogue");
            _dialogueAssetParent.StartFadingIn();
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
            SoundManager.Instance.PlaySFX(_advanceAudioClip, _advanceAudioClipVolume);
            SetDialogueLineData();
        }

        private void FinishDialogue()
        {
            Time.timeScale = 1f;

            _blurCanvas.SetActive(false);

            _dialogueAssetParent.StartFadingOut();
            _playerInput.SwitchCurrentActionMap("Gameplay");
            DialogueEndedEvent?.Invoke();
        }

        private void SetDialogueLineData()
        {
            var currentLine = _currentDialogue.lines[_dialogueLineIndex];

            _portrait.sprite = currentLine.characterPortrait;
            _characterName.text = currentLine.characterName;
            StartCoroutine(TypeText(currentLine.text));
        }

        private IEnumerator TypeText(string fullText)
        {
            _isTyping = true;

            _mainText.text = "";
            foreach (char c in fullText)
            {
                _mainText.text += c;
                yield return new WaitForSeconds(_delay);
            }

            _isTyping = false;
        }
    }
}
