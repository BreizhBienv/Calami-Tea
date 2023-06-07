using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeaCrack : BaseItem
{
    [SerializeField]
    float MoveSpeedModifier;

    [SerializeField]
    float DamageModifier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PickUp(BaseCharacter Character)
    {
        Character.SetMoveSpeed(MoveSpeedModifier);
        Character.SetAttackDamage(DamageModifier);
        Destroy(this.gameObject);
    }
}
