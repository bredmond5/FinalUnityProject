using UnityEngine;
using System.Collections;

public class Reload : MonoBehaviour {

	[HideInInspector] public bool noBullets;
	public AudioClip reloadSound;
//	public float m_TimeWantedForReload;
	public bool m_InfiniteAmmo;

	private bool reloading;
	private PlayerShooting playerShooting;
	private float lengthReloadSound;
	private float timer = 0;

	void Start()
	{
		playerShooting = GetComponent<PlayerShooting> ();

		lengthReloadSound = reloadSound.length;
	}

	void Update () {
		if(reloading)
			timer += Time.deltaTime;

		if ((Input.GetKeyDown (KeyCode.R) || noBullets) && !reloading) {
			startReload ();

		} else if (Input.GetKey (KeyCode.LeftShift) && reloading) {
			
			stopReloading ();

		} else if (timer >= lengthReloadSound) {
			finishReload ();
		}
	}

	private void startReload()
	{
		if (AmmoManager.m_AmmoLeft != 0 && AmmoManager.m_AmmoInClip != playerShooting.m_AmmoPerClip) {
			reloading = true;
			playerShooting.m_CanShoot = false;
			playerShooting.changeToReloadClipAndPlay (reloadSound);
			playerShooting.doReloadingAnimation ();
		} else {
			//Play a cant reload sound. 
			noBullets = false;
			Debug.Log("you cant reload!");
		}
	}

	private void finishReload()
	{
		if (!m_InfiniteAmmo) {
			reloadNotInfinite ();
		} else {
			reloadInfinite ();
		}

		stopReloading ();
	}

	private void reloadNotInfinite()
	{
		int numToReload = playerShooting.m_AmmoPerClip - AmmoManager.m_AmmoInClip;
		if (AmmoManager.m_AmmoLeft >= numToReload) {
			AmmoManager.m_AmmoInClip = playerShooting.m_AmmoPerClip;
			AmmoManager.m_AmmoLeft -= numToReload;

		} else {
			AmmoManager.m_AmmoInClip += AmmoManager.m_AmmoLeft;
			AmmoManager.m_AmmoLeft = 0;
		}
	}

	private void reloadInfinite()
	{
		AmmoManager.m_AmmoInClip = playerShooting.m_AmmoPerClip;
		AmmoManager.m_AmmoLeft = playerShooting.m_AmmoPerClip;
	}

	private void stopReloading()
	{
		timer = 0;
		reloading = false;

		playerShooting.changeToShootingClip();
		playerShooting.changeAnimation("Reloading", false);

		bool noAmmoInClip = AmmoManager.m_AmmoInClip == 0; 

		if (!(noAmmoInClip)) {
			playerShooting.m_CanShoot = true;		
			noBullets = false;

		} else if (noAmmoInClip && AmmoManager.m_AmmoLeft != 0) {
			noBullets = true;
		
		} else {
			noBullets = false;
			playerShooting.m_CanShoot = false;
		}
	}
}
