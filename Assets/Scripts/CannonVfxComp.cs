using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonVfxComp : MonoBehaviour
{
    [Header("Fire Effect")]
    [SerializeField] private ParticleSystem smoke_01;
    [SerializeField] private ParticleSystem smoke_02;
    [SerializeField] private ParticleSystem fire;

    private void Awake()
    {
        CannonController.onFire += OnFire;
    }

    private void OnFire(CannonController cannonController)
    {
        print("ok");
        smoke_01.Play();
        smoke_02.Play();
        fire.Play();
    }
}
