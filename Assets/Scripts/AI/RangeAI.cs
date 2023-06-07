using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAI : BaseAI
{
    [Header("Range AI Parameters")]
    [SerializeField] private bool m_ShouldMove = true;
    [SerializeField] private float m_SecondBeforeFire = 0.3f;
    [SerializeField] private float m_SecondTimeToAim = 1.5f;
    [SerializeField] GameObject m_Bullet;
    [SerializeField] GameObject m_BulletSpawner;

    private bool m_ShouldLookAt = false;


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (m_ShouldLookAt)
            this.transform.LookAt(m_CurrentTarget.transform);
    }

    protected override void MoveAgent()
    {
        base.MoveAgent();

        if(m_ShouldMove && !InAttackRange())
            m_NavAgent.SetDestination(m_CurrentTarget.transform.position);

    }

    protected override IEnumerator Attack()
    {
        SetAnimState(MainStates.Idle);

        m_ShouldLookAt = true;

        yield return new WaitForSeconds(m_SecondTimeToAim);

        m_ShouldLookAt = false;

        yield return new WaitForSeconds(m_SecondBeforeFire);

        SetAnimState(MainStates.Attacking);
        Fire();

        yield return new WaitForSeconds(m_IdleTimeAfterAttack);
        SetAnimState(MainStates.Idle);

        StartCoroutine(Reloading());

        m_CurrentCoroutine = null;
        SetCurrentState(MainStates.Moving);


    }

    protected override void UpdateAttack()
    {

    }

    private void Fire()
    {
        Instantiate(m_Bullet, m_BulletSpawner.transform.position, this.transform.rotation);
    }
}
