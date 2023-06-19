using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField]
    Transform player;

    [SerializeField]
    float distance = 5;

    [SerializeField]
    float _yFocus = 0;

    private float _yRotatation;
    private float _xRotatation;

    public Quaternion PlanerRotation => Quaternion.Euler(0, _yRotatation, 0); 

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        _yRotatation += Input.GetAxis("Mouse X");
        _xRotatation += Input.GetAxis("Mouse Y");

        Quaternion targetRotation = Quaternion.Euler(Mathf.Clamp(_xRotatation, 5f, 30f), _yRotatation, 0);
        var focusPosition = player.position + new Vector3(0, _yFocus);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

}
