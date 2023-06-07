using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class BaseAI : BaseCharacter
{
    protected List<GameObject> m_Players = new List<GameObject>();
    protected GameObject m_CurrentTarget;
    protected NavMeshAgent m_NavAgent;

    [Header("Attack Parameters")]
    [SerializeField] protected float m_MaxRangeAttack = 0;

    [SerializeField] protected float m_TargetLockRange = 0;
    [SerializeField] private float m_SecondTimeBeforeAttack = 0.3f;
    [SerializeField] protected float m_IdleTimeAfterAttack = 0.5f;
    [SerializeField] private float m_SecondTimeToReload = 3.0f;
    protected bool m_Reloading = false;

    private bool m_PlayersInRange = false;

    protected Coroutine m_CurrentCoroutine = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        PlayerComponent[] Players = GameObject.FindObjectsOfType<PlayerComponent>();
        for (int i = 0; i < Players.Length; i++)
            m_Players.Add(Players[i].gameObject);

        m_NavAgent = GetComponent<NavMeshAgent>();
        m_NavAgent.stoppingDistance = m_MaxRangeAttack;
        SetCurrentState(MainStates.Moving);
        TargetLock();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (CharacterState == MainStates.Dead)
        {
            Destroy(gameObject);
            Destroy(this);
            enabled = false;
        }

        base.Update();

        if (!m_PlayersInRange && m_CurrentTarget == null)
            Idle();

        TargetLock();

        if (InAttackRange() && !m_Reloading)
        {
            if (!m_NavAgent || CharacterState == MainStates.Dead || CharacterState == MainStates.Stun)
                return;

            m_NavAgent.isStopped = true;
            m_NavAgent.ResetPath();
            SetCurrentState(MainStates.Attacking);
        }

        CallThroughState();

        IsDead();
    }

    private void CallThroughState()
    {
        switch (CharacterState)
        {
            case MainStates.Moving:
                MoveAgent();
                break;

            case MainStates.Dead:
                if (m_CurrentCoroutine == null)
                {
                    m_CurrentCoroutine = StartCoroutine(Death());
                }
                break;

            case MainStates.Attacking:
                UpdateAttack();
                if (m_CurrentCoroutine == null)
                {
                    m_CurrentCoroutine = StartCoroutine(Attack());
                }
                break;

            case MainStates.Idle:
                if (m_CurrentTarget)
                {
                    SetCurrentState(MainStates.Moving);
                }
                break;

            case MainStates.Interaction:
            default:
                break;
        }
    }

    protected void TargetLock()
    {
        GameObject currentTarget;
        PlayerComponent[] players = FindFirstObjectByType<Spawner>().Players;

        GameObject player1 = players[0].gameObject;
        GameObject player2;
        if (players.Length > 1)
        {
            player2 = players[1].gameObject;

            float dist1 = Vector3.Distance(gameObject.transform.position, player1.transform.position);
            float dist2 = Vector3.Distance(gameObject.transform.position, player2.transform.position);

            if (players[1].CharacterState == MainStates.Dead)
            {
                currentTarget = player1;
            }
            else if (players[0].CharacterState == MainStates.Dead)
            {
                currentTarget = player2;
            }
            else if (dist2 > dist1)
            {
                currentTarget = player1;
            }
            else
            {
                currentTarget = player2;
            }
        }
        else
            currentTarget = player1;

        float currentDist = Vector3.Distance(this.transform.position, currentTarget.transform.position); ;

        //foreach (GameObject target in m_Players)
        //{
        //    if (target == currentTarget)
        //        continue;

        //    float dist = Vector3.Distance(this.transform.position, target.transform.position);

        //    if (dist <= m_TargetLockRange && dist <= currentDist)
        //    {
        //        currentDist = dist;
        //        currentTarget = target;
        //    }
        //}

        m_CurrentTarget = currentTarget;
    }

    protected bool InAttackRange()
    {
        float dist = Vector3.Distance(this.transform.position, m_CurrentTarget.transform.position);

        if (dist <= m_MaxRangeAttack)
            return true;

        return false;
    }

    protected virtual void Idle()
    {
    }

    public void EndIdle()
    {
        SetCurrentState(MainStates.Moving);
    }

    protected virtual void MoveAgent()
    {
        if (!InAttackRange())
            SetAnimState(MainStates.Moving);
        else
            SetAnimState(MainStates.Idle);
    }

    protected virtual IEnumerator Attack()
    {
        SetCurrentState(MainStates.Idle);

        yield return new WaitForSeconds(m_SecondTimeBeforeAttack);

        SetCurrentState(MainStates.Attacking);
    }

    protected virtual void UpdateAttack()
    {
    }

    protected virtual IEnumerator Reloading()
    {
        m_Reloading = true;

        yield return new WaitForSeconds(m_SecondTimeToReload);

        m_Reloading = false;
    }

    protected void IsDead()
    {
        if (this.CurrentHealth > 0)
            return;

        StopCoroutine(m_CurrentCoroutine);
        SetCurrentState(MainStates.Dead);
    }

    protected virtual IEnumerator Death()
    {
        yield return new WaitForSeconds(0.5f);

        Destroy(this.gameObject);
    }

    public void SetAnimState(MainStates state)
    {
        CharacterState = state;
        if (PlayerAnimator)
            PlayerAnimator.SetInteger("AnimState", ((int)state));
    }
}