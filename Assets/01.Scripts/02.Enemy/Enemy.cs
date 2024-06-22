using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
    public int curHealth;

    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    private Material material;
    public Transform target;
    private NavMeshAgent nav;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();

        curHealth = maxHealth;
    }

    private void Update()
    {
        nav.SetDestination(target.position);
    }

    public IEnumerator TakeDamage(int playerDamage)
    {

        if (curHealth > 0)
        {
            curHealth -= playerDamage;
            material.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            material.color = Color.white;
        }
        if (curHealth <= 0 && this != null)
        {
            gameObject.layer = 19;
            material.color = Color.gray;
            Destroy(gameObject, 1);
        }
    }
}
