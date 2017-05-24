using UnityEngine;
using System.Collections;

public class PlayerShootingExplosionAuto : PlayerShootingExplosion {


	private float timeBetweenShots;

	void Start (){
		timeBetweenShots = 0;
	}

	private void Update ()
	{
		timeBetweenShots += Time.deltaTime;

		if (m_FirstPersonController.findIfWalking() && m_CanShoot && Input.GetButton ("Fire1") 
			&& timeBetweenShots >= timeBetweenBullets && Time.timeScale != 0) {
			// ... shoot the gun.
			changeAnimation ("Shooting", true);
			timeBetweenShots = 0f;
			Fire ();
		}

		if (Input.GetButtonUp ("Fire1")) {
			changeAnimation ("Shooting", false);
		}

	}
}
