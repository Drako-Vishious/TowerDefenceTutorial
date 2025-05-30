using UnityEngine;

public class SeekingProjectile : Projectile
{
    [Header("References")]
    public Transform trans;

    private Vector3 targetPosition;

    protected override void OnSetup(){}

    private void Update()
    {
        if(targetEnemy != null)
        {
            //Mark the enemy's last position:
            targetPosition = targetEnemy.projectileSeekPoint.position;
        }

        //Point towards the target position:
        trans.forward = (targetPosition - trans.position).normalized;

        //Move towards the target position:
        trans.position = Vector3.MoveTowards(trans.position, targetPosition, speed *  Time.deltaTime);

        //If we have reached the target position:
        if(trans.position == targetPosition )
        {
            //Damage the enemy if it's still around:
            if(targetEnemy != null)
            {
                targetEnemy.TakeDamage(damage);
            }
            //Destroy the projectile
            Destroy(gameObject);
        }
    }
}
