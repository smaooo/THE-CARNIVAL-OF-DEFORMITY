using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MG4Character : MonoBehaviour
{
    public GameObject coliderObj = null;
    public bool collided = false;
    private void Update()
    {
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Shape"))
        {
            coliderObj = collision.gameObject;

        }

    }

    
    
    private void OnTriggerExit(Collider collision)
    {
        coliderObj = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shape"))
        {
            collided = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        collided = false;
    }

    
}
