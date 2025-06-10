using UnityEngine;

public class Health: MonoBehaviour
{
    public float health = 3;

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("Hit!");
            health -= 1;
        }
    }
}
