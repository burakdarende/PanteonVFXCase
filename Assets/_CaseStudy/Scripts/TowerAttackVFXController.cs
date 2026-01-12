using UnityEngine;

public class TowerAttackVFXController : MonoBehaviour
{   
    [Header("VFX Prefab Root")]
    [SerializeField] private ParticleSystem weaponVFXRoot; 

    [Header("Test Controls")]
    [SerializeField] private KeyCode fireKey = KeyCode.Space;

    private void Update()
    {
        if (Input.GetKeyDown(fireKey))
        {
            Fire();
        }
    }

    public void Fire()
    {
        if (weaponVFXRoot != null)
        {
            weaponVFXRoot.Play(true); 
        }
    }
}