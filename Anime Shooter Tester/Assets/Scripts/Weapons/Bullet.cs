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
        Destroy(gameObject, timeToDestroy);

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.drag = 0f;
            rb.angularDrag = 0f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ✅ Check for EnemyHealth on hit object or parent
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null && !enemyHealth.isDead)
        {
            float damageDealt = weapon.damage;

            // ✅ Headshot detection
            if (collision.collider.name.ToLower().Contains("head"))
            {
                damageDealt *= 2f;
            }

            // ✅ Deal damage
            enemyHealth.TakeDamage(damageDealt);

            // ✅ Knockback if dead
            if (enemyHealth.health <= 0 && !enemyHealth.isDead)
            {
                Rigidbody enemyRb = enemyHealth.GetComponent<Rigidbody>();
                if (enemyRb != null)
                {
                    enemyRb.AddForce(dir * weapon.enemyKickbackForce, ForceMode.Impulse);
                }
                enemyHealth.isDead = true;
            }

            // ✅ Popup
            SpawnPopup(collision.contacts[0].point, damageDealt, collision.collider);
        }

        Destroy(gameObject); // ✅ Bullet disappears on impact
    }

    void SpawnPopup(Vector3 position, float damage, Collider hitCollider)
    {
        if (damagePopupPrefab == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);

        // ✅ Face the popup toward camera
        popup.transform.LookAt(Camera.main.transform);
        popup.transform.Rotate(0, 180, 0); // ✅ Correct direction if needed

        // ✅ If it has a DamagePopup component, send damage info
        DamagePopup dp = popup.GetComponent<DamagePopup>();
        if (dp != null)
        {
            dp.Setup((int)damage, hitCollider.name.ToLower().Contains("head"));
        }
    }
}
