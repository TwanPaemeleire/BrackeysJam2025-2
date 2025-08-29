using Assets.Scripts.General;
using Assets.Scripts.GodFights;
using UnityEngine;

namespace Assets.Scripts.DialogueMisc
{
    public class IntroOutroDialogueHandler : MonoBehaviour
    {
        private AudioClip _loverTheme = null;

        private void Start()
        {
            Invoke(nameof(FaceTheLover), 2.5f);
        }

        private void OnEnable()
        {
            FightSequenceManager.Instance.OnAllGodsDefeated.AddListener(AchieveVictory);
        }

        private void OnDisable()
        {
            //FightSequenceManager.Instance.OnAllGodsDefeated.RemoveListener(AchieveVictory);
        }

        private void FaceTheLover()
        {
            var clip = Resources.Load<AudioClip>($"Music/DecisionSceneTheme");

            if (clip != null)
            {
                _loverTheme = clip;
                SoundManager.Instance.PlayMusic(_loverTheme);
            }

            DialogueManager.Instance.StartDialogue($"{FightSequenceManager.Instance.Lover.ToString()}" + "LoverIntro");
        }

        private void AchieveVictory()
        {
            if (_loverTheme != null)
            {
                SoundManager.Instance.PlayMusic(_loverTheme);
            }

            DialogueManager.Instance.StartDialogue($"{FightSequenceManager.Instance.Lover.ToString()}" + "LoverOutro");
        }
    }
}
