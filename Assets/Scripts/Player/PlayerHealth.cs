using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour 
{
	[HideInInspector] public Slider m_HealthSlider;                   // Reference to the UI's health bar.
	[HideInInspector] public Image m_DamageImage;                     // Reference to an image to flash on the screen on being hurt.
	[HideInInspector] public GameOverManager m_GameOverManager;
	public int m_StartingHealth = 100;                            // The amount of health the player starts the game with.
    public int m_CurrentHealth;                                   // The current health the player has.

    public AudioClip m_DeathClip;                                 // The audio clip to play when the player dies.
    public float m_FlashSpeed = 5f;                               // The speed the m_DamageImage will fade at.
    public Color m_FlashColor = new Color(1f, 0f, 0f, 0.1f);     // The colour the m_DamageImage is set to, to flash.
	
//  Animator anim;                                              // Reference to the Animator component.
    private AudioSource m_PlayerAudio;                                    // Reference to the AudioSource component.
	private UnityStandardAssets.Characters.FirstPerson.FirstPersonController m_PlayerMovement;                              // Reference to the player's movement.
	private bool m_IsDead;                                                // Whether the player is dead.
	private bool m_Damaged;                                               // True when the player gets m_Damaged.

    void Awake ()
    {
        // Setting up the references.
//      anim = GetComponent <Animator> ();
        m_PlayerAudio = GetComponent <AudioSource> ();
		m_PlayerMovement = GetComponent <UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ();

        // Set the initial health of the player.
        m_CurrentHealth = m_StartingHealth;
    }


    void Update ()
    {
        // If the player has just been m_Damaged...
        if(m_Damaged)
        {
            // ... set the colour of the m_DamageImage to the flash colour.
            m_DamageImage.color = m_FlashColor;
        }
        // Otherwise...
        else
        {
            // ... transition the colour back to clear.
            m_DamageImage.color = Color.Lerp (m_DamageImage.color, Color.clear, m_FlashSpeed * Time.deltaTime);
        }

        // Reset the m_Damaged flag.
        m_Damaged = false;
    }


    public void TakeDamage (int amount)
    {
        // Set the m_Damaged flag so the screen will flash.
        m_Damaged = true;

        // Reduce the current health by the damage amount.
        m_CurrentHealth -= amount;

        // Set the health bar's value to the current health.
        m_HealthSlider.value = m_CurrentHealth;

        // Play the hurt sound effect.
        m_PlayerAudio.Play ();

        // If the player has lost all it's health and the death flag hasn't been set yet...
        if(m_CurrentHealth <= 0 && !m_IsDead)
        {
            // ... it should die.
            Death ();
        }
    }


    void Death ()
    {
        // Set the death flag so this function won't be called again.
        m_IsDead = true;

		m_GameOverManager.enabled = true;
		m_GameOverManager.lost ();
		m_GameOverManager.m_Driving = false;

        // Turn off any remaining shooting effects.

        // Tell the animator that the player is dead.
       // anim.SetTrigger ("Die");

        // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
        m_PlayerAudio.clip = m_DeathClip;
        m_PlayerAudio.Play ();


        // Turn off the movement and shooting scripts.
		m_PlayerMovement.enabled = false;

		PlayerShooting playerShooting = GetComponentInChildren <PlayerShooting> ();
        //playerShooting.DisableEffects();

        if (playerShooting != null) {
			playerShooting.turnOffReloadScript ();
			playerShooting.enabled = false;
		} else {
			PlayerMelee playerMelee = GetComponentInChildren<PlayerMelee> ();
			playerMelee.enabled = false;
		}


    }

    public void addHealth(int amount)
    {
        m_CurrentHealth += amount;
        if (m_CurrentHealth > m_StartingHealth)
        {
            m_CurrentHealth = m_StartingHealth;
        }

        m_HealthSlider.value = m_CurrentHealth;

    }
}
