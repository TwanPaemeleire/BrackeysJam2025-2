using Assets.Scripts.GodFights;
using UnityEngine;

public class CreditSceneController : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Destroy(FightSequenceManager.Instance.gameObject);
        Destroy(DialogueManager.Instance.gameObject);
    }
}