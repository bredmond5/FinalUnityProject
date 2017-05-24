﻿using UnityEngine;

public class ShellExplosion : MonoBehaviour
{
    public LayerMask m_EnemyMask;
    public ParticleSystem m_ExplosionParticles;       
    public AudioSource m_ExplosionAudio;              
    public float m_MaxDamage = 100f;                  
    public float m_ExplosionForce = 1000f;            
    public float m_MaxLifeTime = 2f;                  
    public float m_ExplosionRadius = 5f;              


    private void Start()
    {
        Destroy(gameObject, m_MaxLifeTime);
    }


    private void OnTriggerEnter(Collider other)
    {
        // Find all the tanks in an area around the shell and damage them.

		Collider[] colliders = Physics.OverlapSphere (transform.position, m_ExplosionRadius, m_EnemyMask);

		for (int i = 0; i < colliders.Length; i++) 
		{
			Rigidbody targetRigidbody = colliders [i].GetComponent<Rigidbody> ();
			if (!targetRigidbody)
				continue;

			targetRigidbody.AddExplosionForce (m_ExplosionForce, transform.position, m_ExplosionRadius);

			EnemyHealth enemyHealth = targetRigidbody.GetComponent<EnemyHealth> ();

			if (!enemyHealth)
				continue;

			float damage = CalculateDamage (targetRigidbody.position);

			int damageInt = (int)damage;

			//fix this, find where to actually put the particle system.
			Vector3 placementArea = new Vector3 (0f, 0f, 0f);

			enemyHealth.TakeDamage (damageInt, placementArea);
		}

		m_ExplosionParticles.transform.parent = null;

		m_ExplosionParticles.Play();
		m_ExplosionAudio.Play ();

		Destroy (m_ExplosionParticles.gameObject, m_ExplosionParticles.duration);
		Destroy (gameObject);
    }


    private float CalculateDamage(Vector3 targetPosition)
    {
        // Calculate the amount of damage a target should take based on it's position.

		Vector3 explosionToTarget = targetPosition - transform.position;

		float explosionDistance = explosionToTarget.magnitude;

		float relativeDistance = (m_ExplosionRadius - explosionDistance) / m_ExplosionRadius;

		float damage = relativeDistance * m_MaxDamage;
		damage = Mathf.Max (0f, damage);

		return damage;
    }
}