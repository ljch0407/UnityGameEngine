using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTrap : MonoBehaviour
{
    public int pushingForce;
    public int floatingForce;

    private ParticleSystem particles;
    private List<ParticleCollisionEvent> collisionEvents;

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponent<ParticleSystem>();

        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log(name);
        int numCollisionEvents = particles.GetCollisionEvents(other, collisionEvents);

        Rigidbody rb = other.GetComponent<Rigidbody>();
        int i = 0;
        while (i < numCollisionEvents)
        {
            if (rb)
            {
                Vector3 force = -collisionEvents[i].normal * pushingForce;
                force.y *= floatingForce;
                rb.AddForce(force);
            }
            i++;
        }
    }
}
