using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/*
 This class is the overarching manager of the game. It handles moving the player in and out of the car, 
 what happens when the player loses, addingturning on and off the correct race and fight scripts, adding breaks
 between races/fights, and what happens and when the player wins.
 */

public class GameManager : MonoBehaviour {

	[HideInInspector] public bool m_CanGetInCar;
	[HideInInspector] public bool m_InCar = false;
	[HideInInspector] public Animator m_HudAnimator;
	public GameObject m_Player;
	public GameObject[] m_CarRaceManagers;

    public bool m_UseCustomCarCamera;
    public Camera m_CustomCarCamera;

	public GameObject[] m_RoundManagers;
	public PlayerGunsManager m_PlayerGuns;
	public Canvas m_HudCanvas;
	public bool [] m_BreakWanted;
	public bool [] m_IsRace;

	private GameObject m_ShootDot;
	private GameObject m_EnemiesLeftTimeManager;
	private GameObject m_Car;	
	private GameObject m_RoundTimeTextObject; // A reference to the text that is used for showing the round and the countdown before the race starts.
	private PlayerTriggers m_PlayerTriggers;
	private GameOverManager m_GameOverManager;
	private Camera m_MainCamera;
	private bool m_BreakOver;
	private int m_CurRaceOrFightPhase = 0; //Reference to how many phases have passed
	private int m_CurRacePhase = 0; //Reference to how many races have passed.
	private int m_CurFightPhase = 0; //Reference to how many fights have passed.

	void Awake()
	{
		m_PlayerGuns.m_Player = m_Player;
        m_PlayerGuns.m_AmmoManager = findInChildren(m_HudCanvas.gameObject, "AmmoText");

        m_HudAnimator = m_HudCanvas.GetComponentInChildren<Animator> ();
		//Turn off all of the managers. This is just done in case some managers are left enabled that shouldn't be. 
		turnOffCarRaceManagers ();
		turnOffRoundManagers ();
	}

	private void turnOffCarRaceManagers()
	{
		for (int i = 0; i < m_CarRaceManagers.Length; i++) {
			m_CarRaceManagers [i].SetActive(false);
		}
	}

	private void turnOffRoundManagers()
	{
		for (int i = 0; i < m_RoundManagers.Length; i++) {
			m_RoundManagers [i].SetActive(false);
		}
	}

	//Find out if its a race or fight, then set up the reference to the player triggers, the player camera, and the 
	//car slow down component. Also finds the RoundTimeText gameobject and the hudanimator. 
	private void Start () {
		findHudStuff ();

		m_GameOverManager = GetComponent<GameOverManager> ();

		setPlayerHealthStuff ();

		m_GameOverManager.setUpReferences ();
		m_GameOverManager.enabled = false; // GameOverManager not wanted right now, so turn it off.

//		setUpWhatStarts ();

		m_BreakOver = !m_BreakWanted [0];
		startNextPhase ();

		m_PlayerTriggers = m_Player.GetComponentInChildren<PlayerTriggers> (); //Gets a reference to the player's ontriggerenter script.
		m_PlayerTriggers.m_GameManager = this;

		m_MainCamera = m_Player.GetComponentInChildren<Camera> ();

	}
		
	private void findHudStuff()
	{
		m_RoundTimeTextObject = findInChildren (m_HudCanvas.gameObject, "Round/StartRaceText");
		m_RoundTimeTextObject.SetActive (false); // Round text not wanted right now, so set if off.

		m_ShootDot = findInChildren(m_HudCanvas.gameObject, "ShootDot");
		m_EnemiesLeftTimeManager = findInChildren (m_HudCanvas.gameObject, "EnemiesLeftTimeManager");
	}

	private void setPlayerHealthStuff()
	{
		PlayerHealth playerHealth = m_Player.GetComponentInChildren<PlayerHealth> ();
		playerHealth.m_HealthSlider = m_HudCanvas.GetComponentInChildren<Slider> ();
		playerHealth.m_DamageImage = findInChildren (m_HudCanvas.gameObject, "DamageImage").GetComponentInChildren<Image>();
		playerHealth.m_GameOverManager = m_GameOverManager;
	}

	//This update function is for when the player has just finished a fight and is getting ready to get back in the car. 
	void Update () {
		if (Input.GetKeyDown (KeyCode.Y)){
			if (!m_InCar && m_PlayerTriggers.m_IsInsideCarRange && m_CurRacePhase < m_CarRaceManagers.Length && m_CanGetInCar && m_BreakOver) {
				getInCar ();
			} else {
				Debug.Log (!m_InCar + " " + m_PlayerTriggers.m_IsInsideCarRange + " " + (m_CurRacePhase < m_CarRaceManagers.Length) + " " + m_CanGetInCar);
				if(!m_BreakOver)
					Debug.Log ("You cant get into the car yet!");
			}
		}
	}

