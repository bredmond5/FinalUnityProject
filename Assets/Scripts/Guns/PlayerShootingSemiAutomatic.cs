using UnityEngine;
using System.Collections;

public class PlayerShootingSemiAutomatic : PlayerShooting {

	private float timeBetweenShots;                 // A timer to determine when to fire.

	void Awake ()
	{
		timeBetweenShots = 0;
	}

	void Update ()
	{
		// Add the time since Update was last called to the timer.
		timeBetweenShots += Time.deltaTime;

		//Find out if they are walking
		if (timeBetweenShots >= timeBetweenBullets - Time.deltaTime) {
			if (m_FirstPersonController.findIfWalking () && m_CanShoot && Input.GetButtonDown ("Fire1")
			   && Time.timeScale != 0) {
				// ... shoot the gun.
//				StartCoroutine(endShootingAnimation());
				doShootTrigger();
				timeBetweenShots = 0f;
				Shoot ();
			}
		}

		if (timeBetweenShots >= m_EffectsDisplayTime) {
			// ... disable the effects.
			DisableEffects ();
		}
	}

	private IEnumerator endShootingAnimation()
	{
		yield return new WaitForSeconds (timeBetweenBullets);
		changeAnimation ("Shooting", false);
	}
}
