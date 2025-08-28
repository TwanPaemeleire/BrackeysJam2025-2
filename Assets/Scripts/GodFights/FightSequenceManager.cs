using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.General;
using UnityEngine.Events;
using Assets.Scripts.Player;
using UnityEngine.InputSystem;

namespace Assets.Scripts.GodFights
{
    [System.Serializable]
    public class GodInfo
    {
        public GenericGodFight Fight;
        public GameObject ThroneObject;
    }

    public class FightSequenceManager : MonoSingleton<FightSequenceManager>
    {
        [SerializeField] private Transform _godFightSpawnPoint;
        [SerializeField] private List<GodInfo> _allGods = new List<GodInfo>();
        [SerializeField] private GodType _lover;
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private Transform _playerRespawnPoint;
        [SerializeField] private GameObject _defeatedUI;
        [SerializeField] private GameObject _blurObject;
        private GenericGodFight _currentFightGod;
        int _currentGodFightIdx = 0;

        public GameObject PlayerObject { get { return _playerObject; } }

        public UnityEvent OnCurrentGodDefeated = new UnityEvent();
        public UnityEvent OnCurrentGodFightStarted = new UnityEvent();
        public UnityEvent OnAllGodsDefeated = new UnityEvent();

        protected override void Init()
        {
            _playerObject.GetComponent<PlayerHealth>().OnDeath.AddListener(OnPlayerDeathInternal);
            LoverSelectionStorer loverSelectionStorer = FindFirstObjectByType<LoverSelectionStorer>();
            if (loverSelectionStorer)
            {
                _lover = loverSelectionStorer.SelectedLover;
                Destroy(loverSelectionStorer.gameObject);
            }

            RandomizeGodOrder();
        }

        private void OnEnable()
        {
            DialogueManager.Instance.DialogueEndedEvent.AddListener(OnDialogueFinished);
        }

        private void OnDisable()
        {
            DialogueManager.Instance.DialogueEndedEvent.RemoveListener(OnDialogueFinished);
        }


        public void StartCurrentFightGodDialogue()
        {
            DialogueManager.Instance.StartDialogue($"{_allGods[_currentGodFightIdx].Fight.GodType.ToString() + "FightIntro"}");
        }
        

        public void OnDialogueFinished()
        {
            if(_currentGodFightIdx >= _allGods.Count)
            {
                OnAllGodsDefeated.Invoke();
                return;
            }

            _currentFightGod = _allGods[_currentGodFightIdx].Fight;
            _currentFightGod.OnDeath.AddListener(OnCurrentGodDefeatedInternal);
            _allGods[_currentGodFightIdx].Fight.gameObject.SetActive(true);
            Vector3 godSpawnPos = _currentFightGod.transform.position;
            godSpawnPos.x = _godFightSpawnPoint.position.x;
            _currentFightGod.transform.position = godSpawnPos;
            _allGods[_currentGodFightIdx].ThroneObject.SetActive(false);

            SoundManager.Instance.PlayMusic(_currentFightGod.GodType.ToString() + "Theme");

            _currentFightGod.StartBossFight();
            OnCurrentGodFightStarted.Invoke();
        }

        private void OnCurrentGodDefeatedInternal()
        {
            _currentFightGod.OnDeath.RemoveListener(OnCurrentGodDefeatedInternal);
            _allGods[_currentGodFightIdx].Fight.gameObject.SetActive(false);
            _allGods[_currentGodFightIdx].ThroneObject.SetActive(true);
            ++_currentGodFightIdx;
            OnCurrentGodDefeated.Invoke();
            OnDialogueFinished(); // TEMP
        }

        private void RandomizeGodOrder()
        {
            // Put gods in random order, with lover at the end
            GodInfo loverGod = new GodInfo();
            for (int bossIndex = 0; bossIndex < _allGods.Count; ++bossIndex)
            {
                if (_allGods[bossIndex].Fight.GodType == _lover)
                {
                    loverGod = _allGods[bossIndex];
                    _allGods.RemoveAt(bossIndex);
                    break;
                }
            }

            for(int bossIndex = 0; bossIndex < _allGods.Count; ++bossIndex)
            {
                int randIndex = Random.Range(bossIndex, _allGods.Count);
                GodInfo temp = _allGods[bossIndex];
                _allGods[bossIndex] = _allGods[randIndex];
                _allGods[randIndex] = temp;
            }
            _allGods.Add(loverGod);
        }

        private void OnPlayerDeathInternal()
        {
            _blurObject.SetActive(true);
            _playerObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("UI");
            _defeatedUI.SetActive(true);
        }

        public void RetryBossFight()
        {
            // Reset player
            _playerObject.transform.position = _playerRespawnPoint.position;
            _playerObject.GetComponent<PlayerHealth>().ResetHealth();
            _playerObject.GetComponent<PlayerSword>().ResetSword();
            _playerObject.GetComponent<PlayerMovement>().ResetMovement();
            _playerObject.GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");

            // Reset boss
            Vector3 godSpawnPos = _currentFightGod.transform.position;
            godSpawnPos.x = _godFightSpawnPoint.position.x;
            _currentFightGod.transform.position = godSpawnPos;
            _currentFightGod.RestartBossFight();

            _blurObject.SetActive(false);
            _defeatedUI.SetActive(false);
        }
    }
}