using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonInitiaior : MonoBehaviour
{
    [SerializeField] private GameObject _buttonToSelectWhenOpeningMenu;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(_buttonToSelectWhenOpeningMenu);
    }
}