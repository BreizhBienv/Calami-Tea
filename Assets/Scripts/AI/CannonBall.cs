using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [Header("CannonBall Parameters")]
    [SerializeField] private float m_Radius = 1;
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_SecondBeforeHit = 4;

    private SphereCollider m_TriggerZone;

    private ShadowController m_Shader;

    // Start is called before the first frame update
    void Start()
    {
        m_Shader = FindObjectOfType<ShadowController>();
        m_TriggerZone = this.GetComponent<SphereCollider>();
        m_TriggerZone.isTrigger = true;
        m_TriggerZone.radius = m_Radius;
        m_TriggerZone.enabled = true;

        StartCoroutine(SpawnCannonBall());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnCannonBall()
    {
        if (m_Shader != null)
            m_Shader.Fire(new Vector3(this.transform.position.x, this.transform.position.y - 15, this.transform.position.z), m_Radius, m_SecondBeforeHit, 1, 1);
        yield return new WaitForSeconds(m_SecondBeforeHit);

        m_TriggerZone.enabled = true;

        yield return new WaitForEndOfFrame();

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseCharacter bs = other.GetComponent<BaseCharacter>();

        if (bs != null && bs.gameObject.tag == "Player")
        {
            bs.TakeDamage(m_Damage);
        }
    }
}
