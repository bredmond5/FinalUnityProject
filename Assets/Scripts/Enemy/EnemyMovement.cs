using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    Transform m_Player;               // Reference to the player's position.
    PlayerHealth m_PlayerHealth;      // Reference to the player's health.
    EnemyHealth m_EnemyHealth;        // Reference to this enemy's health.
    NavMeshAgent m_Nav;               // Reference to the m_Nav mesh agent.


    void Awake ()
    {
        // Set up the references.
        m_Player = GameObject.FindGameObjectWithTag ("Player").transform;
        m_PlayerHealth = m_Player.GetComponent <PlayerHealth> ();
        m_EnemyHealth = GetComponent <EnemyHealth> ();
        m_Nav = GetComponent <NavMeshAgent> ();
    }
		
    void Update ()
    {

        // If the enemy and the player have health left...
		if(m_EnemyHealth.m_CurrentHealth > 0 && m_PlayerHealth.m_CurrentHealth > 0)
        {
            // ... set the destination of the m_Nav mesh agent to the player.
            m_Nav.SetDestination (m_Player.position);
        }
        // Otherwise...
        else
        {
            // ... disable the m_Nav mesh agent.
            m_Nav.enabled = false;
        }
    }
}