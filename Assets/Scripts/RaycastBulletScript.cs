using UnityEngine;

public class RaycastBulletScript : MonoBehaviour
{
    private float damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            other.SendMessage("Hit",damage);
        }
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }
}
