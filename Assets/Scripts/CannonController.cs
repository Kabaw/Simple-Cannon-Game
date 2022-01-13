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

    private new Camera camera;

    private Vector2 cannonPivotPoint { get { return cannonPivotPointTransform.position; } }

    private void Awake()
    {
        camera = Camera.main;
    }

    void Update()
    {        
        LookAtTarget2D(MousePosition());
    }

    private void LookAtTarget2D(Vector2 targetPoint)
    {
        Vector2 direction = (targetPoint - cannonPivotPoint).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        float finalAngle = ClampAngle(rotation.eulerAngles.z);
        
        rotation = Quaternion.Euler(rotation.x, rotation.y, finalAngle);

        cannonPivotPointTransform.rotation = rotation;
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

    private Vector2 MousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
