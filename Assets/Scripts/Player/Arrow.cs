using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speed = 15f; // 增加默认飞行速度
    [SerializeField] private float lifeSpawn = 2f;
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Sprite buriedSprite;
    [SerializeField] private int damage;
    [SerializeField] private float knockbackForce;
    [SerializeField] private float knockbackTime;
    [SerializeField] private float stunTime;
    
    public Vector2 Direction
    {
        get{return direction;}
        set{direction = value;}
    }

    private void Start()
    {
        rb.velocity = direction * speed;
        RotateArrow();
        Destroy(gameObject, lifeSpawn);
        
    }
    private void RotateArrow()
    {
        float angle = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0,0,angle));
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if((enemyLayer & (1 << collision.gameObject.layer)) > 0)
        {
            collision.gameObject.GetComponent<Enemy_Health>().ChangeHealth(-damage);
            collision.gameObject.GetComponent<Enemy_Knockback>().KnockBack(transform,knockbackForce,knockbackTime,stunTime);
            AttachToTarget(collision.gameObject.transform);
        }
        else if((obstacleLayer & (1 << collision.gameObject.layer)) > 0)
        {
            AttachToTarget(collision.gameObject.transform);
        }
    }
    private void AttachToTarget(Transform target)
    {
        sr.sprite = buriedSprite;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        transform.SetParent(target); 
    }
}
