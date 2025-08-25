using Assets.Scripts.GodFights;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.GodFights
{
    [System.Serializable]
    public class PhaseData
    {
        public List<WeightedAttack> Attacks;
        [HideInInspector] public int WeightSum;
    }

    [System.Serializable]
    public class WeightedAttack
    {
        public BaseGodAttack Attack;
        public int Weight;
    }

    public class GenericGodFight : MonoBehaviour
    {
        [SerializeField] private GodType _godType;
        [SerializeField] private List<PhaseData> _phasesData;
        [SerializeField] private float _delayBeforeFirstAttack = 2.0f;
        [SerializeField] private GodHealth _health;

        private BaseGodAttack _currentAttack;
        private int _currentAttackIndex = -1;
        private int _currentPhaseIndex = 0;

        public GodType GodType { get { return _godType; } }

        public UnityEvent OnDeath = new UnityEvent();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _health.OnDeath.AddListener(OnDeathInternal);
            StartBossFight();
        }

        public void StartBossFight()
        {
            foreach (var phaseAttacks in _phasesData)
            {
                foreach (var weightedAttack in phaseAttacks.Attacks)
                {
                    weightedAttack.Attack.InitializeAttack();
                }
            }
            CalculateWeights();
            Invoke(nameof(StartNextAttack), _delayBeforeFirstAttack);
        }

        private void StartNextAttack()
        {
            // If there's a guaranteed next attack, use that one
            if (_currentAttack != null && _currentAttack.NextGuaranteedAttack != null)
            {
                _currentAttack = _currentAttack.NextGuaranteedAttack;
                _currentAttackIndex = GetIndexOfAttackInPhaseAttacks(_currentAttack);
                _currentAttack.OnAttackFinished.AddListener(OnCurrentAttackFinished);
                _currentAttack.StartAttack();
                Debug.Log($"Starting guaranteed attack: {_currentAttack.name}");
                return;
            }

            // Look for new random attack, based on the weights
            int randomNumberInWeightRange = Random.Range(0, _phasesData[_currentPhaseIndex].WeightSum);
            int currentWeightSum = 0;
            for (int i = 0; i < _phasesData[_currentPhaseIndex].Attacks.Count; ++i)
            {
                var weightedAttack = _phasesData[_currentPhaseIndex].Attacks[i];
                currentWeightSum += weightedAttack.Weight;

                if (randomNumberInWeightRange < currentWeightSum && (_currentAttack == null || i != _currentAttackIndex || _currentAttack.CanExecuteConsecutive))
                {
                    _currentAttack = weightedAttack.Attack;
                    _currentAttackIndex = i;
                    _currentAttack.OnAttackFinished.AddListener(OnCurrentAttackFinished);
                    _currentAttack.StartAttack();
                    Debug.Log($"Starting new attack: {_currentAttack.name}");
                    break;
                }
            }
        }

        private void OnCurrentAttackFinished()
        {
            if (_currentAttack != null)
            {
                _currentAttack.OnAttackFinished.RemoveListener(OnCurrentAttackFinished);
                _currentAttack.StopAttack();
            }
            Invoke(nameof(StartNextAttack), _currentAttack.DelayAfterAttack);
        }

        private void OnDeathInternal()
        {
            if (_currentAttack != null)
            {
                _currentAttack.StopAttack();
                _currentAttack.OnAttackFinished.RemoveListener(OnCurrentAttackFinished);
                _currentAttack = null;
            }
            OnDeath.Invoke();
        }

        private void CalculateWeights()
        {
            foreach (var phaseData in _phasesData)
            {
                phaseData.WeightSum = 0;
                foreach (var weightedAttack in phaseData.Attacks)
                {
                    phaseData.WeightSum += weightedAttack.Weight;
                }
            }
        }

        private int GetIndexOfAttackInPhaseAttacks(BaseGodAttack attack)
        {
            int amountOfPhaseAttacks = _phasesData[_currentPhaseIndex].Attacks.Count;
            for (int attackIdx = 0; attackIdx < amountOfPhaseAttacks; ++attackIdx)
            {
                if (_phasesData[_currentPhaseIndex].Attacks[attackIdx].Attack == attack)
                {
                    return attackIdx;
                }
            }
            return -1;
        }

        public void OnAnimationEvent(string nameOfMethodToExecute)
        {
            if(_currentAttack != null)
            {
                if(!_currentAttack.TryExecuteAction(nameOfMethodToExecute))
                {
                    Debug.LogWarning($"No action registered for animation event {nameOfMethodToExecute} in attack {_currentAttack.name}");
                }
            }
            else
            {
                Debug.LogWarning($"No current attack to execute animation event {nameOfMethodToExecute} in");
            }
        }
    }
}