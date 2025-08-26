using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Assets.Scripts.GodFights;
using TMPro;

namespace Assets.Scripts.UI
{
    public class DecisionBossButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private LoverSelectionStorer _loverSelectionStorer;
        [SerializeField] private GodType _godType;
        [SerializeField] private Vector3 _maxScale = new Vector3(1.2f, 1.2f, 1.2f);
        [SerializeField] private float _timeToReachTargetScale = 0.5f;
        [SerializeField] private Color _hoveredNameColor;
        [SerializeField] private TextMeshProUGUI _nameText;

        private Coroutine _scaleCoroutine;
        private bool _isSelected = false;

        public GodType GodType { get { return _godType; } }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(_isSelected) return;
            _isSelected = true;
            _loverSelectionStorer.SelectNewLover(this);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(_isSelected) return;
            StartScaleCoroutine(_maxScale);
            EventSystem.current.SetSelectedGameObject(gameObject);
            _nameText.color = _hoveredNameColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(_isSelected) return;
            StartScaleCoroutine(Vector3.one);
            _nameText.color = Color.white;
        }

        void StartScaleCoroutine(Vector3 targetScale)
        {
            if(_scaleCoroutine != null)
            {
                StopCoroutine(_scaleCoroutine);
            }
            _scaleCoroutine = StartCoroutine(ChangeButtonScale(targetScale));
        }

        public void OnDeselected()
        {
            _isSelected = false;
            StartScaleCoroutine(Vector3.one);
        }

        private IEnumerator ChangeButtonScale(Vector3 targetScale)
        {
            Vector3 initialScale = transform.localScale;
            float elapsedTime = 0f;

            while(elapsedTime < _timeToReachTargetScale)
            {
                float newScale = Mathf.SmoothStep(initialScale.x, targetScale.x, elapsedTime / _timeToReachTargetScale);
                transform.localScale = new Vector3(newScale, newScale, 1.0f);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }
            transform.localScale = targetScale;
        }

        public void OnSelect(BaseEventData eventData)
        {
            _nameText.color = _hoveredNameColor;
            if (_isSelected) return;
            StartScaleCoroutine(_maxScale);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _nameText.color = Color.white;
            if (_isSelected) return;
            StartScaleCoroutine(Vector3.one);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            if (_isSelected) return;
            _isSelected = true;
            _loverSelectionStorer.SelectNewLover(this);
        }
    }
}