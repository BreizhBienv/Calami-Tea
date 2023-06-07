using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfVisible : MonoBehaviour
{
    // Start is called before the first frame update

    private Renderer m_Renderer;

    [SerializeField]
    private GameObject camera;

    private void Start()
    {
        if (GetComponent<Renderer>())
            m_Renderer = GetComponent<Renderer>();
        else
            m_Renderer = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_Renderer.isVisible)
        {

        }
        else
        {
            GameObject OtherActor;
            BaseCharacter[] otheractors = FindObjectsByType<BaseCharacter>(FindObjectsSortMode.None);
            if (otheractors.Length > 1)
            {
                if (otheractors[0] != this)
                {
                    OtherActor = otheractors[0].gameObject;
                }
                else
                {
                    OtherActor = otheractors[1].gameObject;
                }
                GetComponent<BaseCharacter>().TeleportBackintoView(OtherActor.transform.position - OtherActor.transform.forward * 2);
            }
        }
    }
}