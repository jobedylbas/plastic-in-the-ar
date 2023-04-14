using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    
        void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Collision with particle");
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);
        Debug.Log(other);
        if (other.tag == "SortingBall") {
            Debug.Log("Collided with cheese");
        }
        // var i = 0
        // while (i < numCollisionEvents);
        // // {
        //     if (rb)
        //     {
        //         Vector3 pos = collisionEvents[i].intersection;
        //         Vector3 force = collisionEvents[i].velocity * 10;
        //         rb.AddForce(force);
        //     }
        //     i++;
        // }
    }
}
