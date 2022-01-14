using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBallController : MonoBehaviour
{
    [SerializeField] private float destroyDistance;

    private Vector2 startPoint;

    public Rigidbody2D rb2d { get; private set; }

    private void Awake()
    {
        startPoint = transform.position;

        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        EvaluateDestroy();
    }

    private void EvaluateDestroy()
    {
        if(Vector2.Distance(startPoint, transform.position) > destroyDistance)
        {
            Destroy(gameObject);
        }
    }
}
