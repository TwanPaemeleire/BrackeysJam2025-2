using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class VisualInteractionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private Vector3 _maxScale;
        [SerializeField] private float _timeToReachTargetScale = 1.0f;
        [SerializeField] private Sprite _default;
        [SerializeField] private Sprite _hovered;
        [SerializeField] private Sprite _pressed;

        private bool _hasMadeSelection = false;
        private Image _buttonImage;
        private Coroutine _scaleCoroutine;

        private void OnEnable()
        {
            if (_buttonImage == null) _buttonImage = GetComponent<Image>();
            _hasMadeSelection = false;
            _buttonImage.sprite = _default;
            transform.localScale = Vector3.one;
        }
        private void Awake()
        {
            _buttonImage = GetComponent<Image>();
            _buttonImage.sprite = _default;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_hasMadeSelection) return;
            _buttonImage.sprite = _hovered;
            StartScaleCoroutine(_maxScale);
            EventSystem.current.SetSelectedGameObject(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_hasMadeSelection) return;
            _buttonImage.sprite = _default;
            StartScaleCoroutine(Vector3.one);
        }

        void StartScaleCoroutine(Vector3 targetScale)
        {
            if (_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }
            _scaleCoroutine = StartCoroutine(ChangeButtonScale(targetScale));
        }

        private IEnumerator ChangeButtonScale(Vector3 targetScale)
        {
            Vector3 initialScale = transform.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < _timeToReachTargetScale)
            {
                float newScale = Mathf.SmoothStep(initialScale.x, targetScale.x, elapsedTime / _timeToReachTargetScale);
                transform.localScale = new Vector3(newScale, newScale, 1.0f);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _buttonImage.sprite = _pressed;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _hasMadeSelection = true;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (_hasMadeSelection) return;
            _buttonImage.sprite = _hovered;
            StartScaleCoroutine(_maxScale);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (_hasMadeSelection) return;
            _buttonImage.sprite = _default;
            StartScaleCoroutine(Vector3.one);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _hasMadeSelection = true;
        }
    }
}