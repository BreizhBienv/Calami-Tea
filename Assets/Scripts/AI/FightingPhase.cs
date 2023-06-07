using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

public class FightingPhase : MonoBehaviour
{
    public enum FightingStates
    {
        Idle = 0,
        Moving = 1,
        Attack = 2,
        PowerMove = 3
    }

    private FightingStates m_CurrenFightingState;
    private NavMeshAgent m_Agent;

    private List<GameObject> m_Players = new List<GameObject>();
    private GameObject m_CurrTarget;

    private Animator m_Animator;

    [Header("Stats")]
    [SerializeField] private float m_BaseMoveSpeed;
    [SerializeField] private float m_BaseAttackDamage;

    private float m_CurrentMoveSpeed;
    private float m_CurrentAttackDamage;

    [Header("Fighting Param")]
    [SerializeField] private float m_DelayBetweenLock = 5;

    [Header("Attack Param")]
    [SerializeField] private float m_AttackRange;
    [SerializeField] private float m_TimeBeforeAttack = 0.3f;
    [SerializeField] private float m_IdleTimeAfterAttack = 0.2f;
    [SerializeField] private float m_AttackReloadTime = 5.0f;
    [SerializeField] private float m_AttackRadius = 30;
    [SerializeField] private float m_AttackAngle = 3;
    private bool m_ShouldFindNewTarget = true;
    private bool m_InReload = false;

    [Header("Power Move Param")]
    [SerializeField] private float m_PowerCoolDown = 20;
    [SerializeField] private float m_PowerDuration = 4;
    private bool m_PowerInReload = false;

    private Coroutine m_CurrentCoroutine = null;

    public Coroutine GetCoroutine { get => m_CurrentCoroutine; }

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();

        m_Agent = GetComponent<NavMeshAgent>();

        PlayerComponent[] components = GameObject.FindObjectsOfType<PlayerComponent>();
        foreach (PlayerComponent comp in components)
            m_Players.Add(comp.gameObject);

        m_CurrentMoveSpeed = m_BaseMoveSpeed;
        m_CurrentAttackDamage = m_BaseAttackDamage;

        m_Agent.speed = m_CurrentMoveSpeed;

