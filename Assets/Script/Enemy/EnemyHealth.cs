using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{    
    public int maxHealth;
    public int curHealth;

    public void Awake()
    {
        curHealth = maxHealth;
    }
    
    public void TakeDamage(int playerDamage)
    {
        curHealth -= playerDamage;

        if(curHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
