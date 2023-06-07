using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mouvements : MonoBehaviour
{
    [Header("Mouvement's settings")]
    [SerializeField] private float _baseSpeed = 5;

    [SerializeField] private float _slowSpeed = 2;

    [SerializeField] public bool isInSlow;

    [SerializeField] private GameObject _mainCamera;
    private float _currentSpeed;
    private Rigidbody _rb;
    private Vector3 _input;

    // Start is called before the first frame update
    private void Start()
    {
        isInSlow = false;
        _rb = this.GetComponent<Rigidbody>();
        _currentSpeed = _baseSpeed;
    }

    private void Update()
    {
        Look();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Movements();
    }

    private void Movements()
    {
        Vector3 vec = Vector3.Slerp(this.transform.position, this.transform.position + (this.transform.forward * _input.magnitude) * (isInSlow ? _slowSpeed : _currentSpeed) * Time.fixedDeltaTime, 0.2f);
        _rb.MovePosition(vec);
    }

    private void Look()
    {
        if (_input != Vector3.zero)
        {
            //var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);

            var rot = _mainCamera.transform.rotation.eulerAngles;
            Quaternion rotation = Quaternion.Euler(0, rot.y, 0);
            Matrix4x4 isoMatrix = Matrix4x4.Rotate(rotation);
            Vector3 result = isoMatrix.MultiplyPoint3x4(_input);

            this.transform.rotation = Quaternion.LookRotation(result, Vector3.up);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (Time.timeScale != 0 && GetComponent<BaseCharacter>().CharacterState != BaseCharacter.MainStates.Dead &&
            GetComponent<BaseCharacter>().CharacterState != BaseCharacter.MainStates.Stun &&
            GetComponent<BaseCharacter>().CharacterState != BaseCharacter.MainStates.Attacking)
        {
            _input = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
            if (context.ReadValue<Vector2>().x != 0 || context.ReadValue<Vector2>().y != 0)
            {
                GetComponent<BaseCharacter>().SetCurrentState(BaseCharacter.MainStates.Moving);
            }
            else if (GetComponent<BaseCharacter>().CharacterState == BaseCharacter.MainStates.Moving)
            {
                GetComponent<BaseCharacter>().SetCurrentState(BaseCharacter.MainStates.Idle);
            }
        }
        else
        {
            _input = new Vector3(0, 0, 0);
        }
    }

    public float CurrentSpeed { set => _currentSpeed = value; }
    public float BaseSpeed { get => _baseSpeed; }

    public Vector3 Input { get => _input; }
}