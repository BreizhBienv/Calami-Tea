using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeaBomb : MonoBehaviour
{
    [Header("Explosion Values")]
    [SerializeField]
    private float ExplosionTime;

    [SerializeField]
    private SphereCollider ExpandingSphere;

    [Header("Explosion Damage")]
    [SerializeField]
    private float FullExplosionDamage;

    [SerializeField]
    private float HalfExplosionDamage;

    [SerializeField]
    private float QuarterExplosionDamage;

    [Header("Explosion Range")]
    [SerializeField]
    private float FullExplosionRange;

    [SerializeField]
    private float HalfExplosionRange;

    [SerializeField]
    private float QuarterExplosionRange;

    private void Start()
    {
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(ExplosionTime);
        UpdateSphere();
    }

    private void UpdateSphere()
    {
        while (ExpandingSphere.radius < QuarterExplosionRange)
        {
            ExpandingSphere.radius += 1;
        }
        StartCoroutine(ResetSphereCollider());
        StartCoroutine(Delete());
        //Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<BaseCharacter>() != null)
        {
            if (Vector3.Distance(other.gameObject.transform.position, transform.position) <= FullExplosionRange)
            {
                other.gameObject.GetComponent<BaseCharacter>().TakeDamage(FullExplosionDamage);
            }
            else if (Vector3.Distance(other.gameObject.transform.position, transform.position) <= HalfExplosionRange)
            {
                other.gameObject.GetComponent<BaseCharacter>().TakeDamage(HalfExplosionDamage);
            }
            else if (Vector3.Distance(other.gameObject.transform.position, transform.position) <= QuarterExplosionRange)
            {
                other.gameObject.GetComponent<BaseCharacter>().TakeDamage(QuarterExplosionDamage);
            }
        }
    }

    private IEnumerator ResetSphereCollider()
    {
        yield return new WaitForSeconds(0.4f);
        ExpandingSphere.radius = 0;
    }

    private IEnumerator Delete()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}