using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    private GameObject obj;

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.LookAt(obj.transform);
    }
}