using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;
    Rigidbody rb;


    // Start is called before the first frame
    void Start()
    {
        Destroy(gameObject, timeToDestroy); // Auto destroy after 1 minute
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false; // Fly straight
            rb.drag = 0f;          // No slowdown
            rb.angularDrag = 0f;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // Prevent tilting
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponentInParent<EnemyHealth>())
        {
            EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
            enemyHealth.TakeDamage(weapon.damage);

            if(enemyHealth.health<=0 && enemyHealth.isDead == false)
            {
                Rigidbody rb = collision.gameObject.GetComponentInParent<Rigidbody>();
                rb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
                enemyHealth.isDead = true;
            }
        }
        Destroy(this.gameObject);
    }
}