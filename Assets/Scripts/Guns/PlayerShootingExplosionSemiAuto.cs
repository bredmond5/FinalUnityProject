using UnityEngine;
using System.Collections;

public class PlayerShootingExplosionSemiAuto : PlayerShootingExplosion {


	private float timeBetweenShots;

	void Start () {
		timeBetweenShots = 0;
	}

	private void Update ()
	{
		timeBetweenShots += Time.deltaTime;

		if (m_FirstPersonController.findIfWalking() && m_CanShoot && Input.GetButtonDown ("Fire1") 
			&& timeBetweenShots >= timeBetweenBullets && Time.timeScale != 0) {
			// If the Fire1 button is being press and it's time to fire..
				// ... shoot the gun.
			doShootTrigger();
			timeBetweenShots = 0f;
			Fire ();
		}
	}
}
