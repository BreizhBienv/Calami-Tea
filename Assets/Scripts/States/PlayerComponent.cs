using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerComponent : BaseCharacter
{
    [SerializeField]
    private Transform IteractiblePoint;

    public Healthbar healthbar;

    private enum Attacks
    {
        LightAttack = 0,
        HeavyAttack = 1,
        SpecialAttack = 2,
        None = 3
    };

    private Attacks attacks = Attacks.None;
    private SpecialAttack Specialattacks = SpecialAttack.None;

    private enum SpecialAttack
    {
        TeaGrenade = 0,
        Bait = 1, //not implemented
        Spin = 2,
        None = 3
    };

    public GameObject InteractibleObjectCollisioned;

    public GameObject InteractibleObjectPicked;

    public PlayerComponent TeamMate;

    [Header("Basic Values")]
    [SerializeField]
    private int AmountOfTeaLost;

    [SerializeField]
    private float ThrowingForce;

    private void Start()
    {
        base.Start();
        PlayerComponent[] components = Spawner.FindObjectsByType<PlayerComponent>(FindObjectsSortMode.None);
        if (components.Length == 1)
        {
            healthbar = Spawner.FindAnyObjectByType<Healthbar>();

            if (healthbar != null)
            {
                healthbar.SetMaxHealth(MaxHealth);
            }
        }
    }

    private void Update()
    {
        if (!PlayerAnimator)
            PlayerAnimator = GetComponentInChildren<Animator>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (InteractibleObjectCollisioned && !InteractibleObjectPicked) //pickup
            {
                SetCurrentState(MainStates.Interaction);
                InteractibleObjectPicked = InteractibleObjectCollisioned;
                InteractibleObjectPicked.transform.parent = transform;
                InteractibleObjectPicked.GetComponent<Rigidbody>().isKinematic = true;

                InteractibleObjectPicked.transform.SetPositionAndRotation(IteractiblePoint.position, Quaternion.identity);
            }
            else if (InteractibleObjectPicked)
            {
                InteractibleObjectPicked.transform.parent = null;
                InteractibleObjectPicked.GetComponent<Rigidbody>().isKinematic = false;
                InteractibleObjectPicked.GetComponent<Rigidbody>().AddForce(ThrowingForce * transform.forward, ForceMode.Force);
                InteractibleObjectPicked = null;
                SetCurrentState(MainStates.Idle);
            }

            if (TeamMate)
            {
                TeamMate.Revive();
                TeamMate = null;
            }
        }
    }

    public void Die(InputAction.CallbackContext context)
    {
        TakeDamage(1000);
    }

    public void LightAttack(InputAction.CallbackContext context)
    {
        if (CharacterState == MainStates.Idle || CharacterState == MainStates.Moving)
        {
            if (context.performed)
            {
                SetCurrentState(MainStates.Attacking);
                StartCoroutine(LightAttackCoroutine());
            }
        }
    }

    [Header("Light Attacks")]
    [SerializeField]
    private float LightAttackRadius;

    [SerializeField]
    private float LightAttackWaitTime;

    [SerializeField]
    private float LightAttacksSpecialMoveRegen;

    [SerializeField]
    private float LightAttackDamage;

    public IEnumerator LightAttackCoroutine()
    {
        attacks = Attacks.LightAttack;
        PlayerAnimator.SetInteger("AttackState", ((int)Attacks.LightAttack));
        yield return new WaitForSeconds(LightAttackWaitTime);
        if (CharacterState == MainStates.Attacking)
        {
            if (CharacterState != MainStates.Stun)
            {
                print("Light Attack!");
                List<GameObject> HitEnemies = SphereCastInFront(LightAttackRadius);

                foreach (GameObject enemy in HitEnemies)
                {
                    BaseCharacter baseCharacter = enemy.GetComponent<BaseCharacter>();

                    if (baseCharacter != null)
                        baseCharacter.TakeDamage(LightAttackDamage);
                    else if (enemy.GetComponent<Boss>())
                    {
                        enemy.GetComponent<Boss>().TakeDamage(LightAttackDamage);
                    }
                    this.ChargeSpecialMove(LightAttacksSpecialMoveRegen);
                }
            }

            yield return new WaitForSeconds(0.75f);
        }
        print("Done Light Attack!");
        attacks = Attacks.None;
        PlayerAnimator.SetInteger("AttackState", ((int)Attacks.None));
        if (CharacterState == MainStates.Attacking)
            SetCurrentState(MainStates.Idle);
    }

    [Header("Heavy Attacks")]
    [SerializeField]
    private float HeavyAttackRadius;

    [SerializeField]
    private float HeavyAttackDamage;

    [SerializeField]
    private float HeavyAttackForce;

    [SerializeField]
    private float HeavyAttacksSpecialMoveRegen;

    [SerializeField]
    private float HeavyAttackWaitTime;

    public void HeavyAttack(InputAction.CallbackContext context)
    {
        if (CharacterState == MainStates.Idle || CharacterState == MainStates.Moving)
        {
            if (context.performed)
            {
                SetCurrentState(MainStates.Attacking);
                StartCoroutine(HeavyAttackCoroutine());
            }
        }
    }

    public IEnumerator HeavyAttackCoroutine()
    {
        if (CharacterState != MainStates.Stun)
        {
            attacks = Attacks.HeavyAttack;
            PlayerAnimator.SetInteger("AttackState", ((int)Attacks.HeavyAttack));

            yield return new WaitForSeconds(HeavyAttackWaitTime);
            if (CharacterState == MainStates.Attacking)
            {
                print("Heavy Attack!");
                List<GameObject> HitEnemies = SphereCastInFront(HeavyAttackRadius);

                foreach (GameObject enemy in HitEnemies)
                {
                    if (enemy.GetComponent<BaseCharacter>())
                    {
                        enemy.GetComponent<BaseCharacter>().TakeDamage(HeavyAttackDamage);
                    }
                    else if (enemy.GetComponent<Boss>())
                    {
                        enemy.GetComponent<Boss>().TakeDamage(HeavyAttackDamage);
                    }

                    this.ChargeSpecialMove(HeavyAttacksSpecialMoveRegen);
                    enemy.GetComponent<Rigidbody>().AddForce(transform.forward * HeavyAttackForce);
                }

                // yield return new WaitForSeconds(1.15f);
            }
            attacks = Attacks.None;
            PlayerAnimator.SetInteger("AttackState", ((int)Attacks.None));
            if (CharacterState == MainStates.Attacking)
                SetCurrentState(MainStates.Idle);
        }
    }

    [Header("Tea Bomb Attack")]
    [SerializeField]
    private float TeaThrowTime;

    [SerializeField]
    private GameObject TeaObject;

    [SerializeField]
    private float ThrowForce;

    public void TeaBombAttack(InputAction.CallbackContext context)
    {
        if (CharacterState == MainStates.Idle && CurrentSpecialMoveMeter == SpecialMoveRequirement)
        {
            if (context.performed)
            {
                CurrentSpecialMoveMeter = 0;
                StartCoroutine(TeaBombCoroutine());
            }
        }
    }

    public IEnumerator TeaBombCoroutine()
    {
        SetCurrentState(MainStates.Attacking);
        Specialattacks = SpecialAttack.TeaGrenade;
        PlayerAnimator.SetInteger("SpecialAttackState", ((int)SpecialAttack.TeaGrenade));
        attacks = Attacks.SpecialAttack;
        PlayerAnimator.SetInteger("AttackState", ((int)Attacks.SpecialAttack));
        yield return new WaitForSeconds(TeaThrowTime);
        Vector3 Pos = transform.position;
        Pos.y += transform.localScale.y;
        Pos += transform.forward;
        GameObject Instance = Instantiate(TeaObject, Pos, Quaternion.identity);
        Instance.GetComponent<Rigidbody>().AddForce(ThrowForce * transform.forward);
        SetCurrentState(MainStates.Idle);
        attacks = Attacks.None;
        PlayerAnimator.SetInteger("AttackState", ((int)Attacks.None));
        Specialattacks = SpecialAttack.None;
        PlayerAnimator.SetInteger("SpecialAttackState", ((int)SpecialAttack.None));
    }

    [Header("Spin Attack")]
    public bool IsSpinning;

    [SerializeField]
    private bool CanSpin = true;

    [SerializeField]
    private float SpinTime;

    [SerializeField]
    private float SpinCooldown;

    public float SpinDamage;

    public float SpinKnockback;

    public void SpinAttack(InputAction.CallbackContext context)
    {
        if (CharacterState == MainStates.Idle && CurrentSpecialMoveMeter == SpecialMoveRequirement && CanSpin)
        {
            if (context.performed)
            {
                CurrentSpecialMoveMeter = 0;
                StartCoroutine(SpinCoroutine());
            }
        }
    }

    public IEnumerator SpinCoroutine()
    {
        attacks = Attacks.SpecialAttack;
        PlayerAnimator.SetInteger("AttackState", ((int)Attacks.SpecialAttack));
        Specialattacks = SpecialAttack.Spin;
        PlayerAnimator.SetInteger("SpecialAttackState", ((int)SpecialAttack.Spin));
        SetCurrentState(MainStates.Attacking);
        IsSpinning = true;
        CanSpin = false;
        yield return new WaitForSeconds(SpinTime);
        IsSpinning = false;
        attacks = Attacks.None;
        PlayerAnimator.SetInteger("AttackState", ((int)Attacks.None));
        Specialattacks = SpecialAttack.None;
        PlayerAnimator.SetInteger("SpecialAttackState", ((int)SpecialAttack.None));
        SetCurrentState(MainStates.Idle);
        yield return new WaitForSeconds(SpinCooldown);
        CanSpin = true;
    }

    private List<GameObject> SphereCastInFront(float Radius)
    {
        List<GameObject> HitObjects = new List<GameObject>();
        Collider[] Colliders = Physics.OverlapSphere(transform.position, Radius);

        Debug.Log("working" + Colliders.Length);

        foreach (Collider col in Colliders)
        {
            if (col.gameObject.tag.Contains("Enemy"))
            {
                HitObjects.Add(col.gameObject);
            }
            else if (col.gameObject.tag.Contains("Breakable"))
            {
                BreakObjectAndSpawnItem(col.gameObject);
            }
        }

        return HitObjects;
    }

    [Header("Parry")]
    [SerializeField]
    private float ParryCooldown;

    [SerializeField]
    private float ParryTime;

    private bool CanParry = true;
    private bool Parrying = false;

    public void Parry()
    {
        if (CanParry && (CharacterState == MainStates.Idle || CharacterState == MainStates.Moving))
        {
            StartCoroutine(ParryCoroutine());
        }
    }

    private IEnumerator ParryCoroutine()
    {
        SetCurrentState(MainStates.Parry);
        Parrying = true;
        yield return new WaitForSeconds(ParryTime);
        SetCurrentState(MainStates.Idle);
        Parrying = false;
        CanParry = false;
        yield return new WaitForSeconds(ParryCooldown);
        CanParry = true;
    }

    [SerializeField]
    private GameObject CoinObj;

    [SerializeField]
    private GameObject HealthObj;

    [SerializeField]
    private GameObject TeaCrackObj;

    [SerializeField]
    private GameObject InfiniteObj;

    private bool CanBreakBreakable = true;

    private void BreakObjectAndSpawnItem(GameObject Object)
    {
        if (!CanBreakBreakable)
            return;
        CanBreakBreakable = false;
        Vector3 StartPos = Object.transform.position;
        Destroy(Object);

        List<GameObject> items = new List<GameObject>();
        items.Add(CoinObj);
        items.Add(HealthObj);
        items.Add(TeaCrackObj);
        items.Add(InfiniteObj);

        int Numb = Random.Range(0, 3);
        int RandX = Random.Range(-20, 10);
        int RandZ = Random.Range(-10, 10);
        Vector3 Pos = StartPos;
        Pos.x += RandX;
        Pos.z += RandZ;
        GameObject Item = Instantiate(items[Numb], Pos, Quaternion.identity);
    }

    private IEnumerator TimeBetweenBreakiingobjects()
    {
        yield return new WaitForSeconds(1);
        CanBreakBreakable = true;
    }

    public override void TakeDamage(float damage)
    {
        if (Parrying)
        {
            return;
        }
        else
        {
            PlayerAnimator.SetInteger("AttackState", ((int)Attacks.None));
            PlayerAnimator.SetInteger("SpecialAttackState", ((int)SpecialAttack.None));

            attacks = Attacks.None;
            Specialattacks = SpecialAttack.None;
            StopAllCoroutines();
            base.TakeDamage(damage);
            if (healthbar)
                healthbar.SetHealth((int)CurrentHealth);
        }
    }

    public void Revive()
    {
        if (PlayerAnimator)
        {
            PlayerAnimator.SetTrigger("Reviving");
        }
        CurrentHealth = MaxHealth;
        SetCurrentState(MainStates.Idle);
    }

    public void SpawnSecondPlayer()
    {
        GameObject Player = null;
        PlayerComponent[] components = Spawner.FindObjectsByType<PlayerComponent>(FindObjectsSortMode.None);
        print("Count : " + components.Length);
        foreach (PlayerComponent comp in components)
        {
            if (comp.gameObject.transform.position != gameObject.transform.position)
            {
                Player = comp.gameObject;
            }
        }
        if (components.Length > 1)
            gameObject.transform.SetPositionAndRotation(Player.transform.position, Quaternion.identity);

        Spawner.FindFirstObjectByType<Spawner>().Players = FindObjectsOfType<PlayerComponent>();
    }
}