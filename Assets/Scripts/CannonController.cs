using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonController : MonoBehaviour
{  
    [Header("References")]
    [SerializeField] private Transform cannonPivotPointTransform;

    [Header("Angle rotation limit")]
    //[SerializeField] private bool inverseArc;
    [SerializeField] private float angleLeftThreshholdOffset;
    [SerializeField] private float angleRightThreshholdOffset;

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
        bool leftOK = angle <= angleLeftThreshholdOffset;
        bool rightOK = 360 - angle >= angleRightThreshholdOffset;

        if (leftOK && rightOK)
            return angle;

        float leftDiff = angle - angleLeftThreshholdOffset;
        float rightDiff = 360 - angle - angleLeftThreshholdOffset;

        return leftDiff > rightDiff ? angleLeftThreshholdOffset : 360 - angleRightThreshholdOffset;
    }

    private Vector2 MousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
