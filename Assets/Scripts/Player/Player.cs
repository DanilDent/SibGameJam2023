using Config;
using Enemy;
using Game;
using GameTime;
using Helpers;
using Sound;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        private GameConfigSO _config;
        private PlayerSO _localConfig;
        private SFXController _sfxController;
        private AudioSource _audioSource;

        private int _currentHealth;

        private void Start()
        {
            _config = ConfigContainer.Instance.Value;
            _localConfig = _config.Player;
            _sfxController = SFXController.Instance;
            _audioSource = GetComponent<AudioSource>();

            FillFromConfig(_localConfig);
            _eventBus = EventBusSingleton.Instance;
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
            _attackColliderTransform.gameObject.SetActive(false);

            _eventBus.Subscribe<EnemyHited>(OnEnemyHited);
            SubscribeBeatEffectsCommands();
        }

        public void FillFromConfig(PlayerSO config)
        {
            config.Init(this);

            _hitBeatEffectDuration = config.HitBeatEffectDuration;
            _movementSpeed = config.MovementSpeed;
            _movementForce = config.MovementForce;
            _dashForce = config.DashForce;
            _attackDashForce = config.AttackDashForce;
            _attackDashTimeScaleFactor = config.AttackDashTimeScaleFactor;
            _dashTimeScaleFactor = config.DashTimeScaleFactor;
            _attackSpeed = config.AttackSpeed;
            _damage = config.Damage;
            _currentHealth = config.Health;
            _dashDelaySec = config.DashDelaySec;
        }

        [Header("Beat effetcts")]
        [SerializeField] private BeatEffect[] _beatEffects;

        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _movementForce;
        [SerializeField] private float _dashForce;
        [SerializeField] private float _attackDashForce;
        [SerializeField] private float _attackDashTimeScaleFactor;
        [SerializeField] private float _dashTimeScaleFactor;
        [SerializeField] private float _attackSpeed;
        [SerializeField] private int _damage;
        [SerializeField] private float _movementEpsThreshold = 1f;
        [SerializeField] private Transform _attackColliderTransform;
        [SerializeField] private float _hitBeatEffectDuration = 0.3f;
        [SerializeField] private float _dashDelaySec;
        private Animator _animator;
        private string _currentAnimState;
        private bool _isDash;
        private bool _isAttack;

        private Rigidbody2D _rb;
        private Vector3 _movementInput;
        private int _dirX;
        private int _dirY;

        private EventBusSingleton _eventBus;

        #region BeatEffectsCommands
        public enum BeatEffect
        {
            None,
            CanDash,
            CanAttack,
        }

        private bool _canDash = true;
        private bool _canAttack = true;
        private bool _hitBeatNormal = false;

        public void HandleHitBeat(ClockStepSignal signal)
        {
            _hitBeatNormal = true;
            StartCoroutine(CoroutineHelpers.InvokeWithDelay(
            () =>
            {
                _hitBeatNormal = false;
            },
            delay: _hitBeatEffectDuration));
        }

        public void HandleCanDash(ClockStepSignal signal)
        {
            if (!_beatEffects.Contains(BeatEffect.CanDash))
            {
                return;
            }

            _canDash = true;
            StartCoroutine(CoroutineHelpers.InvokeWithDelay(
            () =>
            {
                _canDash = false;
            },
            delay: _hitBeatEffectDuration));
        }

        public void HandleCanAttack(ClockStepSignal signal)
        {
            if (!_beatEffects.Contains(BeatEffect.CanAttack))
            {
                return;
            }

            _canAttack = true;
            StartCoroutine(CoroutineHelpers.InvokeWithDelay(
            () =>
            {
                _canAttack = true;
            },
            delay: _hitBeatEffectDuration));
        }

        #endregion

        #region BeatEffectsController
        private void SubscribeBeatEffectsCommands()
        {
            _eventBus.Subscribe<ClockStepSignal>(HandleHitBeat);

            //_eventBus.Subscribe<ClockStepSignal>(HandleCanDash);
            //_eventBus.Subscribe<ClockStepSignal>(HandleCanAttack);
        }

        private void UnsubscribeBeatEffectsCommands()
        {
            _eventBus.Unsubscribe<ClockStepSignal>(HandleHitBeat);

            //_eventBus.Unsubscribe<ClockStepSignal>(HandleCanDash);
            //_eventBus.Unsubscribe<ClockStepSignal>(HandleCanAttack);
        }
        #endregion

        private void OnDestroy()
        {
            _eventBus.Unsubscribe<EnemyHited>(OnEnemyHited);
            UnsubscribeBeatEffectsCommands();
        }

        private void Update()
        {
            HandleInput();
            HandleOrientation();

            if (_isAttack && _currentAnimState != "attack")
            {
                _animator.SetTrigger("AttackTrigger");
                _currentAnimState = "attack";

                return;
            }
            else if (_isDash && _currentAnimState != "jump")
            {
                _currentAnimState = "jump";
                _animator.SetBool("IsDash", true);
                return;
            }
            else if (!_isDash && !_isAttack && _rb.velocity.magnitude > _movementEpsThreshold && _currentAnimState != "run")
            {
                _currentAnimState = "run";
            }
            else if (!_isDash && !_isAttack && _rb.velocity.magnitude < _movementEpsThreshold && _currentAnimState != "idle")
            {
                _currentAnimState = "idle";
                _animator.SetFloat("Speed", 0f);
            }

            _animator.SetFloat("Speed", _movementInput == Vector3.zero ? 0f : 1f);
        }

        private void HandleOrientation()
        {
            if (_isAttack)
            {
                return;
            }

            if (Mathf.Abs(_movementInput.x) > 0f)
            {
                _dirX = (int)Mathf.Sign(_movementInput.x);
                transform.right = Vector3.right * _dirX;
            }
            else
            {
                _dirX = 0;
            }

            if (Mathf.Abs(_movementInput.y) > 0f)
            {
                _dirY = (int)Mathf.Sign(_movementInput.y);
                if (_dirY != 0)
                {
                    _animator.SetFloat("Direction", _dirY);
                }
            }
            else
            {
                _dirY = 0;
            }

            //if (_movementInput != Vector3.zero && Mathf.Abs(_movementInput.y) > 0f)
            //{
            //    int dirY = Mathf.RoundToInt(_movementInput.y / Mathf.Abs(_movementInput.y));
            //    _direction = -dirY;
            //    _animator.SetFloat("Direction", -(float)dirY);
            //}

            //if (_movementInput != Vector3.zero && Mathf.Abs(_movementInput.x) > 0f)
            //{
            //    transform.rotation = Quaternion.Euler(0f, 90f - 90f * _movementInput.x / Mathf.Abs(_movementInput.x) * (_direction), 0f);
            //}
        }

        private void Dash()
        {
            _rb.velocity = Vector3.zero;
            _rb.AddForce((_movementInput == Vector3.zero ? transform.right : _movementInput.normalized) * _dashForce, ForceMode2D.Impulse);
            _sfxController.PlaySFX("dash", _audioSource);
            //Debug.Log($"Dash force: {((_movementInput == Vector3.zero ? transform.right : _movementInput.normalized) * _dashForce).magnitude}");
        }

        private void Attack()
        {
            if (_isDash || _isAttack)
            {
                return;
            }

            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 attackDashPoint = new Vector3(cursorWorldPosition.x, cursorWorldPosition.y, transform.position.z);
            Vector3 attackDashDir = (attackDashPoint - transform.position).normalized;

            if (attackDashDir != Vector3.zero && Mathf.Abs(attackDashDir.x) > 0f)
            {
                transform.rotation = Quaternion.Euler(0f, 90f - 90f * attackDashDir.x / Mathf.Abs(attackDashDir.x), 0f);
            }

            if (attackDashDir != Vector3.zero && Mathf.Abs(attackDashDir.y) > 0f)
            {
                _animator.SetFloat("Direction", attackDashDir.y / Mathf.Abs(attackDashDir.y));
            }

            _attackColliderTransform.transform.right = attackDashDir;
            _attackColliderTransform.gameObject.SetActive(true);

            _rb.velocity = Vector3.zero;
            _rb.AddForce(attackDashDir * _attackDashForce, ForceMode2D.Impulse);
            _sfxController.PlaySFX("sword", _audioSource);
            //Debug.Log($"Attack dash force: {(attackDashDir * _attackDashForce).magnitude}");
        }

        private void FixedUpdate()
        {
            float eps = 0.1f;
            if (!_isAttack && !_isDash && _rb.velocity.magnitude < (_movementSpeed - eps))
            {
                _rb.AddForce(_movementInput * _movementForce * Time.deltaTime, ForceMode2D.Force);
            }

            if (_rb.velocity.magnitude > _movementSpeed && !_isDash && !_isAttack)
            {
                //Debug.Log($"Before normalize velocity: {_rb.velocity.magnitude}");
                _rb.velocity = _rb.velocity.normalized * _movementSpeed;
                //Debug.Log($"Normalize velocity: {_rb.velocity.magnitude}");
            }
        }

        private void HandleInput()
        {
            if (!_isDash && !_isAttack)
            {
                _movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
                _movementInput = _movementInput.normalized;
            }

            if (_isDash || _isAttack)
            {
                _movementInput = Vector3.zero;
            }

            if (!_isAttack && Input.GetKeyDown(KeyCode.Space) && _canDash && !_isDash && (_beatEffects.Contains(BeatEffect.CanDash) ? _hitBeatNormal : true))
            {
                float oldDrag = _rb.drag;
                _rb.drag = 0f;
                _isDash = true;
                float dashTime = _dashForce / (_rb.mass * _dashTimeScaleFactor);
                Debug.Log($"Dash time sec: {dashTime}");
                StartCoroutine(ResetDashCoroutine(dashTime));
                Dash();
                _rb.drag = oldDrag;
            }

            if (!_isDash && Input.GetMouseButtonDown(0) && _canAttack && !_isAttack && (_beatEffects.Contains(BeatEffect.CanAttack) ? _hitBeatNormal : true))
            {
                float oldDrag = _rb.drag;
                _rb.drag = 0f;
                _canAttack = false;
                float attackDashTimeSec = _attackDashForce / (_rb.mass * _attackDashTimeScaleFactor);
                //Debug.Log($"Attack dash time sec: {attackDashTimeSec}");
                StartCoroutine(ResetAttackDashCoroutine(_attackSpeed));
                StartCoroutine(Helpers.CoroutineHelpers.InvokeWithDelay(() =>
                {
                    _canAttack = true;
                }, _attackSpeed));
                Attack();
                _isAttack = true;
                _rb.drag = oldDrag;
            }
        }

        private IEnumerator ResetDashCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            _animator.SetBool("IsDash", false);
            _isDash = false;
        }

        private IEnumerator ResetAttackDashCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            _isAttack = false;
            _attackColliderTransform.rotation = Quaternion.identity;
            _attackColliderTransform.gameObject.SetActive(false);
        }

        private void OnEnemyHited(EnemyHited signal)
        {
            signal.EnemyContainer.EnemyView.EnemyLogic.TakeDamage(_damage);
        }

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _eventBus.Invoke(new TakeDamage(this, damage));

            if (_currentHealth <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            _eventBus.Invoke(new Die(this));
        }
    }
}
