using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.BossFights
{
    public class LoverSelectionStorer : MonoBehaviour
    {
        // Note for future, when boss manager reads this value, it should delete this object afterwards
        private GodType _selectedLover;
        public GodType SelectedLover { get { return _selectedLover; }}

        private DecisionBossButton _currentlySelectedButton;
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void SelectNewLover(DecisionBossButton loverButton)
        {
            if(_currentlySelectedButton != null)
            {
                _currentlySelectedButton.OnDeselected();
            }
            _selectedLover = loverButton.GodType;
            _currentlySelectedButton = loverButton;
        }
    }
}