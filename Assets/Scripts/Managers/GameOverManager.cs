using UnityEngine;

/*
 This class is called whenever the player loses. It is made because it has a seperate update function that 
 waits for the player to press the restart. 
 */

   public class GameOverManager : MonoBehaviour
   {		
	[HideInInspector] public Animator m_Anim;
	[HideInInspector] public bool m_Driving;
	[HideInInspector] public Vector3 m_PlayerPosition;
	[HideInInspector] public Quaternion m_PlayerRotation;
	[HideInInspector] public PlayerGunsManager playerGuns;

	private GameManager m_GameManager;

	void Awake ()
	{
		m_GameManager = GetComponent<GameManager> ();
	}
		
	//Calls this function whenever the script is enabled.
	public void setUpReferences()
	{
		m_Driving = m_GameManager.m_InCar;
		m_Anim = m_GameManager.m_HudAnimator;
		playerGuns = m_GameManager.m_PlayerGuns;
	}

	public void lost()
	{
		m_Anim.SetTrigger ("GameOver");

		if (!m_Driving) {
			playerGuns.turnOffShootingAbility (); //Makes it so the player can't shoot while dead.
		}
	}

	//The update function. To change what your restart button is, just change the keycode. 
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.B)) {
			tryAgain ();
		}
	}

	//Tries again.
	private void tryAgain()
	{
		if (m_Driving) {
			StartCoroutine(m_GameManager.tryAgainRace ());
		} else {
			m_GameManager.tryAgainFight (m_PlayerPosition, m_PlayerRotation);
			playerGuns.turnOnShootingAbility ();
		}
		enabled = false; //Turns itself off. 
	}
}