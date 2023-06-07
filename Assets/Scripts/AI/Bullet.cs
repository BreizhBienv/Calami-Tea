using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Parametes")]
    [SerializeField] private float m_Speed;
    [SerializeField] private int m_Damage;
    [SerializeField] private float m_BulletLife = 3.0f;

    private Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = this.GetComponent<Rigidbody>();
        StartCoroutine(BeginLifeCycle());
    }


    void FixedUpdate()
    {
        m_Rigidbody.MovePosition(this.transform.position + this.transform.forward * m_Speed * Time.fixedDeltaTime);
    }

    private void OnDestroy()
    {
    }

    public void OnCollisionEnter(Collision collision)
    {
        BaseCharacter bc = collision.gameObject.GetComponent<BaseCharacter>();

        if (bc != null && collision.gameObject.tag == "Player")
        {
            bc.TakeDamage(m_Damage);
            StopCoroutine(BeginLifeCycle());
            Destroy(this.gameObject);
        }
    }

    public IEnumerator BeginLifeCycle()
    {
        yield return new WaitForSeconds(m_BulletLife);
        Destroy(this.gameObject);
    }
}
