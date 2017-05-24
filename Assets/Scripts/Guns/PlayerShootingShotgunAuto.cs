using UnityEngine;
using System.Collections;

public class PlayerShootingShotgunAuto : PlayerShootingShotgun {


	private float timeBetweenShots;

	void Awake ()
	{
		timeBetweenShots = 0f;
		m_IsShotgun = true;
	}

	void Start()
	{
		setUpShotgun ();
	}

	void Update ()
	{
		timeBetweenShots += Time.deltaTime;

		//Find out if theyre walking
		if (m_FirstPersonController.findIfWalking() && m_CanShoot && Input.GetButton ("Fire1") 
			&& timeBetweenShots >= timeBetweenBullets && Time.timeScale != 0) {
			changeAnimation ("Shooting", true);
			
			shootShotgun ();
			timeBetweenShots = 0f;
		}

		if (timeBetweenShots >= timeBetweenBullets * m_EffectsDisplayTime) {
			disableShotgunEffects (m_NumPelletsPerShell);
		}

		if (Input.GetButtonUp ("Fire1")) {
			changeAnimation ("Shooting", false);
		}

	}
}

