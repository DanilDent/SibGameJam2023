using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class EnemyView : MonoBehaviour
    {
        [field: SerializeField] public SpriteRenderer Sprite { get; private set; }

        [field: SerializeField] public float FadeDuraion { get; private set; }
        public Animator Animator;

        public EnemyLogic EnemyLogic { get; private set; }

        private const string DIE_TRIGGER = "DieTrigger";
        private const string SPAWN_TRIGGER = "SpawnTrigger";
        private const string CHASE_TRIGGER = "ChaseTrigger";
        private const string IDLE_TRIGGER = "IdleTrigger";

        private void OnEnable()
        {
            Sprite.color = Color.white;
        }

        //use in spawner
        public void Init(EnemyLogic enemyLogic)
        {
            Sprite.transform.localScale = new Vector3(-Sprite.transform.localScale.x, Sprite.transform.localScale.y);
            EnemyLogic = enemyLogic;
            SubsribeOnEvents();
        }
        
        private void SubsribeOnEvents()
        {
            EventBusSingleton.Instance.Subscribe<Die>(OnDie);
            EventBusSingleton.Instance.Subscribe<TakeDamage>(OnDamageTaken);
        }

        private void UnsubscribeFromEvents()
        {

            EventBusSingleton.Instance.Unsubscribe<Die>(OnDie);
            EventBusSingleton.Instance.Unsubscribe<TakeDamage>(OnDamageTaken);
        }

        //some ui & anim logic
        private void OnDie(Die signal)
        {
            if (signal.Enemy == EnemyLogic)
            {
                Animator.SetTrigger(DIE_TRIGGER);
                Sprite.DOFade(0, FadeDuraion);
                UnsubscribeFromEvents();
            }
        }

        private void OnDamageTaken(TakeDamage signal)
        {
            if (signal.Enemy == EnemyLogic)
            {
                if (signal.Enemy.CurrentHealth <= 0)
                    return;

                Sprite.DOColor(Color.red, .5f).OnComplete(() => Sprite.color = Color.white);
            }
        }

        public void ActivateChaseAnim()
        {
            Animator.SetTrigger(CHASE_TRIGGER);
        }

        public void ActivateIdleAnim()
        {
            Animator.SetTrigger(IDLE_TRIGGER);
        }
    }
}