using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private float health = 100;
    void Start()
    {
        
    }
    void Update()
    {
        if(health > 100)
        {
            health = 100;
        }
        if(health < 0)
        {
            Destroy(gameObject);
        }
    }
    public void Hurt(float damage)
    {
        health -= damage;
    }
}
