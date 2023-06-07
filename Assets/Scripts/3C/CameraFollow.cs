using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] public Transform _player;

    [Range(0.01f, 1.0f)]
    [SerializeField] private float _smoothFactor = 0.5f;

    public Vector3 _cameraOffset;

    //Additional Values
    public bool IsBlock;

    public bool StopALlMovement;

    public Vector3 BlockAxis;

    public Vector3 StartPoint;

    // Start is called before the first frame update
    private void Start()
    {
        /*_cameraOffset.x = 0;
        _cameraOffset.y = 10;
        _cameraOffset.z = -20;*/
        StopALlMovement = false;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (!StopALlMovement)
        {
            if (!IsBlock)
            {
                Vector3 newPos;

                newPos = _player.position + _cameraOffset;

                this.transform.position = Vector3.Slerp(this.transform.position, newPos, _smoothFactor / 10);
            }
            else
            {
                Vector3 Secondpoint = StartPoint + BlockAxis * 10;
                if (_player)
                {
                    Vector3 ProjectedPlayerPoint = Vector3.Project((_player.position - StartPoint), (Secondpoint - StartPoint)) + StartPoint;
                    this.transform.position = Vector3.Slerp(this.transform.position, ProjectedPlayerPoint, _smoothFactor / 10);
                }
            }
        }
    }
}