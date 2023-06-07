using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    public enum MainStates
    {
        Idle = 0,
        Moving = 1,
        Interaction = 2,
        Attacking = 3,
        Stun = 4,
        Parry = 5,
        Dead = 6
    };

    public float MaxHealth = 100;
    [SerializeField] protected float BaseMoveSpeed = 100;
    [SerializeField] public float BaseAttackDamage = 100;
    [SerializeField] public float StunCooldownTime = 2;
    [SerializeField] public float SpecialMoveRequirement = 100;
    [SerializeField] public float IncomingDamageSpecialMoveRegen;
    protected Animator PlayerAnimator;

    protected float CurrentAttackDamage;
    public float CurrentHealth;
    protected float CurrentMoveSpeed;
    protected float CurrentCoins;
    protected float CurrentSpecialMoveMeter;

    private MainStates characterState = MainStates.Idle;

    public MainStates CharacterState { get => characterState; set => characterState = value; }

    protected virtual void Start()
    {
        CurrentHealth = MaxHealth;
        CurrentAttackDamage = BaseAttackDamage;
        CurrentMoveSpeed = 30;
        PlayerAnimator = GetComponentInChildren<Animator>();
        ManaBar Mana = Spawner.FindAnyObjectByType<ManaBar>();

        if (Mana != null)
        {
            Mana.SetMaxStamina(100);
            Mana.SetStamina(0);
        }
    }

    protected virtual void Update()
    {
        if (!PlayerAnimator)
            PlayerAnimator = GetComponentInChildren<Animator>();
    }

    public virtual void TakeDamage(float damage)
    {
        if (CurrentHealth - damage <= 0)
        {
            CurrentHealth = 0;
            SetCurrentState(MainStates.Dead);
        }
        else
        {
            SetCurrentState(MainStates.Stun);

            if (PlayerAnimator)
                PlayerAnimator.SetTrigger("Hit");

            CurrentHealth -= damage;

            this.ChargeSpecialMove(IncomingDamageSpecialMoveRegen);
            StartCoroutine(StunCooldown());
        }

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            SetCurrentState(MainStates.Dead);
        }

        if (characterState == MainStates.Stun)
        {
            StartCoroutine(StunCooldown());
        }
    }

    public void SetMoveSpeed(float MoveSpeed)
    {
        CurrentMoveSpeed += MoveSpeed;
        GetComponent<Mouvements>().CurrentSpeed = CurrentMoveSpeed;
    }

    public void SetAttackDamage(float AttackDamage)
    {
        CurrentAttackDamage += AttackDamage;
    }

    public void GetHealth(float HealthAmount)
    {
        if (CurrentHealth + HealthAmount < MaxHealth)
        {
            CurrentHealth += HealthAmount;
        }
        else
        {
            CurrentHealth = MaxHealth;
        }
    }

    public void GetCoins(float CoinsAmount)
    {
        CurrentCoins += CoinsAmount;
    }

    public void SetCurrentState(MainStates state)
    {
        CharacterState = state;
        if (PlayerAnimator)
            PlayerAnimator.SetInteger("CharacterState", ((int)state));
    }

    public IEnumerator StunCooldown()
    {
        yield return new WaitForSeconds(StunCooldownTime);
        SetCurrentState(MainStates.Idle);
    }

    public void ChargeSpecialMove(float Amount)
    {
        ManaBar manaBar = Spawner.FindAnyObjectByType<ManaBar>();

        if (CurrentSpecialMoveMeter + Amount > SpecialMoveRequirement)
        {
            if (manaBar)
                manaBar.SetMaxStamina((int)CurrentSpecialMoveMeter);
            CurrentSpecialMoveMeter = SpecialMoveRequirement;
        }
        else
        {
            CurrentSpecialMoveMeter += Amount;
            if (manaBar)
                manaBar.SetStamina((int)CurrentSpecialMoveMeter);
        }
    }

    public void TeleportBackintoView(Vector3 point)
    {
        Vector3 lerpedPos = Vector3.Lerp(transform.position, point, 1f);
        transform.position = lerpedPos;
    }
}