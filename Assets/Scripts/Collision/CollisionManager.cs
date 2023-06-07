using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private BaseCharacter Player;

    private void OnCollisionExit(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Interactible":
                Player.GetComponent<PlayerComponent>().InteractibleObjectCollisioned = null;
                break;

            case "Player":
                Player.GetComponent<PlayerComponent>().TeamMate = null;
                break;

            default:
                break;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        switch (other.gameObject.tag)
        {
            case "Interactible":
                Player.GetComponent<PlayerComponent>().InteractibleObjectCollisioned = other.gameObject;
                break;

            case "PickUp":
                other.transform.root.GetComponentInChildren<BaseItem>().PickUp(Player);
                Destroy(other.gameObject);
                break;

            case "Player":
                PlayerComponent com = other.gameObject.GetComponent<PlayerComponent>();
                if (com.CharacterState == BaseCharacter.MainStates.Dead)
                    Player.GetComponent<PlayerComponent>().TeamMate = other.gameObject.GetComponent<PlayerComponent>();
                other.gameObject.GetComponent<PlayerComponent>().TeamMate = Player.GetComponent<PlayerComponent>();
                break;

            default:
                break;
        }
        if (Player.tag.Contains("Player"))
        {
            if (Player.GetComponent<PlayerComponent>().IsSpinning && other.gameObject.tag.Contains("Enemy") && Vector3.Distance(Player.transform.position, other.gameObject.transform.position) < 10)
            {
                other.gameObject.GetComponent<BaseCharacter>().TakeDamage(Player.GetComponent<PlayerComponent>().SpinDamage);
                other.gameObject.GetComponent<Rigidbody>().AddForce(Player.GetComponent<PlayerComponent>().SpinKnockback * Player.transform.forward);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Oil":
                {
                    Player.GetComponent<Mouvements>().isInSlow = true;
                    break;
                };
        };
    }

    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Oil":
                {
                    Player.GetComponent<Mouvements>().isInSlow = false;
                    break;
                };
        };
    }
}