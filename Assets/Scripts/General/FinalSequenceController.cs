using Assets.Scripts.GodFights;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FinalSequenceController : MonoBehaviour
{
    [SerializeField] private float _walkSpeed = 5.0f;
    [SerializeField] private Transform _startPos;
    [SerializeField] private float _offsetBetweenPlayerAndLover;
    [SerializeField] private float _distanceFromEdgeToTriggerFade = 0.2f;

    private bool _hasFinished = false;
    private GameObject _player;
    private GodInfo _lover;

    public UnityEvent OnFinalSequenceFinished = new UnityEvent();
    private void Start()
    {
        _player = FightSequenceManager.Instance.PlayerObject;
        _lover = FightSequenceManager.Instance.GetLoverInfo();
    }
    public void StartFinalSequence()
    {
        _lover.ThroneObject.SetActive(false);
        _player.transform.localScale = Vector3.one;
        _player.GetComponent<PlayerInput>().DeactivateInput();
        _player.GetComponent<Animator>().SetTrigger("Moving");
        _player.transform.position = _startPos.position;
        Vector3 loverPos = _startPos.position;
        loverPos.x += _offsetBetweenPlayerAndLover;
        loverPos.y = _lover.Fight.transform.position.y;
        _lover.Fight.transform.position = loverPos;
        _lover.Fight.transform.localScale = Vector3.one;
        _lover.Fight.gameObject.SetActive(true);
        _lover.Fight.GetComponent<Animator>().SetTrigger("Move");
        StartCoroutine(WalkAway());
    }

    private IEnumerator WalkAway()
    {
        Vector3 cameraEdgeToWorld = Camera.main.ViewportToWorldPoint(new Vector3(1, 0.0f, 0.0f));
        float endPos = cameraEdgeToWorld.x;

        while(true)
        {
            float movement = _walkSpeed * Time.deltaTime;
            Vector3 newPos = _player.transform.position;
            newPos.x += movement;
            _player.transform.position = newPos;
            newPos.x += _offsetBetweenPlayerAndLover;
            newPos.y = _lover.Fight.transform.position.y;
            _lover.Fight.transform.position= newPos;
            if(!_hasFinished && (endPos - _player.transform.position.x) < _distanceFromEdgeToTriggerFade)
            {
                _hasFinished = true;
                OnFinalSequenceFinished.Invoke();
            }
            yield return null;
        }
    }
}