        SetFighting(FightingStates.Moving);
    }

    // Update is called once per frame
    void Update()
    {
        TargetLock();

        if (InAttackRange() && !m_InReload)
        {
            m_Agent.isStopped = true;
            m_Agent.ResetPath();

            if (m_PowerInReload)
                SetFighting(FightingStates.Attack);
            else
                SetFighting(FightingStates.PowerMove);
        }
    }

    private void SetFighting(FightingStates newFighting)
    {
        m_CurrenFightingState = newFighting;
    }

    public void FightingManager()
    {
        switch (m_CurrenFightingState)
        {
            case FightingStates.Idle:
                break;
            case FightingStates.Moving:
                MoveAgent();
                break;
            case FightingStates.Attack:
                if (m_CurrentCoroutine == null)
                    m_CurrentCoroutine = StartCoroutine(Attack());
                break;
            case FightingStates.PowerMove:
                if (m_CurrentCoroutine == null)
                    m_CurrentCoroutine = StartCoroutine(PowerMove());
                break;

            default:
                break;
        }
    }

    protected void TargetLock()
    {
        if (!m_ShouldFindNewTarget)
            return;

        PlayerComponent[] players = FindObjectsOfType<PlayerComponent>();

        PlayerComponent currentTarget = players[0];
        float currentDist = Vector3.Distance(this.transform.position, currentTarget.transform.position); ;

        foreach (PlayerComponent target in players)
        {
            if (target == currentTarget)
                continue;

            float dist = Vector3.Distance(this.transform.position, target.transform.position);

            if (dist <= currentDist)
            {
                currentDist = dist;
                currentTarget = target;
            }
        }

        m_CurrTarget = currentTarget.gameObject;

        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        m_ShouldFindNewTarget = false;

        yield return new WaitForSeconds(m_DelayBetweenLock);

        m_ShouldFindNewTarget = true; ;
    }

    private void MoveAgent()
    {
        if (!InAttackRange())
        {
            m_Agent.SetDestination(m_CurrTarget.transform.position);
            SetAnimState(FightingStates.Moving);
        }
        else
        {
            SetAnimState(FightingStates.Idle);
        }
    }

    protected bool InAttackRange()
    {
        float dist = Vector3.Distance(this.transform.position, m_CurrTarget.transform.position);

        if (dist <= m_AttackRange)
            return true;

        return false;
    }

    private IEnumerator Attack()
    {
        SetAnimState(FightingStates.Idle);

        yield return new WaitForSeconds(m_TimeBeforeAttack);

        SetAnimState(FightingStates.Attack);

        List<GameObject> Players = RaycastInFront(m_AttackAngle, m_AttackRadius);

        foreach (GameObject player in Players)
        {
            BaseCharacter bc = player.GetComponent<BaseCharacter>();

            if (bc != null && player.tag == "Player")
                bc.TakeDamage(m_CurrentAttackDamage);
        }

        yield return new WaitForSeconds(m_IdleTimeAfterAttack);

        m_CurrentCoroutine = null;
        StartCoroutine(Reloading());
        SetFighting(FightingStates.Moving);
    }

    private IEnumerator PowerMove()
    {
        SetAnimState(FightingStates.Idle);

        yield return new WaitForSeconds(m_TimeBeforeAttack);

        SetAnimState(FightingStates.PowerMove);
        GetComponent<BossPowerMove>().Activate = true;

        yield return new WaitForSeconds(m_PowerDuration);

        GetComponent<BossPowerMove>().Activate = false;

        yield return new WaitForSeconds(m_IdleTimeAfterAttack);

        m_CurrentCoroutine = null;
        StartCoroutine(ReloadPowerMove());
        SetFighting(FightingStates.Moving);
    }

    private IEnumerator ReloadPowerMove()
    {
        m_PowerInReload = true;

        yield return new WaitForSeconds(m_PowerCoolDown);

        m_PowerInReload = false;
    }

    private IEnumerator Reloading()
    {
        m_InReload = true;

        yield return new WaitForSeconds(m_AttackReloadTime);

        m_InReload = false;
    }

    private List<GameObject> RaycastInFront(float MaxAngle, float Radius)
    {
        List<GameObject> HitObjects = new List<GameObject>();
        RaycastHit Hit;
        Vector3 RaycastDirection = transform.forward * Radius;
        Vector3 LocalPos = transform.position;
        LocalPos.y += transform.localScale.y;
        float Angle = 0;
        while (Angle < MaxAngle)
        {
            Debug.DrawRay(LocalPos, RaycastDirection, Color.green, Radius);
            if (Physics.Raycast(LocalPos, RaycastDirection, out Hit, Radius))
            {
                if (Hit.collider.gameObject.tag == "Player")
                {
                    if (!HitObjects.Contains(Hit.collider.gameObject))
                        HitObjects.Add(Hit.collider.gameObject);
                }
            }
            Angle += 5;
            RaycastDirection = Quaternion.AngleAxis(5, Vector3.up) * RaycastDirection;
        }

        Angle = 0;
        RaycastDirection = transform.forward * Radius;
        while (Angle > -MaxAngle)
        {
            Debug.DrawRay(LocalPos, RaycastDirection, Color.green, Radius);
            if (Physics.Raycast(LocalPos, RaycastDirection, out Hit, Radius))
            {
                if (Hit.collider.gameObject.tag == "PLayer")
                {
                    if (!HitObjects.Contains(Hit.collider.gameObject))
                        HitObjects.Add(Hit.collider.gameObject);
                }
            }
            Angle -= 5;
            RaycastDirection = Quaternion.AngleAxis(-5, Vector3.up) * RaycastDirection;
        }

        return HitObjects;
    }

    public void SetAnimState(FightingStates state)
    {
        if (m_Animator)
            m_Animator.SetInteger("AnimState", ((int)state));
    }
}
