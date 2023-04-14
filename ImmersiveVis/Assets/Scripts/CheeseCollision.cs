using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheeseCollision : MonoBehaviour
{
    void Start()
    {
    
    }
    
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trigger Cheese collision");
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.tag != "Plane")
        {
            Debug.Log(collision.gameObject.tag);
        }
    }
}
