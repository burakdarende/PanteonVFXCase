using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackVFXController : MonoBehaviour
{ [Header("Refs")]
    [SerializeField] private Transform muzzlePoint;
    [SerializeField] private Transform targetPoint;

    [Header("VFX")]
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private ParticleSystem projectile;
    [SerializeField] private ParticleSystem smokeTrail;

    [Header("Test")]
    [SerializeField] private KeyCode fireKey = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(fireKey))
            Fire();
    }

    public void Fire()
    {
        if (muzzlePoint == null) return;

        // Spawn & align
        SetTransform(muzzleFlash, muzzlePoint);
        SetTransform(projectile, muzzlePoint);
        SetTransform(smokeTrail, muzzlePoint);

        // Aim (optional)
        if (targetPoint != null)
        {
            Vector3 dir = (targetPoint.position - muzzlePoint.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                if (projectile) projectile.transform.rotation = rot;
                if (smokeTrail) smokeTrail.transform.rotation = rot;
            }
        }

        // Restart particles cleanly
        Restart(muzzleFlash);
        Restart(projectile);
        Restart(smokeTrail);
    }

    private static void SetTransform(ParticleSystem ps, Transform t)
    {
        if (!ps) return;
        ps.transform.SetPositionAndRotation(t.position, t.rotation);
    }

    private static void Restart(ParticleSystem ps)
    {
        if (!ps) return;
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Play(true);
    }
}
