using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.General;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights
{
    public class FightSequenceManager : MonoSingleton<FightSequenceManager>
    {
        private List<GameObject> _allGods = new List<GameObject>(); // Change this to be god base script later on
        public GodType _lover;
        private List<int> _godsLeftToDefeat = new List<int>();
        private GameObject _currentFightGod;

        public UnityEvent OnCurrentGodDefeated = new UnityEvent();
        public UnityEvent OnCurrentGodFightStarted = new UnityEvent();
        public UnityEvent OnAllGodsDefeated = new UnityEvent();
        protected override void Init()
        {
            LoverSelectionStorer loverSelectionStorer = FindFirstObjectByType<LoverSelectionStorer>();
            _lover = loverSelectionStorer.SelectedLover;
            Destroy(loverSelectionStorer.gameObject);
            for(int bossIndex = 0; bossIndex < _allGods.Count; ++bossIndex)
            {
                //if(_allGods[bossIndex].GetComponent<BaseGod>().GodType == _lover)
                //{
                //    continue;
                //}
                _godsLeftToDefeat.Add(bossIndex);
            }
        }

        public void OnDialogueFinished()
        {
            if(_godsLeftToDefeat.Count == 0)
            {
                OnAllGodsDefeated.Invoke();
                return;
            }
            int randomIndex = Random.Range(0, _godsLeftToDefeat.Count);
            int godIndex = _godsLeftToDefeat[randomIndex];
            _godsLeftToDefeat.RemoveAt(randomIndex);
            _currentFightGod = _allGods[godIndex];
            // Hook into death event of god health script
            // Call start on god fight script
            OnCurrentGodFightStarted.Invoke();
        }

        private void OnCurrentGodDefeatedInternal()
        {
            OnCurrentGodDefeated.Invoke();
        }
    }
}