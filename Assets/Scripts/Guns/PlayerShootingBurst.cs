using UnityEngine;
using System.Collections;

public class PlayerShootingBurst : PlayerShooting {

	public int numShotsPerBurst = 3;
	public float timeBetweenBursts = 0.3f;

	private float timeBetweenShots;                                    // A timer to determine when to fire.
	private float timeSinceEndShot;
	private bool shooting;
	private int numShotsFired;

	void Awake ()
	{
		timeBetweenShots = 0;
		timeSinceEndShot = 0;
	}

	void Update ()
	{
		// Add the time since Update was last called to the timer.
		if (shooting)
			timeBetweenShots += Time.deltaTime;
		else
			timeSinceEndShot += Time.deltaTime;

			//Find out if theyre walking
			// If the Fire1 button is being press and it's time to fire...
		if (m_FirstPersonController.findIfWalking() && m_CanShoot && Input.GetButtonDown ("Fire1")
			&& timeSinceEndShot >= (timeBetweenBursts - timeBetweenBullets) && Time.timeScale != 0 && !shooting) {
				// ... shoot the gun.
			changeAnimation ("Shooting", true);
			Shoot ();
			numShotsFired++;
			timeBetweenShots = 0;
			shooting = true;
				

		} else if (shooting && numShotsFired < numShotsPerBurst && timeBetweenShots >= timeBetweenBullets && m_CanShoot) {
			Shoot ();
			numShotsFired++;
			timeBetweenShots = 0;

		} else if (((numShotsFired == numShotsPerBurst) || !m_CanShoot) && (timeBetweenShots >= timeBetweenBullets || timeBetweenShots >= timeBetweenBursts)) {
			restartBurst ();
			timeBetweenShots = 0;
		}

		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
		if (timeBetweenShots >= timeBetweenBullets * m_EffectsDisplayTime) {
			DisableEffects ();
		}
	}

	private void restartBurst()
	{
		changeAnimation ("Shooting", false);
		numShotsFired = 0;
		timeSinceEndShot = 0;
		shooting = false;
	}
}

