using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Rotation : MonoBehaviour
{
    public string trigger;
    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(other.CompareTag(trigger))
        {
            rb.freezeRotation = false;
        }
    }
}
