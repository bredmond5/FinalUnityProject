using UnityEngine;
using System.Collections;

/*
 This is the parent class of the playerShooting. It handles the actual shot, changing the audio clips, 
 and reloading when the player has run out of bullets.
 */ 

public class PlayerShooting : MonoBehaviour {

	[HideInInspector] public UnityStandardAssets.Characters.FirstPerson.FirstPersonController m_FirstPersonController;
	[HideInInspector] public bool m_CanShoot;
	[HideInInspector] public Reload m_Reload;
	[HideInInspector] public bool m_IsShotgun = false;
	[HideInInspector] public LineRenderer[] m_LineRenderers;
	[HideInInspector] public Animator m_ArmAnimator;
	public float timeBetweenBullets;
	public int m_DamagePerBullet;
	public int m_AmountGivenAmmoCrate;
	public int m_AmmoPerClip;
	public int m_Ammo;
	public AudioClip m_ShootSound;
	public float m_Range = 100f;                      // The distance the gun can fire.
	public float m_EffectsDisplayTime = 0.2f;
    public float innaccuracy = 0.02f;

	private int m_ShootableMask;
	private Ray m_ShootRay;                                   // A ray from the gun end forwards.
	private RaycastHit m_ShootHit;                            // A raycast hit to get information about what was hit.                           // A layer mask so the raycast only hits things on the shootable layer.
	private ParticleSystem m_GunParticles;                    // Reference to the particle system.
	private LineRenderer m_GunLine;                           // Reference to the line renderer.
	private AudioSource m_GunAudio;                           // Reference to the audio source.
	private Light m_GunLight; 
	private AudioSource m_GunAudioSource;
	private Animator m_GunAnimator;
	private bool m_AnimatorsPresent;

	public void setUpReferences()
	{
		setUpGun ();
		if (!m_IsShotgun) {
			m_GunLine = GetComponent <LineRenderer> ();
		}
	}

	private void setUpGun()
	{
		m_ShootableMask = LayerMask.GetMask ("Shootable");
		m_GunParticles = GetComponent<ParticleSystem> ();

		m_GunAudio = GetComponent<AudioSource> ();
		m_GunLight = GetComponent<Light> ();

		m_GunAnimator = GetComponentInParent<Animator> ();

		m_Reload = GetComponent<Reload> ();
		m_GunAudioSource = GetComponent<AudioSource> ();

 //       m_FirstPersonController = transform.parent.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
	}

	public void checkAnimators()
	{
		if (m_GunAnimator != null && m_ArmAnimator != null) {
			m_AnimatorsPresent = true;

			setFiringLength ();

		} else {
			m_AnimatorsPresent = false;
		}
	}

	private void setFiringLength()
	{
		changeArmAnimatorFiringLength ();
		changeGunAnimatorFiringLength ();
	}

	private void changeArmAnimatorFiringLength()
	{
		RuntimeAnimatorController ac = m_ArmAnimator.runtimeAnimatorController;   
		AnimationClip animationClip = findClip (ac, "Fire");

		float time = animationClip.length;
		m_ArmAnimator.SetFloat ("Multiplier", (time / timeBetweenBullets));
	}

	private void changeGunAnimatorFiringLength()
	{
		RuntimeAnimatorController ac = m_GunAnimator.runtimeAnimatorController;   
		AnimationClip animationClip = findClip (ac, "Fire");

		float time = animationClip.length;
		m_GunAnimator.SetFloat ("Multiplier", (time / timeBetweenBullets));
	}

	private AnimationClip findClip(RuntimeAnimatorController ac, string clipName)
	{
		for (int i = 0; i < ac.animationClips.Length; i++) {
			if (getLastLetters (ac.animationClips [i].name, clipName.Length) == clipName) {
				return ac.animationClips [i];
			}
		}
		Debug.Log ("Couldnt find the animation clip for fire!");
		return null;
	}

	private string getLastLetters(string clipName, int numLetters)
	{
		string stringMade = "";
		int endInt = clipName.Length;
		if (clipName [clipName.Length - 1] == 'w') {
			numLetters += 2; 
			endInt -= 2;
		}

		for (int i = clipName.Length - numLetters; i < endInt; i++) {
			stringMade += clipName [i];
		}

		return stringMade;
	}


