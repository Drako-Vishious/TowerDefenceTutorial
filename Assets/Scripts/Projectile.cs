using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public Enemy targetEnemy;

    public void Setup(float damage, float speed, Enemy targetEnemy)
    {
        this.damage = damage;
        this.speed = speed;
        this.targetEnemy = targetEnemy;

        OnSetup();
    }

    protected abstract void OnSetup();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
