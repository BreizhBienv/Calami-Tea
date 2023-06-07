using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEnable : MonoBehaviour
{
    [SerializeField]
    private BoxCollider collider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableWeaponCollisions()
    {
        collider.enabled = true;
    }

    public void DisableWeaponCollisions()
    {
        collider.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject != this.gameObject)
        {
            other.GetComponentInParent<BaseCharacter>().TakeDamage(10);
            this.GetComponentInParent<BaseCharacter>().ChargeSpecialMove(25);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject != this.gameObject)
        {
            other.GetComponentInParent<BaseCharacter>().TakeDamage(10);
            this.GetComponentInParent<BaseCharacter>().ChargeSpecialMove(25);
        }
    }
}
