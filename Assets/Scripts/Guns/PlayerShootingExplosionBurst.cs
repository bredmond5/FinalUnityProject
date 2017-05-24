using UnityEngine;
using System.Collections;

public class PlayerShootingExplosionBurst : PlayerShootingExplosion {

	public int numShotsPerBurst = 3;
	public float timeBetweenBursts = 0.3f;

	private float timeBetweenShots;                                   
	private float timeSinceEndShot;
	private bool shooting;
	private int numShotsFired;

	// Use this for initialization
	void Start () {
		timeBetweenShots = 0;
		timeSinceEndShot = 0;
	}
	
	// Update is called once per frame
	void Update () {
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
			Fire ();
			numShotsFired++;
			timeBetweenShots = 0;
			shooting = true;

		} else if (shooting && numShotsFired < numShotsPerBurst && timeBetweenShots >= timeBetweenBullets) {
			Fire ();
			numShotsFired++;
			timeBetweenShots = 0;

		} else if (numShotsFired == numShotsPerBurst && (timeBetweenShots >= timeBetweenBullets || timeBetweenShots >= timeBetweenBursts)) {
			restartBurst ();
			timeBetweenShots = 0;
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
