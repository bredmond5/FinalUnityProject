using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 This class controls one race. 
 */

public class CarRaceManager : MonoBehaviour {

	[HideInInspector] public GameManager m_GameManager;
	[HideInInspector] public GameObject m_Car;
	[HideInInspector] public GameObject m_EnemiesLeftTimeManager;
	[HideInInspector] public GameObject m_TimeToStartRaceText; // A reference to the countdown text.
	public GameObject m_CheckPoints;
	public int m_TimeToFinish;
	public bool m_CountDownWanted;

	private GameObject m_CurCheckPoint;
	private UnityStandardAssets.Vehicles.Car.CarController m_CarController;
	private CarOnTriggerEnter m_CarOnTriggerEnter;

	/*
	 Manages one race.
	 */

	//Sets up all the references the racemanager will need. Car is set from the gamemanager script.
	public void setUpReferences()
	{
		m_CarOnTriggerEnter = m_Car.GetComponentInChildren<CarOnTriggerEnter> ();
		m_CarController = m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarController> ();
	}

	//Starts the timer and sets the first checkpoint.
	public IEnumerator startRace()
	{
		m_CurCheckPoint = m_CheckPoints.transform.GetChild (0).gameObject;

		if (m_CountDownWanted) {
			m_TimeToStartRaceText.SetActive (true);
			StartCoroutine (m_TimeToStartRaceText.GetComponentInChildren<CountDown> ().countDown ());
			yield return new WaitForSeconds (3);
		}

		m_EnemiesLeftTimeManager.SetActive (true);

		EnemiesLeftTimeManager enemiesLeftTime = m_EnemiesLeftTimeManager.GetComponentInChildren<EnemiesLeftTimeManager> ();
		enemiesLeftTime.m_CarRaceManager = this;

		EnemiesLeftTimeManager.m_Timer = m_TimeToFinish + 1;
		EnemiesLeftTimeManager.m_TimeLeft = m_TimeToFinish;

		enemiesLeftTime.m_IsDriving = true;
		enemiesLeftTime.enabled = true;

		if (m_CountDownWanted) {
			yield return new WaitForSeconds (1);
			m_TimeToStartRaceText.SetActive(false);
		}
	}

	public void outOfTime()
	{
		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl> ().carOn = false;
		m_GameManager.lostRace ();
		Debug.Log ("You ran out of time!");
	}

	public void restart()
	{
		m_EnemiesLeftTimeManager.SetActive (false);

		StartCoroutine(startRace ());
		StartCoroutine(goBack(true));
	}

	public void ChangeCheckPoint()
	{
		m_CurCheckPoint = m_CarOnTriggerEnter.m_CurCheckPoint;
		string name = m_CurCheckPoint.name;
		if (name == "CheckPointLast") { 
			raceFinished();

		}
	}

	private void raceFinished()
	{
		Debug.Log("finished a race!");
		this.gameObject.SetActive(false);
		m_GameManager.slowDown ();
	}

	public IEnumerator goBack(bool restart)
	{
		m_Car.transform.position = m_CurCheckPoint.transform.position;

		Rigidbody rb = m_Car.GetComponentInChildren<Rigidbody> ();
		rb.velocity = new Vector3 (0f, 0f, 0f);

		rb.constraints = RigidbodyConstraints.FreezeRotation;
		rb.constraints = RigidbodyConstraints.None;

		m_CarController.turnLarge (m_CurCheckPoint.transform);

		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl> ().carOn = false;

		if(m_CountDownWanted && restart)
			yield return new WaitForSeconds (3);

		m_Car.GetComponentInChildren<UnityStandardAssets.Vehicles.Car.CarUserControl> ().carOn = true;
	}
}
