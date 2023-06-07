using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossPowerMove : MonoBehaviour
{
    [Header("Spinning")]
    [SerializeField] private float m_Damage = 0.1f;

    private SphereCollider m_TriggerZone;

    private List<BaseCharacter>m_Players = new List<BaseCharacter>();

    UnityEvent m_PowerMove;

    private bool m_PowerMoveActive = false;

    public bool Activate { set => m_PowerMoveActive = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (m_PowerMove == null)
            m_PowerMove = new UnityEvent();

        m_TriggerZone = GetComponent<SphereCollider>();
        m_TriggerZone.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_PowerMove != null && m_PowerMoveActive)
        {
            m_PowerMove.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        m_PowerMove.RemoveAllListeners();

        PlayerComponent pc = other.GetComponent<PlayerComponent>();

        if (other.tag == "Player" && pc != null)
            m_Players.Add(pc);

        m_PowerMove.AddListener(Spinning);
    }

    private void OnTriggerExit(Collider other)
    {
        m_PowerMove.RemoveAllListeners();

        PlayerComponent pc = other.GetComponent<PlayerComponent>();

        if (other.tag == "Player" && pc != null)
            m_Players.Remove(pc);

        m_PowerMove.AddListener(Spinning);
    }

    private void Spinning()
    {
        foreach (PlayerComponent player in m_Players)
        {
            player.TakeDamage(m_Damage);
        }
    }
}
