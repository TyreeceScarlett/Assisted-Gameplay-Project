using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float timeToDestroy = 5f;
    [HideInInspector] public WeaponManager weapon;
    [HideInInspector] public Vector3 dir;

    Rigidbody rb;

    public GameObject damagePopupPrefab;

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
        EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null && !enemyHealth.isDead)
        {
            float damageDealt = weapon.damage;
            Transform hitPart = collision.collider.transform;

            // Improved headshot detection
            string boneName = hitPart.name.Replace("Enemy ", "").Trim().ToLower();
            bool isHeadshot = boneName == "j_bip_c_head";

            if (isHeadshot)
                damageDealt *= 2f;

            enemyHealth.TakeDamage(damageDealt, hitPart);

            // Spawn popup (optional, remove if not using)
            SpawnPopup(collision.contacts[0].point, damageDealt, isHeadshot);
        }

        Destroy(gameObject);
    }

    void SpawnPopup(Vector3 position, float damage, bool isHeadshot)
    {
        if (damagePopupPrefab == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, position, Quaternion.identity);
        popup.transform.LookAt(Camera.main.transform);
        popup.transform.Rotate(0, 180, 0);

        DamagePopup dp = popup.GetComponent<DamagePopup>();
        if (dp != null)
        {
            dp.Setup((int)damage, isHeadshot);
        }
    }
}
