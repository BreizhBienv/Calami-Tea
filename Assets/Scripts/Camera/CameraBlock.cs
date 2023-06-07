using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBlock : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private bool ShouldBlockAxis;

    [SerializeField]
    private bool ShouldBlockCamera;

    [SerializeField]
    private GameObject Camera;

    [SerializeField]
    private float UpDistance;

    [SerializeField]
    private float RightDistance;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (ShouldBlockAxis)
            {
                Camera.GetComponent<CameraFollow>().IsBlock = true;
                Camera.GetComponent<CameraFollow>().StopALlMovement = false;
                Camera.GetComponent<CameraFollow>().BlockAxis = transform.right;
                Camera.GetComponent<CameraFollow>().StartPoint = transform.position - transform.forward * RightDistance + transform.up * UpDistance;
            }
            else if (ShouldBlockCamera)
            {
                Camera.GetComponent<CameraFollow>().IsBlock = false;
                Camera.GetComponent<CameraFollow>().StopALlMovement = true;
            }
            else
            {
                Camera.GetComponent<CameraFollow>().IsBlock = false;
                Camera.GetComponent<CameraFollow>().StartPoint = Camera.GetComponent<CameraFollow>().transform.position;
                Camera.GetComponent<CameraFollow>().StopALlMovement = false;
            }
        }
    }
}