	//Overall get in car function.
	private void getInCar()
	{
        m_ShootDot.SetActive(false);

		CarRaceManager curCarRaceManager = m_CarRaceManagers [m_CurRacePhase].GetComponentInChildren<CarRaceManager> ();
		curCarRaceManager.m_EnemiesLeftTimeManager = m_EnemiesLeftTimeManager;
		curCarRaceManager.m_GameManager = this;
		curCarRaceManager.m_TimeToStartRaceText = m_RoundTimeTextObject;

		bool countdownWanted = curCarRaceManager.m_CountDownWanted;

		StartCoroutine(setUpCar(curCarRaceManager, countdownWanted));
		CarOnTriggerEnter carOnTriggerEnter = m_Car.GetComponentInChildren<CarOnTriggerEnter> ();
		carOnTriggerEnter.m_CarRaceManager = curCarRaceManager;
		carOnTriggerEnter.m_ShouldRecognizeTriggers = true;

		turnOffPlayer ();
		setUpCamera ();
	}

	//Slows the car down to a stop and stops the player from being able to move the car anymore. When this finishes
	//get out is called. Get out is called from the m_CarSlowDown script.
	public void slowDown()
	{
		UnityStandardAssets.Vehicles.Car.CarUserControl carUserControl = m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl>();
		carUserControl.carOn = false;

		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarController>().enabled = false; 

		CarSlowDown carSlowDown = m_Car.GetComponentInChildren<CarSlowDown>();

		carSlowDown.enabled = true;
		carSlowDown.m_GameManager = this;
	}

	//Gets the player out of the car, and sets up the player, the camera, and finishes turning the car off.
	public void getOut()
	{
		putPlayerBack ();
		turnCarOff ();
		putCameraBack ();

        m_CurRaceOrFightPhase++;
        m_CurRacePhase++;

		startNextPhase ();

		m_ShootDot.SetActive(true);
	}
		
	//Sets up everything the car needs to drive.
	private IEnumerator setUpCar(CarRaceManager curCarRaceManager, bool countdownWanted)
	{
		m_Car = m_PlayerTriggers.m_Car; //The car is first found from the playerTriggers which encounters the car when it touches the driver side door trigger.
		//find the collider and tuern it off
		m_CarRaceManagers [m_CurRacePhase].SetActive (true);
		curCarRaceManager.m_Car = m_Car;
		curCarRaceManager.setUpReferences ();
		StartCoroutine(curCarRaceManager.startRace ());
		m_CanGetInCar = false;

		if(countdownWanted)
			yield return new WaitForSeconds (3);

		m_InCar = true;
		//		Car.tag = "Player";

		m_Car.GetComponentInChildren<Rigidbody>().constraints= 0;

		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarController> ().enabled = true;
		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl>().enabled = true;
		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl> ().carOn = true;
	}

	//turns off the players gun, makes his parent the car, and turns off the player.
	private void turnOffPlayer()
	{
		m_PlayerGuns.turnOffGunAndArms();
		m_PlayerGuns.enabled = false;
		m_Player.transform.SetParent(m_Car.transform);
		m_Player.SetActive (false);
	}

	//Sets up the camera on the car.
	private void setUpCamera()
	{
        if (m_UseCustomCarCamera)
        {
            m_CustomCarCamera.enabled = true;
            GameObject.Find("FirstPersonCharacter").GetComponent<Camera>().enabled = false;
        }else {
            Vector3 cameraPosition = new Vector3 (0f, 2, -2); //These values work for the car I have, they might have to change.
		    m_MainCamera.transform.parent = m_Car.transform;
		    m_MainCamera.transform.localPosition = cameraPosition;
		    m_MainCamera.transform.rotation = m_Car.transform.rotation;
        }
    }

	private void putPlayerBack()
	{
		//		Quaternion playerRotToY = new Quaternion(0.0f, 0.0f, m_Car.transform.rotation.eulerAngles.y, 0.0f);
		//		m_Player.transform.rotation = playerRotToY;

		Vector3 offset = new Vector3 (-3f, 0f, 0f);

		m_Player.transform.localPosition = offset;

		m_Player.SetActive (true);
		m_Player.transform.parent = null;

		m_PlayerGuns.enabled = true;
		m_PlayerGuns.turnOnGunAndArms();
	}


	//Sends the player references to the game over script so that the player can be put back in the same position and same rotation
	//and same amount of ammo should they die. 
	private void getPlayerReferences()
	{
		//put the player references into the game over manager.
		m_GameOverManager.m_PlayerPosition = m_Player.transform.position;
		m_GameOverManager.m_PlayerRotation = m_Player.transform.rotation;
		m_PlayerGuns.getAmmosAndGunsBeforeFight ();
	}

	//Turns off the player's ability to use the car.
	private void turnCarOff()
	{
		m_CanGetInCar = false;

		m_PlayerTriggers.m_IsInsideCarRange = false;

		m_InCar = false;

		m_Car.GetComponentInChildren<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;

		CarRaceManager curCarRaceManager = m_CarRaceManagers [m_CurRacePhase].GetComponentInChildren<CarRaceManager> ();
		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl> ().enabled = false;
		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarController> ().enabled = false;
		m_Car.GetComponentInChildren<CarOnTriggerEnter> ().m_ShouldRecognizeTriggers = false;
		curCarRaceManager.m_Car = null;
		m_Car = null;

		m_CarRaceManagers[m_CurRacePhase].SetActive(false);
	}

