using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class CannonPhase : MonoBehaviour
{
    public enum CannonPattern
    {
        Random,
        Pentagon,
        Cross,
        Star,
        Line,
        Target
    }

    private Animator m_Animator;

    [Header("Cannon Parameters")]
    [SerializeField] private GameObject m_CannonBall;
    [SerializeField] private float m_RespiteAfterSalvo = 2;
    [SerializeField] private float m_CanonHight;

    private List<GameObject> m_Players = new List<GameObject>();

    [Header("Random Parameters")]
    [SerializeField] private GameObject m_ZoneToSpawnCannon;
    [SerializeField] private float m_TimeBetweenRandomSpawn = 0.3f;
    [SerializeField] private int m_NumberOfRandomTypeToSpawn = 10;
    Vector3 m_Origin;
    Vector3 m_Range;

    [Header("Pentagon Parameters")]
    [SerializeField] private GameObject m_PentagonPattern;

    [Header("Cross Parameters")]
    [SerializeField] private GameObject m_CrossPattern;

    [Header("Star Parameters")]
    [SerializeField] private GameObject m_StarPattern;

    [Header("Line Parameters")]
    [SerializeField] private List<GameObject> m_LinePatterns;
    [SerializeField] private float m_TimeBetweenEachLine = 0.3f;

    [Header("Target Parameters")]
    [SerializeField] private float m_TimeBetweenTargetSpawn = 0.1f;
    [SerializeField] private int m_NumberOfTargetTypeToSpawn = 10;


    private Coroutine m_CurrentCoroutine = null;
    private CannonPattern m_CurrentPattern;

    private NavMeshAgent m_Agent;

    public Coroutine GetCoroutine { get => m_CurrentCoroutine; }

    // Start is called before the first frame update
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponentInChildren<Animator>();

        SpawnZoneSet();
        PlayerComponent[] components = GameObject.FindObjectsOfType<PlayerComponent>();
        foreach (PlayerComponent comp in components)
            m_Players.Add(comp.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPattern(CannonPattern newPattern)
    {
        m_CurrentPattern = newPattern;
        if (m_Animator)
            m_Animator.SetTrigger("Salve");
    }

    public void PatternManager()
    {
        if (m_CurrentCoroutine != null)
            return;

        switch (m_CurrentPattern)
        {
            case CannonPattern.Random:
                m_CurrentCoroutine = StartCoroutine(SpawnRandom());
                break;
            case CannonPattern.Pentagon:
                m_CurrentCoroutine = StartCoroutine(SpawnPattern(m_PentagonPattern, CannonPattern.Cross));
                break;
            case CannonPattern.Cross:
                m_CurrentCoroutine = StartCoroutine(SpawnPattern(m_PentagonPattern, CannonPattern.Star));
                break;
            case CannonPattern.Star:
                m_CurrentCoroutine = StartCoroutine(SpawnPattern(m_PentagonPattern, CannonPattern.Line));
                break;
            case CannonPattern.Line:
                m_CurrentCoroutine = StartCoroutine(SpawLines());
                break;
            case CannonPattern.Target:
                m_CurrentCoroutine = StartCoroutine(SpawnTarget());
                break;

            default:
                break;
        }
    }

    private IEnumerator SpawnRandom()
    {
        SetPattern(CannonPattern.Pentagon);
        GetComponent<Boss>().SetAnimState(Boss.Phase.CannonBall);

        for (int i = 0; i <= m_NumberOfRandomTypeToSpawn; ++i)
        {
            SpawnRandomTypeCannonBall();

            yield return new WaitForSeconds(m_TimeBetweenRandomSpawn);
        }

        yield return new WaitForSeconds(m_RespiteAfterSalvo);

        m_CurrentCoroutine = null;
    }

    private void SpawnRandomTypeCannonBall()
    {
        //calcul random pos
        Vector3 RandomRange = new Vector3(Random.Range(-m_Range.x, m_Range.x),
                                    m_CanonHight,
                                    Random.Range(-m_Range.z, m_Range.z));


        Vector3 RandomCoordinate = m_Origin + RandomRange;

        Instantiate(m_CannonBall, RandomCoordinate, Quaternion.identity);
    }

    private void SpawnZoneSet()
    {
        m_Origin = m_ZoneToSpawnCannon.transform.position;
        m_Range = m_ZoneToSpawnCannon.transform.localScale / 2.0f;
    }

    private IEnumerator SpawnTarget()
    {
        GetComponent<Boss>().SetAnimState(Boss.Phase.CannonBall);

        for (int i = 0; i <= m_NumberOfTargetTypeToSpawn; ++i)
        {
            SpawnTargetTypeCannonBall();

            yield return new WaitForSeconds(m_TimeBetweenTargetSpawn);
        }

        yield return new WaitForSeconds(m_RespiteAfterSalvo);

        m_CurrentCoroutine = null;
        GetComponent<Boss>().SetPhase(Boss.Phase.Fighting);
        GetComponent<Boss>().SetAnimState(Boss.Phase.Fighting);
        gameObject.transform.position = GetComponent<Boss>().Bridge.transform.position;
        GetComponent<Boss>().PhaseCounter++;
    }

    private void SpawnTargetTypeCannonBall()
    {
        int random = Random.Range(0, m_Players.Count - 1);
        PlayerComponent[] players = FindObjectsOfType<PlayerComponent>();
        Vector3 vector3 = new Vector3(players[random].transform.position.x, m_CanonHight, players[random].transform.position.z);
        Instantiate(m_CannonBall, vector3, Quaternion.identity);
    }

    private IEnumerator SpawnPattern(GameObject pattern, CannonPattern newPattern)
    {
        SetPattern(newPattern);
        GetComponent<Boss>().SetAnimState(Boss.Phase.CannonBall);

        float randomYaw = Random.Range(0, 360);
        pattern.transform.Rotate(0, randomYaw, 0, Space.World);

        List<GameObject> children = GetChildren(pattern);

        foreach (GameObject child in children)
        {
            Instantiate(m_CannonBall, new Vector3(child.transform.position.x, m_CanonHight, child.transform.position.z), Quaternion.identity);
        }

        yield return new WaitForSeconds(m_RespiteAfterSalvo);

        m_CurrentCoroutine = null;
    }

    private List<GameObject> GetChildren(GameObject pattern)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform t in pattern.transform)
            children.Add(t.gameObject);

        return children;
    }

    private IEnumerator SpawLines()
    {
        SetPattern(CannonPattern.Target);
        GetComponent<Boss>().SetAnimState(Boss.Phase.CannonBall);

        foreach (GameObject line in m_LinePatterns)
        {
            SpawnLineTypeCannonBall(line);
            yield return new WaitForSeconds(m_TimeBetweenEachLine);
        }

        yield return new WaitForSeconds(m_RespiteAfterSalvo);

        m_CurrentCoroutine = null;
    }

    private void SpawnLineTypeCannonBall(GameObject line)
    {
        List<GameObject> children = GetChildren(line);

        foreach (GameObject child in children)
        {
            Instantiate(m_CannonBall, new Vector3(child.transform.position.x, m_CanonHight, child.transform.position.z), Quaternion.identity);
        }
    }
}
