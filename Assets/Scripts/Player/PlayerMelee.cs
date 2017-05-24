using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMelee : MonoBehaviour {

	[HideInInspector] public bool m_CanAttack = true;
	[HideInInspector] public UnityStandardAssets.Characters.FirstPerson.FirstPersonController m_FirstPersonController;
    [HideInInspector] public Animator m_MeleeAnim;

    public int m_DamagePerMelee;
	public int m_MeleeDistance;
	public float m_TimePerMelee;
	public string m_AnimatorNameWanted;

	private GameObject m_Sphere;
	private PlayerMeleeOnTriggerEnter m_PlayerMeleeOnTriggerEnter;
	private AudioSource m_MeleeAudio;
	private float m_TimeBetweenAttacks = 0;

	// Use this for initialization
	public void setUpReferences () {
		m_FirstPersonController = GetComponentInParent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();

		constructSphere();

		m_MeleeAudio = GetComponent<AudioSource> ();
		m_PlayerMeleeOnTriggerEnter = m_Sphere.GetComponentInChildren<PlayerMeleeOnTriggerEnter> ();
	}

	private void constructSphere()
	{
		m_Sphere = GameObject.Find ("HitSphere");
		
		m_Sphere.GetComponentInChildren<SphereCollider> ().radius = m_MeleeDistance;
		m_Sphere.GetComponentInChildren<PlayerMeleeOnTriggerEnter> ().enabled = true;
	}

	// Update is called once per frame
	void Update () {
		m_TimeBetweenAttacks += Time.deltaTime;

		if (Input.GetButtonDown ("Fire1") && m_TimeBetweenAttacks > m_TimePerMelee && m_CanAttack) {
			attack ();
			Debug.Log ("attacking!");
		}
	}

	private void attack()
	{
		m_MeleeAnim.SetTrigger ("Attack");

		List <EnemyHealth> enemiesInSphereList = m_PlayerMeleeOnTriggerEnter.returnEnemiesInSphereList();

		if (enemiesInSphereList != null) { //prevent from making the hit sound if there are no enemies to hit.
			m_MeleeAudio.Play ();
		}
			
		int i = 0;
		foreach (EnemyHealth enemyHealth in enemiesInSphereList) {
			enemyHealth.TakeDamage (m_DamagePerMelee, Vector3.zero);
			i++;
			Debug.Log (i);
		}

		m_TimeBetweenAttacks = 0;
	}

	private void setFiringLength()
	{
		changeMeleeAnimatorAttackLength ();
	}

	private void changeMeleeAnimatorAttackLength()
	{
		RuntimeAnimatorController ac = m_MeleeAnim.runtimeAnimatorController;   
		AnimationClip animationClip = findClip (ac, "Attack");

		float time = animationClip.length;
		m_MeleeAnim.SetFloat ("Multiplier", (time / m_TimeBetweenAttacks));
	}

	private AnimationClip findClip(RuntimeAnimatorController ac, string clipName)
	{
		for (int i = 0; i < ac.animationClips.Length; i++) {
			if (getLastLetters (ac.animationClips [i].name, clipName.Length) == clipName) {
				return ac.animationClips [i];
			}
		}
		Debug.Log ("Couldnt find the animation clip for attack!");
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

	public string returnAnimatorName()
	{
		return m_AnimatorNameWanted;
	}
}
