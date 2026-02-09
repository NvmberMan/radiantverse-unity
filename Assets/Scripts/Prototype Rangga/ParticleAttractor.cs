using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleAttractor : MonoBehaviour
{
    public Transform target;

    [Header("Delay")]
    public float freeMoveTime = 0.4f;

    [Header("Move")]
    public float moveSpeed = 25f;
    public float stopDistance = 0.25f;

    ParticleSystem ps;
    ParticleSystem.Particle[] particles;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        if (particles == null || particles.Length < ps.main.maxParticles)
            particles = new ParticleSystem.Particle[ps.main.maxParticles];

        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            float lifeTime = particles[i].startLifetime - particles[i].remainingLifetime;

            // Masih fase bebas (delay)
            if (lifeTime < freeMoveTime)
                continue;

            Vector3 pos = particles[i].position;
            float dist = Vector3.Distance(pos, target.position);

            if (dist <= stopDistance)
            {
                particles[i].remainingLifetime = 0f;
                continue;
            }

            particles[i].position = Vector3.MoveTowards(
                pos,
                target.position,
                moveSpeed * Time.deltaTime
            );
        }

        ps.SetParticles(particles, count);
    }
}