	public void Shoot ()
	{
		playEffects ();

		m_GunLine.enabled = true;
		m_GunLine.SetPosition (0, transform.position);

		m_ShootRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        Vector3 rand = new Vector3(Random.Range(-innaccuracy, innaccuracy), 0, Random.Range(-innaccuracy, innaccuracy));
        m_ShootRay.direction += rand;

        if (Physics.Raycast (m_ShootRay, out m_ShootHit, m_Range, m_ShootableMask))
		{
			EnemyHealth enemyHealth = m_ShootHit.collider.GetComponent <EnemyHealth> ();
			if(enemyHealth != null)
			{
				enemyHealth.TakeDamage (m_DamagePerBullet, m_ShootHit.point);
			}
			m_GunLine.SetPosition (1, m_ShootHit.point);
		}
		else
		{
			m_GunLine.SetPosition (1, m_ShootRay.origin + m_ShootRay.direction * m_Range);
		}

		changeAmmo ();

//		if (m_AnimatorPresent) 
//			m_GunAnimator.SetTrigger ("DoneShooting");
	}

	public void shootTheShotgun(float spread, int numPelletsPerShell)
	{
		playEffects ();

		for (int i = 0; i < numPelletsPerShell; i++) {

			Vector3 randomDirection = new Vector3 (Random.Range (-spread, spread), 
				Random.Range (-spread, spread), Random.Range (-spread, spread));

			m_ShootRay.origin = transform.position;
			m_ShootRay.direction = transform.forward + randomDirection;

			m_LineRenderers [i].enabled = true;
			m_LineRenderers [i].SetPosition (0, transform.position);

			if(Physics.Raycast (m_ShootRay, out m_ShootHit, m_Range, m_ShootableMask))
			{
				EnemyHealth enemyHealth = m_ShootHit.collider.GetComponent <EnemyHealth> ();
				if(enemyHealth != null)
				{
					enemyHealth.TakeDamage (m_DamagePerBullet, m_ShootHit.point);
				}
				m_LineRenderers[i].SetPosition (1, m_ShootHit.point);
			}
			else
			{
				m_LineRenderers[i].SetPosition (1, m_ShootRay.origin + m_ShootRay.direction * m_Range);
			}
		}

		changeAmmo ();

//		if (m_AnimatorPresent) 
//			m_GunAnimator.SetTrigger ("DoneShooting"); 
	}

	public void doShootTrigger()
	{
		m_ArmAnimator.SetTrigger ("Shoot");
		m_GunAnimator.SetTrigger ("Shoot");
	}
		
	private void playEffects()
	{

		m_GunAudio.Play ();

		m_GunLight.enabled = true;

		m_GunParticles.Stop ();
		m_GunParticles.Play ();

	}

	public void changeAmmo()
	{
		AmmoManager.m_AmmoInClip--;
		if (AmmoManager.m_AmmoInClip == 0) {
            m_CanShoot = false;
            StartCoroutine(noBullets());
		}
	}

    private IEnumerator noBullets()
    {
        yield return new WaitForSeconds(m_ShootSound.length);
        m_Reload.noBullets = true;
    }

	public void disableShotgunEffects(int numPelletsPerShell)
	{
		m_GunLight.enabled = false;
		for (int i = 0; i < numPelletsPerShell; i++) {
			m_LineRenderers [i].enabled = false;
		}
	}

	public void DisableEffects ()
	{
		// Disable the line renderer and the light.
		m_GunLine.enabled = false;
		m_GunLight.enabled = false;
	}

	public void changeToReloadClipAndPlay(AudioClip m_ReloadSound)
	{
		m_GunAudioSource.clip = m_ReloadSound;
		m_GunAudioSource.Play();
	}

	public void changeToShootingClip()
	{
		m_GunAudioSource.clip = m_ShootSound;
	}

	public void doReloadingAnimation()
	{
		if (m_AnimatorsPresent) {
			m_ArmAnimator.SetTrigger ("DoReload");
			m_GunAnimator.SetTrigger ("DoReload");
			m_ArmAnimator.SetBool ("Reloading", true);
			m_GunAnimator.SetBool ("Reloading", true);
		}
	}

	public void turnOffReloadScript()
	{
		m_Reload.enabled = false;
	}

	public void turnOnReloadScript()
	{
		m_Reload.enabled = true;
	}

	public void changeAnimation(string animation, bool on)
	{
		if (m_AnimatorsPresent) {
			m_ArmAnimator.SetBool (animation, on);
			m_GunAnimator.SetBool (animation, on);
		}
	}

	public string returnAnimatorName()
	{
		return m_GunAnimator.runtimeAnimatorController.name;
	}
}
