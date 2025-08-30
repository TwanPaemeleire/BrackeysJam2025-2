using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.General;
using UnityEngine.Events;
using Assets.Scripts.Player;
using UnityEngine.InputSystem;

namespace Assets.Scripts.GodFights
{
    [System.Serializable]
    public class GodInfo
    {
        public BaseGodFight Fight;
        public GameObject ThroneObject;
    }

    public class FightSequenceManager : MonoSingleton<FightSequenceManager>
    {
        [SerializeField] private Transform _godFightSpawnPoint;
        [SerializeField] private List<GodInfo> _allGods = new List<GodInfo>();
        [SerializeField] private GodType _lover;
        [SerializeField] private GameObject _playerObject;
        [SerializeField] private Transform _playerRespawnPoint;
        [SerializeField] private FadeInUIHandler _defeatedUI;
        [SerializeField] private FadeInUIHandler _HUD;
        [SerializeField] private GameObject _blurObject;
        private BaseGodFight _currentFightGod;
        int _currentGodFightIdx = 0;

        private bool _readyToFight = false;
        private bool _arenaReached = false;

        public GodType Lover { get { return _lover; } }

        public GameObject PlayerObject { get { return _playerObject; } }

        public UnityEvent OnCurrentGodDefeated = new UnityEvent();
        public UnityEvent OnCurrentGodFightStarted = new UnityEvent();
        public UnityEvent OnAllGodsDefeated = new UnityEvent();
        public UnityEvent EndingAchieved = new UnityEvent();

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
            //DialogueManager.Instance.DialogueEndedEvent.RemoveListener(OnDialogueFinished);
        }


        public void StartCurrentFightGodDialogue()
        {
            var currentGodType = _allGods[_currentGodFightIdx].Fight.GodType;

            if (currentGodType != Lover)
            {
                SoundManager.Instance.PlayMusic("AmbientTheme");
                DialogueManager.Instance.StartDialogue($"{currentGodType.ToString() + "FightIntro"}");
            }
            else
            {
                SoundManager.Instance.PlayMusic("DecisionSceneTheme");
                DialogueManager.Instance.StartDialogue($"{currentGodType.ToString() + "LoverFightIntro"}");
            }

            _readyToFight = true;

            if (!_arenaReached)
            {
                _arenaReached = true;
            }
        }
        

        public void OnDialogueFinished()
        {
            if(!_readyToFight)
            {
                if (_currentGodFightIdx >= _allGods.Count)
                {
                    EndingAchieved?.Invoke();
                }
                else if (_arenaReached)
                {
                    StartCurrentFightGodDialogue();
                }

                return;
            }

            if (_currentGodFightIdx >= _allGods.Count)
            {
                return;
            }

            StartNextGodFight();
        }

        private void StartNextGodFight()
        {
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

            _HUD.StartFadingIn();
        }

        private void OnCurrentGodDefeatedInternal()
        {
            _currentFightGod.OnDeath.RemoveListener(OnCurrentGodDefeatedInternal);
            _allGods[_currentGodFightIdx].Fight.gameObject.SetActive(false);
            _allGods[_currentGodFightIdx].ThroneObject.SetActive(true);

            var defeatedGodType = _allGods[_currentGodFightIdx].Fight.GodType;

            ++_currentGodFightIdx;
            OnCurrentGodDefeated.Invoke();
            _HUD.StartFadingOut();

            if (_currentGodFightIdx >= _allGods.Count)
            {
                OnAllGodsDefeated.Invoke();
                _readyToFight = false;
                return;
            }

            StartDefeatedGodDialogue(defeatedGodType);
        }

        private void StartDefeatedGodDialogue(GodType defeatedGod)
        {
            DialogueManager.Instance.StartDialogue($"{defeatedGod + "FightOutro"}");

            if (defeatedGod != Lover)
            {
                SoundManager.Instance.PlayMusic("AmbientTheme");
            }
            else
            {
                SoundManager.Instance.PlayMusic("LoverTheme");
            }

            _readyToFight = false;
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
            _defeatedUI.StartFadingIn();
            _HUD.StartFadingOut();
        }

        public void RetryBossFight()
        {
            _HUD.StartFadingIn();

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
            _defeatedUI.StartFadingOut();
        }

        public GodInfo GetLoverInfo()
        {
            return _allGods.FirstOrDefault(god => god.Fight.GodType == _lover);
        }
    }
}