using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cannonPivotPointTransform;

    [Header("Angle rotation limit")]
    [SerializeField] private bool invertArc;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [Header("Movement")]
    [SerializeField] private float maxAngularVelocity;

    [Header("All Purpuse")]
    [SerializeField] private bool xScaleInverted;

    private new Camera camera;

    private Vector2 cannonPivotPoint { get { return cannonPivotPointTransform.position; } }
    private int xScaleSign { get { return xScaleInverted ? -1 : 1; } }

    private void Awake()
    {
        camera = Camera.main;
    }

    void Update()
    {       
        //if(Input.GetMouseButtonDown(0))
            LookAtTarget2D(MousePosition());
    }

    private void LookAtTarget2D(Vector2 targetPoint)
    {
        Vector2 direction = (targetPoint - cannonPivotPoint).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle = ConvertFrom0To360(angle);

        angle = EvaluateAngularVelocity(angle);
        angle = ClampAngle(angle);

        RotateTo(angle);
    }

    private float EvaluateAngularVelocity(float angle)
    {
        float currentAngle = cannonPivotPointTransform.eulerAngles.z;
        float rotationAngle = Mathf.Abs(currentAngle - angle);
        float deltaTimeMaxAngularVelocity = maxAngularVelocity * Time.deltaTime;

        if (rotationAngle <= deltaTimeMaxAngularVelocity)
            return angle;       

        if (rotationAngle > 180)
        {
            if (currentAngle > angle)            
                angle = currentAngle + deltaTimeMaxAngularVelocity;
            else
                angle = currentAngle - deltaTimeMaxAngularVelocity;
        }
        else
        {
            if (currentAngle > angle)
                angle = currentAngle - deltaTimeMaxAngularVelocity;
            else
                angle = currentAngle + deltaTimeMaxAngularVelocity;
        }

        return ConvertFrom0To360(angle);
    }

    private float ClampAngle(float angle)
    {      
        if (!invertArc)
        {
            return Mathf.Clamp(angle, minAngle, maxAngle);
        }
        else
        {
            if (angle > minAngle && angle < maxAngle)
            {
                return angle - minAngle > maxAngle - angle ? maxAngle : minAngle;
            }
            else
            {
                return angle;
            }
        }
    }

    private void RotateTo(float angle)
    {
        float rotationAngle;
        float currentAngle = cannonPivotPointTransform.eulerAngles.z;

        rotationAngle = Math.Abs(currentAngle - angle);

        if (rotationAngle > 180)
        {
            rotationAngle = rotationAngle - 360;
            rotationAngle *= currentAngle > angle ? -1 : 1;
        }
        else
        {
            rotationAngle *= currentAngle > angle ? -1 : 1;
        }

        Vector3 rotation = new Vector3(0, 0, rotationAngle * xScaleSign);

        cannonPivotPointTransform.Rotate(rotation);
    }

    private float ConvertFrom0To360(float angle)
    {
        if(angle < 0)
        {
            while (angle < 0)
                angle = angle + 360.0f;
        }
        else
        {
            while (angle > 360f)
                angle = angle - 360.0f;
        }

        return angle;
    }

    private Vector2 MousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
