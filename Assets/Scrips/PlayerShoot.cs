using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public float fireRate = 0.3f; // Cooldown time in seconds
    private float nextFireTime = 0f;

    void Update()
    {
        if (!PauseMenu.isPaused)
        {
            if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime) // Left click with cooldown check
            {
                Shoot();
                nextFireTime = Time.time + fireRate; // Set the next allowed shot time
            }
        }
    }

    void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 shootDirection = (mousePosition - transform.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * bulletSpeed;

        Destroy(bullet, 2f);
    }
}
