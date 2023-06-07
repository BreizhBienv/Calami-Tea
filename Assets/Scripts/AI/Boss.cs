using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public enum Phase
    {
        CannonBall,
        Fighting,
        Death
    }

    [Header("Jump")]
    [SerializeField] private GameObject m_OnBridge;
    [SerializeField] private GameObject m_NearSteerWheel;

    public GameObject Bridge { get => m_OnBridge; }
    public GameObject NearSteerWheel { get => m_NearSteerWheel; }

    private Animator m_Animator;

    public Animator Animator { get => m_Animator; }

    [Header("Stats")]
    [SerializeField] private float m_BaseHealth = 100;

    private float m_CurrentHealth;

    private Phase m_CurrentPhase;

    private CannonPhase m_CannonPhase;
    private FightingPhase m_FightingPhase;
    private NavMeshAgent m_Agent;

    private int m_PhaseCounter = 1;

    public int PhaseCounter { get => m_PhaseCounter; set => m_PhaseCounter = value; }

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_CannonPhase = GetComponent<CannonPhase>();
        m_FightingPhase = GetComponent<FightingPhase>();
        m_Agent = GetComponent<NavMeshAgent>();

        m_CurrentHealth = m_BaseHealth;

        m_CurrentPhase = Phase.CannonBall;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CurrentHealth <= m_BaseHealth * (1 - 0.25f * m_PhaseCounter))
        {
            if (m_CurrentPhase == Phase.Fighting)
            {
                gameObject.transform.position = m_NearSteerWheel.transform.position;
                m_CannonPhase.SetPattern(CannonPhase.CannonPattern.Random);
                SetPhase(Phase.CannonBall);
            }

        }

        if (m_CurrentHealth <= 0)
        {
            m_Agent.isStopped = true;
            m_Agent.ResetPath();
            SetPhase(Phase.Death);
        }

        PhaseManager();
    }

    public void SetPhase(Phase newPhase)
    {
        m_CurrentPhase = newPhase;
    }

    private void PhaseManager()
    {
        switch (m_CurrentPhase)
        {
            case Phase.CannonBall:
                m_CannonPhase.PatternManager();
                break;
            case Phase.Fighting:
                m_FightingPhase.FightingManager();
                break;
            case Phase.Death:
                StartCoroutine(Death());
                break;
            default:
                break;
        }
    }

    public void TakeDamage(float damage)
    {
        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0)
        {
            SetPhase(Phase.Death);
            if (m_Animator)
                m_Animator.SetTrigger("Dead");
        }
    }

    private IEnumerator Death()
    {
        SetAnimState(Phase.Death);

        GetComponent<BossPowerMove>().Activate = false;

        if (m_CannonPhase.GetCoroutine != null)
            StopCoroutine(m_CannonPhase.GetCoroutine);

        if (m_FightingPhase.GetCoroutine != null)
            StopCoroutine(m_FightingPhase.GetCoroutine);

        yield return new WaitForSeconds(3);

        Destroy(gameObject, 0.1f);

        UnityEngine.AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Boss");
    }

    public void SetAnimState(Phase state)
    {
        if (m_Animator)
            m_Animator.SetInteger("PhaseState", ((int)state));
    }
}
