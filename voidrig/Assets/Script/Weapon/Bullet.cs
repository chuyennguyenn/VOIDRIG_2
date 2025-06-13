using UnityEngine;

public class Bullet: MonoBehaviour
{
    private void OnCollisionEnter(Collision objectHit)
    {
        if (objectHit.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Hit " + objectHit.gameObject.name + " !");
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }

        if (objectHit.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Hit " + objectHit.gameObject.name + " !");
            CreateBulletImpactEffect(objectHit);
            Destroy(gameObject);
        }
    }
    void CreateBulletImpactEffect(Collision objectHit)
    {
        ContactPoint contact = objectHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalRefrences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
            );

        hole.transform.SetParent(objectHit.gameObject.transform);
    }
}
