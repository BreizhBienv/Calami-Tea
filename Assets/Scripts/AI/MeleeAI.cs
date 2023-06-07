using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAI : BaseAI
{
    [Header("Attack Parameter")]
    [SerializeField] private float m_AttackRadius;

    [SerializeField] private float m_AttackAngle;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (CharacterState == MainStates.Idle)
        {
            SetCurrentState(MainStates.Moving);
        }
    }

    protected override void MoveAgent()
    {
        base.MoveAgent();

        if (!InAttackRange())
            m_NavAgent.SetDestination(m_CurrentTarget.transform.position);
    }

    protected override IEnumerator Attack()
    {
        base.Attack();

        SetAnimState(MainStates.Attacking);
        yield return new WaitForSeconds(0.75f);

        if (CharacterState != MainStates.Stun)
        {
            List<GameObject> Players = RaycastInFront(m_AttackAngle, m_AttackRadius);

            foreach (GameObject player in Players)
            {
                if (CharacterState == MainStates.Stun)
                {
                    break;
                }
                BaseCharacter bc = player.GetComponent<BaseCharacter>();

                if (bc != null && player.tag == "Player")
                    bc.TakeDamage(CurrentAttackDamage);
            }
        }

        yield return new WaitForSeconds(0.75f);
        SetAnimState(MainStates.Idle);

        m_CurrentCoroutine = null;

        StartCoroutine(Reloading());
        SetCurrentState(MainStates.Moving);
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
            Debug.DrawRay(LocalPos, RaycastDirection, Color.red, Radius);
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
            Debug.DrawRay(LocalPos, RaycastDirection, Color.red, Radius);
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
}