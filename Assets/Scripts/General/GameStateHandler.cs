using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.General
{
    public class GameStateHandler : MonoBehaviour
    {
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private GameObject _pauseMenuUI;

        public void PauseGame()
        {
            Time.timeScale = 0f;
            _playerInput.SwitchCurrentActionMap("UI");
            _pauseMenuUI.SetActive(true);
        }

        public void ResumeGame()
        {
            Time.timeScale = 1f;
            _pauseMenuUI.SetActive(false);
            _playerInput.SwitchCurrentActionMap("Gameplay");
        }

        public void QuitGame()
        {
#if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE) 
            Application.Quit();
#elif (UNITY_WEBGL)
            Application.OpenURL(Application.absoluteURL);
#endif
        }
    }
}