using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionBehavior : MonoBehaviour
{
    public ParticleManager particleManager;
    public ObjectData objectData;
    public bool isPlaying = true;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Trigger");
        Debug.Log(collision.gameObject.tag);
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "Plane")
        {
            if(isPlaying) {
                Debug.Log("Object collision");
                ContactPoint contact = collision.contacts[0];
                Vector3 position = contact.point;
                // particleManager
                // CreateRandomParticleSystem(position);
                particleManager.CreateParticleSystemFor(objectData, position);
                Destroy(gameObject);
            } else {
                Debug.Log("Cheese");
            }
            
        }
    }
}
