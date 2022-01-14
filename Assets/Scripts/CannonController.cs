using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnFire(CannonController cannonController);

public class CannonController : MonoBehaviour
{
    #region Events
    private static OnFire _onFire;

    public static event OnFire onFire
    {
        add { _onFire += value; }
        remove { _onFire -= value; }
    }
    #endregion

    [Header("References")]
    [SerializeField] private Transform cannonPivotPointTransform;
    [SerializeField] private Transform firePoint;

    [Header("Angle rotation limit")]
    [SerializeField] private bool invertArc;
    [SerializeField] private float minAngle;
    [SerializeField] private float maxAngle;

    [Header("FirePoint angle offset")]
    [SerializeField] private LeftRighEnum rotationOrientation;

    [Header("Movement")]
    [SerializeField] private float maxAngularVelocity;

    [Header("Fire data")]
    [SerializeField] private CannonBallController projectile_prefab;
    [SerializeField] private float muzzleSpeed;

    [Tooltip("Fire rate per second.")]
    [Range(0.1f, 100)]
    [SerializeField] private float fireRate;

    [Header("All Purpose")]
    [SerializeField] private bool xScaleInverted;

    private new Camera camera;
    private float lastShotTime = 0;
    private float firePointPivotPointOffsetAngle;

    private Vector2 cannonPivotPoint { get { return cannonPivotPointTransform.position; } }
    private int xScaleSign { get { return xScaleInverted ? -1 : 1; } }


    private void Awake()
    {
        camera = Camera.main;
        CalculateFirePointPivotPointOffsetAngle();
    }

    void Update()
    {       
        LookAtTarget2D(MousePosition());
        EvaluateFire();
    }

    private void EvaluateFire()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (!(Time.time - lastShotTime > 1 / fireRate))
            return;

        lastShotTime = Time.time;

        Fire();
    }

    private void Fire()
    {
        CannonBallController cannonBall = Instantiate(projectile_prefab, firePoint.position, firePoint.rotation);

        cannonBall.rb2d.velocity = cannonBall.transform.right * muzzleSpeed;

        _onFire?.Invoke(this);
    }

    private void LookAtTarget2D(Vector2 targetPoint)
    {
        Vector2 direction = (targetPoint - cannonPivotPoint).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        angle = ConvertFrom0To360(angle);

        angle = EvaluateAngularVelocity(angle);
        angle = ClampAngle(angle);
        angle = AddFirePointOffSetAngle(angle);

        angle = ConvertFrom0To360(angle);

        RotateTo(angle);
    }

    private float AddFirePointOffSetAngle(float angle)
    {
        float offsetAngle = firePointPivotPointOffsetAngle * (rotationOrientation == LeftRighEnum.LEFT ? 1 : -1);

        return angle + offsetAngle;
    }

    //private float AddFirePointOffSetAngle(Vector2 pivotDirection)
    //{
    //    float firePointOffSet = Vector2.Angle(firePoint.right, pivotDirection);
    //
    //    firePointOffSet *= (rotationOrientation == LeftRighEnum.LEFT ? 1 : -1);
    //
    //    return firePointOffSet;
    //}

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

    private void CalculateFirePointPivotPointOffsetAngle()
    {
        Vector2 pivotToFireDirection = ((Vector2)firePoint.position - cannonPivotPoint).normalized;

        firePointPivotPointOffsetAngle = Vector2.Angle(pivotToFireDirection, cannonPivotPointTransform.right);
    }

    private Vector2 MousePosition()
    {
        return camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
