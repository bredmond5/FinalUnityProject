using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector] public EnemyManager m_parent;

    public int m_StartingHealth = 100;            // The amount of health the enemy starts the game with.
    public int m_CurrentHealth;                   // The current health the enemy has.
    public float m_SinkSpeed = 2.5f;              // The speed at which the enemy sinks through the floor when dead.
//  public int scoreValue = 10;                   // The amount added to the player's score when the enemy dies.
    public AudioClip m_DeathClip;                 // The sound to play when the enemy dies.

    private Animator m_Anim;                              // Reference to the animator.
    private AudioSource m_EnemyAudio;                     // Reference to the audio source.
    private ParticleSystem m_HitParticles;                // Reference to the particle system that plays when the enemy is damaged.
    private CapsuleCollider m_CapsuleCollider;            // Reference to the capsule collider.
    private bool m_IsDead;                                // Whether the enemy is dead.

    void Awake ()
    {
        // Setting up the references.

        m_Anim = GetComponent<Animator>();
        if(!m_Anim)m_Anim = GetComponentInChildren<Animator>();
        m_EnemyAudio = GetComponent <AudioSource> ();
        m_HitParticles = GetComponentInChildren <ParticleSystem> ();
        m_CapsuleCollider = GetComponent <CapsuleCollider> ();

        // Setting the current health when the enemy first spawns.
        m_CurrentHealth = m_StartingHealth;
    }


    public void TakeDamage (int amount, Vector3 hitPoint)
    {
        // If the enemy is dead...
        if(m_IsDead)
            // ... no need to take damage so exit the function.
            return;

        // Play the hurt sound effect.
        m_EnemyAudio.Play ();

        // Reduce the current health by the amount of damage sustained.
        m_CurrentHealth -= amount;
        
        // Set the position of the particle system to where the hit was sustained.
        m_HitParticles.transform.position = hitPoint;

        // And play the particles.
        m_HitParticles.Play();

        // If the current health is less than or equal to zero...
        if(m_CurrentHealth <= 0)
        {
            // ... the enemy is dead.
            Death ();
        }
    }


    void Death ()
    {
		EnemyDropsAmmo enemyDrops = GetComponent<EnemyDropsAmmo> ();
		if (enemyDrops != null)
			enemyDrops.dropAmmo ();

        EnemyDropsHealth enemyDropsHealth = GetComponent<EnemyDropsHealth>();
        if (enemyDropsHealth != null)
            enemyDropsHealth.dropHealth();

        // The enemy is dead.
        m_IsDead = true;

        // Turn the collider into a trigger so shots can pass through it.
        m_CapsuleCollider.isTrigger = true;

        // Tell the animator that the enemy is dead.
        m_Anim.SetTrigger ("Dead");

        // Change the audio clip of the audio source to the death clip and play it (this will stop the hurt clip playing).
        m_EnemyAudio.clip = m_DeathClip;
        m_EnemyAudio.Play ();

		// Find and disable the Nav Mesh Agent.
		GetComponent <NavMeshAgent> ().enabled = false;

		// Find the rigidbody component and make it kinematic (since we use Translate to sink the enemy).
		GetComponent <Rigidbody> ().isKinematic = true;

        //Do the Ragdoll physics.

        m_parent.dudeDied();

		// After 2 seconds destory the enemy.
		Destroy (gameObject, 2f);
    }
}
