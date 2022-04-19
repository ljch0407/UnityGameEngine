using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrap : MonoBehaviour
{
    private ParticleSystem particles;
    private List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnParticleCollision(GameObject other)
    {
        Debug.Log("Ãæµ¹");
        int numCollisionEvents = particles.GetCollisionEvents(other, collisionEvents);

        Rigidbody rb = other.GetComponent<Rigidbody>();
        int i = 0;
        while (i < numCollisionEvents)
        {
            if (rb)
            {
                Vector3 force = collisionEvents[i].normal * 100 * Time.deltaTime;
                rb.AddForce(force, ForceMode.Acceleration);
                break;
            }
            i++;
        }
    }
}
