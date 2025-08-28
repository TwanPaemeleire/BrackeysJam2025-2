using Assets.Scripts.Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ArenaEnterTrigger : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _timeToReachTarget;
    private bool _startedMoving = false;

    public UnityEvent OnArenaEnterStart = new UnityEvent();
    public UnityEvent OnArenaEntered = new UnityEvent();
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_startedMoving) return;
        if(collision.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            if(collision.gameObject.transform.position.x > transform.position.x)
            {
                GetComponent<BoxCollider2D>().isTrigger = false;
                OnArenaEnterStart.Invoke();
                _startedMoving = true;
                StartCoroutine(MoveToArena());
            }
        }
    }

    private IEnumerator MoveToArena()
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = _targetTransform.position;
        float elapsedTime = 0.0f;
        while (elapsedTime < _timeToReachTarget)
        {
            elapsedTime += Time.deltaTime;
            float newX = Mathf.SmoothStep(startPos.x, targetPos.x, elapsedTime / _timeToReachTarget);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            yield return null;
        }
        OnArenaEntered.Invoke();
    }
}
