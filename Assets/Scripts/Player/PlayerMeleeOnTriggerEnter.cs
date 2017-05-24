using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerMeleeOnTriggerEnter : MonoBehaviour {

	public List<EnemyHealth> m_EnemyHealthsInSphere;
//	private int i;

	void Start () {
		m_EnemyHealthsInSphere = new List<EnemyHealth> ();
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
			m_EnemyHealthsInSphere.Add(other.gameObject.GetComponentInChildren<EnemyHealth>());
//			i++;
//			Debug.Log (i);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag ("Enemy")) {
			m_EnemyHealthsInSphere.Remove (other.gameObject.GetComponentInChildren<EnemyHealth> ());
//			i--;
//			Debug.Log (i);
		}
	}
		
	public List<EnemyHealth> returnEnemiesInSphereList()
	{
		return m_EnemyHealthsInSphere;
	}
}