	//Puts the camera back onto the player.
	private void putCameraBack()
	{
        if (m_UseCustomCarCamera)
        {
            m_CustomCarCamera.enabled = false;
            GameObject.Find("FirstPersonCharacter").GetComponent<Camera>().enabled = true;
        }
        Vector3 cameraPosition = new Vector3 (0f, 1f, 0f);
		m_MainCamera.transform.parent = m_Player.transform;
		m_MainCamera.transform.localPosition = cameraPosition;
		m_MainCamera.transform.rotation = m_Player.transform.rotation;
	}

	public void endBreak()
	{
		m_BreakOver = true;
		if (m_IsRace[m_CurRaceOrFightPhase]) {
			Debug.Log ("Get in the car to start the race!");
		} else {
			startNextFight ();
		}
	}

	public void endFightBreak()
	{
		m_RoundManagers[m_CurFightPhase].GetComponentInChildren<RoundManager>().startTheRound();
	}
		
	public void startNextPhase()
	{
		m_EnemiesLeftTimeManager.SetActive (false);
		if (m_CurRaceOrFightPhase < m_IsRace.Length) {
			if (m_IsRace [m_CurRaceOrFightPhase]) {
				nextDrivingPhase ();

			} else {
				//prepare for next fight.
				setUpFight ();
			}
			
		} else {
			Debug.Log ("You finished!");
			SceneManager.LoadScene(2);
		}
	}

	private void setUpFight()
	{
		getPlayerReferences ();

		if (!m_BreakWanted [m_CurRaceOrFightPhase])
			startNextFight ();
		else
			m_EnemiesLeftTimeManager.SetActive (false);
	}

	private void startNextFight()
	{
        if (m_RoundManagers [m_CurFightPhase] != null) { //If there is no more fights, the game is over.
			m_RoundManagers [m_CurFightPhase].SetActive (true);
			RoundManager curRoundManager = m_RoundManagers [m_CurFightPhase].GetComponentInChildren<RoundManager> ();

			curRoundManager.m_RoundTextObject = m_RoundTimeTextObject;
			curRoundManager.m_PlayerHealth = m_Player.GetComponentInChildren<PlayerHealth> ();
			curRoundManager.m_EnemiesLeftTimeManager = m_EnemiesLeftTimeManager;
			curRoundManager.setUpReferences ();
			curRoundManager.m_GameManager = this;

			m_EnemiesLeftTimeManager.GetComponentInChildren<EnemiesLeftTimeManager> ().m_RoundManager = curRoundManager;
			Debug.Log ("There are some enemies for you to fight!");
		}
	}

	//The last fight just finished, makes it so you can get in the car again and start the next race.
	public void nextDrivingPhase()
	{
		m_CanGetInCar = true;

		if (!m_BreakWanted [m_CurRaceOrFightPhase] && m_CarRaceManagers [m_CurRacePhase] != null) {
			Debug.Log ("Get in the car to start the next race!");
		}
	}

	//Turns on the game over manager.
	public void lostRace()
	{
		m_Car.GetComponentInChildren<CarOnTriggerEnter> ().m_ShouldRecognizeTriggers = false;
		m_GameOverManager.enabled = true;
		m_GameOverManager.lost ();
		m_GameOverManager.m_Driving = true;
	}

	//Restarts the race. 
	public IEnumerator tryAgainRace()
	{
		setHudBack ();
		yield return new WaitForSeconds (.5f);
		m_CarRaceManagers [m_CurRacePhase].GetComponentInChildren<CarRaceManager> ().restart ();
		m_Car.GetComponentInChildren<CarOnTriggerEnter> ().m_ShouldRecognizeTriggers = true;
	}

	//restart the fight
	public void tryAgainFight(Vector3 playerPosition, Quaternion playerRotation)
	{
		m_RoundManagers [m_CurFightPhase].GetComponentInChildren<RoundManager> ().restart ();
		setHudBack();
		m_Player.transform.position = playerPosition;
		m_PlayerGuns.restoreAmmosAndGuns ();
		m_Player.GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.FirstPersonController> ().enabled = true;
		PlayerShooting playerShooting = m_Player.GetComponentInChildren<PlayerShooting> ();
		playerShooting.enabled = true;
		playerShooting.turnOnReloadScript ();
		startNextFight ();
	}

	//Sets the hud back.
	private void setHudBack()
	{
		m_HudAnimator.SetTrigger ("PressedTryAgain");
	}

	public void incrementFightPhase()
	{
        m_CurRaceOrFightPhase++;
        m_CurFightPhase++;
	}

	//Helper method to find a gameobject that is a child of a parent by name. 
	public GameObject findInChildren(GameObject go, string name)
	{
		foreach (Transform x in go.GetComponentsInChildren<Transform>()) {
			if (x.gameObject.name == name)
				return x.gameObject;
		}
		return null;
	}
}