using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHealth : MonoBehaviour
{
    private Targets targetManager;

    public int maxHealth;
    public int curHealth;

    public void Awake()
    {
        curHealth = maxHealth;
        targetManager = FindObjectOfType<Targets>();
    }

    public void TakeDamage(int playerDamage)
    {
        curHealth -= playerDamage;

        if (curHealth <= 0)
        {
            Targets.instance.targetCount++;
            Debug.Log(Targets.instance.targetCount);            
            Destroy(gameObject);
           
        }
    }
    
}
