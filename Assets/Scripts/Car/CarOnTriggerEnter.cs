using UnityEngine;
using System.Collections;

/*
 This class handles what should happen when the car encounters different colliders. 
 */

public class CarOnTriggerEnter : MonoBehaviour {

	[HideInInspector] public GameObject m_CurCheckPoint;
	[HideInInspector] public bool m_CheckPointChanged;
	[HideInInspector] public bool m_ShouldRecognizeTriggers = true;
	[HideInInspector] public CarRaceManager m_CarRaceManager;
	public int m_NumCarColliders = 4;

	private Rigidbody m_CarRigidbody;
	private int m_NumCarCollidersEncountered;

	void Start()
	{
		m_CarRigidbody = GetComponent<Rigidbody> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (m_ShouldRecognizeTriggers) {
			if (other.gameObject.CompareTag ("CheckPoint")) {
				m_NumCarCollidersEncountered++;

				if (m_NumCarCollidersEncountered  == m_NumCarColliders) {
					m_CurCheckPoint = other.gameObject;
					m_CarRaceManager.ChangeCheckPoint ();
					m_NumCarCollidersEncountered = 0;
				}

			} else if (other.gameObject.CompareTag ("GoBack")) {
				StartCoroutine (m_CarRaceManager.goBack (false));
			} else if (other.gameObject.CompareTag ("SpeedBoost")) {
				m_CarRigidbody.AddForce (other.GetComponentInChildren<SpeedBoost> ().m_Force * transform.forward);
				Debug.Log ("adding force!");
			}
		}
	}
}
