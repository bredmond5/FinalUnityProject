using UnityEngine;
using UnityEngine.UI;

/*
 Similar to how playerShooting is the parent of all the playerShooting classes, playerShootingExplosion is 
 the parent to all the playerShootingExplosion classes. This class handles firing a shell and changing the ammo.
 */ 
public class PlayerShootingExplosion : PlayerShooting
{
	public Rigidbody m_Shell;                   // Prefab of the shell.
	public float m_LaunchForce = 15f;        // The force given to the shell if the fire button is not held.

	private string m_FireButton;                // The input axis that is used for launching shells.
		
	public void Fire ()
	{
		// Create an instance of the shell and store a reference to it's rigidbody.
		Rigidbody shellInstance =
			Instantiate (m_Shell, transform.position, transform.rotation) as Rigidbody;

		// Set the shell's velocity to the launch force in the fire position's forward direction.
		shellInstance.velocity = m_LaunchForce * transform.forward;

//		gunAudioSource.Play ();

		changeAmmo ();
	}
}