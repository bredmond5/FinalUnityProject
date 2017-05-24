using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyAttack : MonoBehaviour
    {
        public float m_TimeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int m_AttackDamage = 10;               // The amount of health taken away per attack.


        Animator m_Anim;                              // Reference to the animator component.
        GameObject m_Player;                          // Reference to the player GameObject.
        PlayerHealth m_PlayerHealth;                  // Reference to the player's health.
        EnemyHealth m_m_EnemyHealth;                    // Reference to this enemy's health.
        bool m_PlayerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float m_Timer;                                // m_Timer for counting up to the next attack.


        void Awake ()
        {
            // Setting up the references.
           	m_Player = GameObject.FindGameObjectWithTag ("Player");
            m_PlayerHealth = m_Player.GetComponent <PlayerHealth> ();
            m_m_EnemyHealth = GetComponent<EnemyHealth>();
            m_Anim = GetComponent <Animator> ();
        }


        void OnTriggerEnter (Collider other)
        {
            // If the entering collider is the player...
            if(other.gameObject == m_Player)
            {
                // ... the player is in range.
                m_PlayerInRange = true;
            }
        }


        void OnTriggerExit (Collider other)
        {
            // If the exiting collider is the player...
            if(other.gameObject == m_Player)
            {
                // ... the player is no longer in range.
                m_PlayerInRange = false;
            }
        }


        void Update ()
        {
            // Add the time since Update was last called to the m_Timer.
            m_Timer += Time.deltaTime;

            // If the m_Timer exceeds the time between attacks, the player is in range and this enemy is alive...
            if(m_Timer >= m_TimeBetweenAttacks && m_PlayerInRange && m_m_EnemyHealth.m_CurrentHealth > 0)
            {
                // ... attack.
                Attack ();
            }

            // If the player has zero or less health...
            if(m_PlayerHealth.m_CurrentHealth <= 0)
            {
                // ... tell the animator the player is dead.
                m_Anim.SetTrigger("PlayerDead");
            }
        }


        void Attack ()
        {
            // Reset the m_Timer.
            m_Timer = 0f;

            // If the player has health to lose...
            if(m_PlayerHealth.m_CurrentHealth > 0)
            {
                // ... damage the player.
                m_PlayerHealth.TakeDamage (m_AttackDamage);
            }
        }
    }
}