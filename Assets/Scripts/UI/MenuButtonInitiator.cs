using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class MenuButtonInitiator : MonoBehaviour
    {
        [SerializeField] private GameObject _buttonToSelectWhenOpeningMenu;
        private void OnEnable()
        {
            StartCoroutine(SelectNextFrame()); // Needs to be done after one frame because of timing issues
        }

        private IEnumerator SelectNextFrame()
        {
            yield return null;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(_buttonToSelectWhenOpeningMenu);
        }
    }
}