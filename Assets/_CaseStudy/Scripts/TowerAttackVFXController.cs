using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackVFXController : MonoBehaviour
{   
     [Header("VFX References (assign in Inspector)")]
    [SerializeField] private ParticleSystem muzzleFlashDirectional;
    [SerializeField] private ParticleSystem muzzleFlashCore;
    [SerializeField] private ParticleSystem projectilePs;

    [Header("Test")]
    [SerializeField] private KeyCode fireKey = KeyCode.Space;
    [SerializeField] private bool restartWithClear = true;

    private void Awake()
    {
        // Don't auto-play at scene start
        DisablePlayOnAwake(muzzleFlashDirectional);
        DisablePlayOnAwake(muzzleFlashCore);
        DisablePlayOnAwake(projectilePs);
    }

    private void Update()
    {
        if (Input.GetKeyDown(fireKey))
            Fire();
    }

    public void Fire()
    {
        Trigger(muzzleFlashDirectional);
        Trigger(muzzleFlashCore);
        Trigger(projectilePs);
    }

    private void Trigger(ParticleSystem ps)
    {
        if (!ps) return;

        if (restartWithClear)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        else
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        ps.Play(true);
    }

    private void DisablePlayOnAwake(ParticleSystem ps)
    {
        if (!ps) return;
        var main = ps.main;
        main.playOnAwake = false;
    }
}
