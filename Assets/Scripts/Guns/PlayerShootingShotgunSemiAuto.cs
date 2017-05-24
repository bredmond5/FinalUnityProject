using UnityEngine;
using System.Collections;

public class PlayerShootingShotgunSemiAuto : PlayerShootingShotgun {


	private float timeBetweenShots;                 // A timer to determine when to fire.

	void Awake ()
	{
		timeBetweenShots = 0;
		m_IsShotgun = true;
	}

	void Start()
	{
		setUpShotgun ();
	}
		
	void Update ()
	{
		// Add the time since Update was last called to the timer.
		timeBetweenShots += Time.deltaTime;

		//Find out if they are walking
		if (m_FirstPersonController.findIfWalking() && m_CanShoot && Input.GetButtonDown ("Fire1") 
			&& timeBetweenShots >= timeBetweenBullets && Time.timeScale != 0) {
				// ... shoot the gun.
			timeBetweenShots = 0f;
			doShootTrigger();
			shootShotgun ();
		}
		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
		if (timeBetweenBullets != 0) {
			if (timeBetweenShots >= timeBetweenBullets * m_EffectsDisplayTime) {
				// ... disable the effects.
				disableShotgunEffects (m_NumPelletsPerShell);
			}
		} else {
			if (timeBetweenShots >= 0.03f) {
				disableShotgunEffects (m_NumPelletsPerShell);
			}
		}
	}
}
