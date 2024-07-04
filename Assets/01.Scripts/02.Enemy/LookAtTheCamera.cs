using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTheCamera : MonoBehaviour
{
    
    void Update()
    {
        Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
        transform.LookAt(transform.position + dirFromCamera);
    }
}
