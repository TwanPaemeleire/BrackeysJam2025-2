using Assets.Scripts.GodFights;
using UnityEngine;

public class CreditSceneController : MonoBehaviour
{
    private bool _isReturning = false;
    public void ReturnToMainMenu()
    {
        if (_isReturning) return;
        _isReturning = true;
        Destroy(FightSequenceManager.Instance.gameObject);
        Destroy(DialogueManager.Instance.gameObject);
    }
}