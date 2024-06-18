using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEvent : UnityEngine.Events.UnityEvent<int, int> { }

public class TargetHealth : MonoBehaviour
{

    [HideInInspector]
    public ScoreEvent onScoreEvent = new ScoreEvent();

    public int maxHealth;
    public int curHealth;

    public void Awake()
    {
        curHealth = maxHealth;
    }

    public void TakeDamage(int playerDamage)
    {
        curHealth -= playerDamage;
        
        if (curHealth <= 0)
        {
            Targets.instance.targetCount++;
            onScoreEvent.Invoke(Targets.instance.targetCount, Targets.instance.numberOfTargets);
            Targets.instance.isSpawningAllowed = true;
            Destroy(gameObject);
           
        }
    }
    
}
