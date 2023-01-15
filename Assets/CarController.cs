using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CarController : MonoBehaviour
{
    private bool isParked = false;

    [Header("Control Metrics")]
    public float rotationSpeed = 90;
    public float rotationSpeedFactor = 0.7f;
    public float maxForwardSpeed = 20;
    public float maxBackwardSpeed = -10;
    public float accelerationForward = 10;
    public float accelerationBackward = 10;
    public float decelerationSpeed = 0.9f;
    public float decelerationDelta = 0.3f;

    private float speed = 0;
    public bool IsStopped => speed == 0;

    private Vector3 controls;
    private Vector3 dir;

    [Header("Car Wheels")]
    public Transform frontRightWheel;
    public Transform frontLeftWheel;
    public float clampRotationAngle = 15;

    [Header("UI")]
    public TextMeshProUGUI speedTMP;

    private void Start()
    {
        dir = transform.forward;
    }

    void Update()
    {
        if (isParked) return;

        // Inputs
        ReadInputs();
        
        // "Physics" and direction
        CalculateDirection();
        ApplyInertiaAndAcceleration();

        // Move and Visual
        ControlWheels();
        Move();

        // Update UI
        UpdateSpeedUI();
    }

    private void UpdateSpeedUI()
    {
        speedTMP.text = $"{(int)speed} km/h";
    }

    private void ControlWheels()
    {
        // Rotate wheels
        frontRightWheel.LookAt(frontRightWheel.position + dir, Vector3.left);
        frontLeftWheel.LookAt(frontLeftWheel.position + dir, Vector3.left);
    }

    private void ReadInputs()
    {
        var horizontalAxis = 0;
        var verticalAxis = 0;

        horizontalAxis += Input.GetKey(KeyCode.D) ? 1 : 0;
        horizontalAxis += Input.GetKey(KeyCode.Q) ? -1 : 0;

        verticalAxis += Input.GetKey(KeyCode.Z) ? 1 : 0;
        verticalAxis += Input.GetKey(KeyCode.S) ? -1 : 0;

        controls.x = horizontalAxis;
        controls.z = verticalAxis;
    }

    private void ApplyInertiaAndAcceleration()
    {
        // Vitesse propre à l'input
        float celeretion = controls.z > 0 ? accelerationForward : accelerationBackward;
        speed += celeretion * controls.z * Time.deltaTime;

        // Décélération propre à la vitesse de base (hors input)
        if (controls.z == 0 && speed != 0)
        {
            speed = Mathf.Lerp(speed, speed * decelerationSpeed * Time.deltaTime, decelerationDelta);
            if (speed < 0.05f && speed >  -0.05f)
            {
                speed = 0;
            }
        }

        // Clamp la vitesse selon si la voiture avance ou recule
        speed = Mathf.Clamp(speed, maxBackwardSpeed, maxForwardSpeed);
    }

    private void CalculateDirection()
    {
        // Rotate steering wheel (volant de voiture)
        dir = Quaternion.AngleAxis(controls.x * rotationSpeed * Time.deltaTime, Vector3.up) * dir;

        float currentCarAngle = Vector3.Angle(transform.forward, dir);
        // Clamp rotation
        if (currentCarAngle > clampRotationAngle)
        {
            dir = Quaternion.AngleAxis(-controls.x * rotationSpeed * Time.deltaTime, Vector3.up) * dir;
        }
        Debug.DrawRay(transform.position, dir * 10, Color.red);
    }

    private void Move()
    {
        transform.LookAt(transform.position + dir * (Mathf.Abs(speed) * rotationSpeedFactor) * Time.deltaTime);
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    public void Park()
    {
        isParked = true;
    }
}
