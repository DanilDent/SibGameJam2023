using Config;
using Enemy;
using Game;
using GameTime;
using Helpers;
using Spine.Unity;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        private GameConfigSO _config;
        private PlayerSO _localConfig;

        private void Start()
        {
            _config = ConfigContainer.Instance.Value;
            _localConfig = _config.Player;

            FillFromConfig(_localConfig);
            _eventBus = EventBusSingleton.Instance;
            _rb = GetComponent<Rigidbody2D>();
            _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
            _attackColliderTransform.gameObject.SetActive(false);

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
        }

        public enum BeatEffect
        {
            None,
            CanDash
        }

        [Header("Beat effetcts")]
        [SerializeField] private BeatEffect[] _beatEffects;

        [SerializeField] private float _movementSpeed;
        [SerializeField] private float _movementForce;
        [SerializeField] private float _dashForce;
        [SerializeField] private float _attackDashForce;
        [SerializeField] private float _attackDashTimeScaleFactor;
        [SerializeField] private float _dashTimeScaleFactor;
        [SerializeField] private float _damage;
        [SerializeField] private float _attackSpeed;
        [SerializeField] private float _movementEpsThreshold = 1f;
        [SerializeField] private Transform _attackColliderTransform;
        [SerializeField] private float _hitBeatEffectDuration = 0.3f;
        private SkeletonAnimation _skeletonAnimation;
        private string _currentAnimName;
        private bool _isDash;
        private bool _isAttack;

        private Rigidbody2D _rb;
        private Vector3 _movementInput;

        private EventBusSingleton _eventBus;

        #region BeatEffectsCommands
        private bool _canDash = false;
        private bool _canAttack = true;
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
        #endregion

        #region BeatEffectsController
        private void SubscribeBeatEffectsCommands()
        {
            _eventBus.Subscribe<ClockStepSignal>(HandleCanDash);
        }

        private void UnsubscribeBeatEffectsCommands()
        {
            _eventBus.Unsubscribe<ClockStepSignal>(HandleCanDash);
        }
        #endregion

        private void OnDestroy()
        {
            UnsubscribeBeatEffectsCommands();
        }

        private void Update()
        {
            HandleInput();

            if (_isAttack && _currentAnimName != "attack")
            {
                _skeletonAnimation.AnimationState.ClearTracks();

                _currentAnimName = "attack";
                _skeletonAnimation.AnimationState.SetAnimation(0, _currentAnimName, true);

                return;
            }
            else if (_isDash && _currentAnimName != "jump")
            {
                _skeletonAnimation.AnimationState.ClearTracks();

                _currentAnimName = "jump";
                _skeletonAnimation.AnimationState.SetAnimation(0, _currentAnimName, true);

                return;
            }
            else if (!_isDash && !_isAttack && _rb.velocity.magnitude > _movementEpsThreshold && _currentAnimName != "run")
            {
                _skeletonAnimation.AnimationState.ClearTracks();

                _currentAnimName = "run";
                _skeletonAnimation.AnimationState.SetAnimation(0, _currentAnimName, true);
            }
            else if (!_isDash && !_isAttack && _rb.velocity.magnitude < _movementEpsThreshold && _currentAnimName != "idle")
            {
                _skeletonAnimation.AnimationState.ClearTracks();

                _currentAnimName = "idle";
                _skeletonAnimation.AnimationState.SetAnimation(0, _currentAnimName, true);
            }
        }

        private void Dash()
        {
            _rb.AddForce((_movementInput == Vector3.zero ? transform.right : _movementInput.normalized) * _dashForce, ForceMode2D.Impulse);
            //Debug.Log($"Dash force: {((_movementInput == Vector3.zero ? transform.right : _movementInput.normalized) * _dashForce).magnitude}");
        }

        private void Attack()
        {
            Vector3 cursorWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 attackDashPoint = new Vector3(cursorWorldPosition.x, cursorWorldPosition.y, transform.position.z);
            Vector3 attackDashDir = (attackDashPoint - transform.position).normalized;

            if (attackDashDir != Vector3.zero && Mathf.Abs(attackDashDir.x) > 0f)
            {
                transform.rotation = Quaternion.Euler(0f, 90f - 90f * attackDashDir.x / Mathf.Abs(attackDashDir.x), 0f);
            }

            _attackColliderTransform.transform.right = attackDashDir;
            _attackColliderTransform.gameObject.SetActive(true);

            _rb.AddForce(attackDashDir * _attackDashForce, ForceMode2D.Impulse);
            //Debug.Log($"Attack dash force: {(attackDashDir * _attackDashForce).magnitude}");
        }

        private void FixedUpdate()
        {
            if (_rb.velocity.magnitude < _movementSpeed)
            {
                _rb.AddForce(_movementInput * _movementForce * Time.deltaTime, ForceMode2D.Force);
            }

            if (_rb.velocity.magnitude > _movementSpeed && !_isDash && !_isAttack)
            {
                _rb.velocity = _rb.velocity.normalized * _movementSpeed;
            }

            if (_movementInput != Vector3.zero && Mathf.Abs(_movementInput.x) > 0f)
            {
                transform.rotation = Quaternion.Euler(0f, 90f - 90f * _movementInput.x / Mathf.Abs(_movementInput.x), 0f);
            }
        }

        private void HandleInput()
        {
            _movementInput = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);
            _movementInput = _movementInput.normalized;

            if (Input.GetKeyDown(KeyCode.Space) && _canDash && !_isDash)
            {
                float oldDrag = _rb.drag;
                _rb.drag = 0f;
                _isDash = true;
                float dashTime = _dashForce / (_rb.mass * _dashTimeScaleFactor);
                //Debug.Log($"Dash time sec: {dashTime}");
                StartCoroutine(ResetDashCoroutine(dashTime));
                Dash();
                _rb.drag = oldDrag;
            }

            if (Input.GetMouseButtonDown(0) && _canAttack)
            {
                float oldDrag = _rb.drag;
                _rb.drag = 0f;
                _isAttack = true;
                _canAttack = false;
                float attackDashTimeSec = _attackDashForce / (_rb.mass * _attackDashTimeScaleFactor);
                //Debug.Log($"Attack dash time sec: {attackDashTimeSec}");
                StartCoroutine(ResetAttackDashCoroutine(attackDashTimeSec));
                StartCoroutine(Helpers.CoroutineHelpers.InvokeWithDelay(() =>
                {
                    _canAttack = true;
                }, _attackSpeed));
                Attack();
                _rb.drag = oldDrag;
            }
        }

        private IEnumerator ResetDashCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            _isDash = false;
        }

        private IEnumerator ResetAttackDashCoroutine(float time)
        {
            yield return new WaitForSeconds(time);
            _isAttack = false;
            _attackColliderTransform.rotation = Quaternion.identity;
            _attackColliderTransform.gameObject.SetActive(false);
        }
    }
}
