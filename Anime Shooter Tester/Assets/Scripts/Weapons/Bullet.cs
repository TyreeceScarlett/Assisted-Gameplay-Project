using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 5f; // ✅ Bullet auto destroys after 5s
    [HideInInspector] public WeaponManager weapon; // ✅ Reference to weapon
    [HideInInspector] public Vector3 dir; // ✅ Bullet direction

    Rigidbody rb;

    [Header("Popup Settings")]
    public GameObject damagePopupPrefab; // ✅ Popup prefab

    void Start()
    {
        // ✅ Destroy bullet after timeout
        Destroy(gameObject, timeToDestroy);

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false; // ✅ Fly straight
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.constraints = RigidbodyConstraints.FreezeRotation; // ✅ Prevent tilting
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ✅ Check for EnemyHealth
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemyHealth)
        {
            float damageDealt = weapon.damage;

            // ✅ Check if hit collider is head
            if (collision.collider.name.ToLower().Contains("head"))
            {
                damageDealt *= 2f; // ✅ Double damage for headshot
            }

            enemyHealth.TakeDamage(damageDealt);

            // ✅ Apply knockback if enemy died
            if (enemyHealth.health <= 0 && !enemyHealth.isDead)
            {
                Rigidbody enemyRb = enemyHealth.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
                }
                enemyHealth.isDead = true;
            }

            // ✅ Spawn damage popup
            SpawnPopup(collision.contacts[0].point, damageDealt, collision.collider);
        }

        Destroy(gameObject); // ✅ Destroy bullet
    }

    void SpawnPopup(Vector3 position, float damage, Collider hitCollider)
    {
        if (damagePopupPrefab == null) return; // ✅ No popup prefab assigned

        GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);

        // ✅ Face camera
        popup.transform.LookAt(Camera.main.transform);

        // ✅ Try DamagePopup component
    }
}
