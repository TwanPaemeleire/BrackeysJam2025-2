using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public float MinDistanceToExecute;
        public float MaxDistanceToExecute;
    }

    public class SunGodFight : BaseGodFight
    {
        [SerializeField] private List<PhaseData> _phasesData;
        [SerializeField] private float _idleTimeAfterAttack = 1.0f;
        [SerializeField] private float _speedToTrackPlayer = 1.0f;
        [SerializeField] private float _minimumTimeToTrackPlayer = 1.0f;
        [SerializeField] private AudioSource _footStepAudioSource;

        private BaseGodAttack _currentAttack;
        private int _currentAttackIndex = -1;
        private int _currentPhaseIndex = 0;
        private Coroutine _trackingCoroutine;

        public override void StartBossFight()
        {
            base.StartBossFight();

            foreach (var phaseAttacks in _phasesData)
            {
                foreach (var weightedAttack in phaseAttacks.Attacks)
                {
                    weightedAttack.Attack.InitializeAttack();
                    weightedAttack.Attack.God = this;
                }
            }
            CalculateWeights();
            Animator.SetTrigger("Idle");
            _trackingCoroutine = StartCoroutine(TrackPlayerCoroutine());
        }

        public override void RestartBossFight()
        {
            StopAllCoroutines();
            if (_currentAttack != null)
            {
                _currentAttack.StopAttack();
                _currentAttack.OnAttackFinished.RemoveListener(OnCurrentAttackFinished);
            }
            _currentAttack = null;
            _currentAttackIndex = -1;
            _currentPhaseIndex = 0;
            Health.ResetHealth();
            Animator.SetTrigger("Idle");
            _trackingCoroutine = StartCoroutine(TrackPlayerCoroutine());
        }

        private void StartNextAttack()
        {
            // Look for new random attack, based on the weights
            List<WeightedAttack> viableAttacks = GetAllViableAttacks();
            if(viableAttacks.Count == 0)
            {
                _trackingCoroutine = StartCoroutine(TrackPlayerCoroutine());
                return;
            }
            int weightSum = 0;
            foreach(var attack in viableAttacks)
            {
                weightSum += attack.Weight;
            }

            int randomNumberInWeightRange = Random.Range(0, weightSum);
            int currentWeightSum = 0;
            for (int i = 0; i < viableAttacks.Count; ++i)
            {
                var weightedAttack = viableAttacks[i];
                currentWeightSum += weightedAttack.Weight;

                if (randomNumberInWeightRange < currentWeightSum)
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
            StartCoroutine(AfterAttackCoroutine());
        }

        protected override void OnDeathInternal()
        {
            if (_currentAttack != null)
            {
                _currentAttack.StopAttack();
                _currentAttack.OnAttackFinished.RemoveListener(OnCurrentAttackFinished);
                _currentAttack = null;
            }
            StopAllCoroutines();
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

        private List<WeightedAttack> GetAllViableAttacks()
        {
            List<WeightedAttack> viableAttacks = new List<WeightedAttack>();
            float distanceToPlayer = Mathf.Abs(FightSequenceManager.Instance.PlayerObject.transform.position.x - transform.position.x);
            foreach (var weightedAttack in _phasesData[_currentPhaseIndex].Attacks)
            {
                if (distanceToPlayer >= weightedAttack.MinDistanceToExecute && distanceToPlayer <= weightedAttack.MaxDistanceToExecute)
                {
                    viableAttacks.Add(weightedAttack);
                }
            }
            return viableAttacks;
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

        private IEnumerator AfterAttackCoroutine()
        {
            Animator.SetTrigger("Idle");
            yield return new WaitForSeconds(_idleTimeAfterAttack);
            _trackingCoroutine = StartCoroutine(TrackPlayerCoroutine());
        }

        private IEnumerator TrackPlayerCoroutine()
        {             
            float elapsedTime = 0f;
            Vector3 initialForward = transform.forward;
            float distanceToPlayer = FightSequenceManager.Instance.PlayerObject.transform.position.x - transform.position.x;
            CurrentDirectionMultiplier = ((distanceToPlayer > 0.0f) ? 1.0f : -1.0f);
            transform.localScale = new Vector3(CurrentDirectionMultiplier, 1.0f, 1.0f);
            if (Mathf.Abs(distanceToPlayer) < 0.5f)
            {
                StartNextAttack();
                _trackingCoroutine = null;
            }
            else
            {
                Animator.SetTrigger("Move");
                bool startedNextAttackEarly = false;
                while (elapsedTime < _minimumTimeToTrackPlayer)
                {
                    distanceToPlayer = FightSequenceManager.Instance.PlayerObject.transform.position.x - transform.position.x;
                    if (Mathf.Abs(distanceToPlayer) < 0.5f)
                    {
                        StartNextAttack();
                        startedNextAttackEarly = true;
                        break;
                    }
                    Vector3 directionToPlayer = (FightSequenceManager.Instance.PlayerObject.transform.position - transform.position).normalized;
                    directionToPlayer.y = 0;
                    transform.position += directionToPlayer * _speedToTrackPlayer * Time.deltaTime;
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                if(!startedNextAttackEarly) StartNextAttack();
            }
            _trackingCoroutine = null;
        }

        public void OnFootStepEvent()
        {
            _footStepAudioSource.Play();
        }
    }
}