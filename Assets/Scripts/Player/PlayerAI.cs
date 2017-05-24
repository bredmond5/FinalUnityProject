using UnityEngine;
using System.Collections;

public class PlayerAI : MonoBehaviour {

	[HideInInspector] public Transform m_CarDoor;

	private NavMeshAgent m_Nav;

	// Use this for initialization
	void Start () {
		m_Nav = GetComponent<NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (m_CarDoor != null) {
			m_Nav.SetDestination (m_CarDoor.position);
		}
	}
}
