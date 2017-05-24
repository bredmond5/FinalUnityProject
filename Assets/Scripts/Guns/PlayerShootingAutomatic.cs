using UnityEngine;
using System.Collections;

public class PlayerShootingAutomatic : PlayerShooting {

	private float timeBetweenShots;

	void Awake ()
	{
		timeBetweenShots = 0f;
	}

	void Update ()
	{
		timeBetweenShots += Time.deltaTime;

		//Find out if theyre walking
		if (m_FirstPersonController.findIfWalking() && m_CanShoot && Input.GetButton ("Fire1") 
			&& timeBetweenShots >= timeBetweenBullets && Time.timeScale != 0) {
			changeAnimation ("Shooting", true);
			Shoot ();
			timeBetweenShots = 0f;
		}

		if (timeBetweenShots >= timeBetweenBullets * m_EffectsDisplayTime) {
			DisableEffects ();
		}

		if (Input.GetButtonUp ("Fire1")) {
			changeAnimation ("Shooting", false);
		}
	}
}
