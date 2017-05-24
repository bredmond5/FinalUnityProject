using UnityEngine;
using System.Collections;

public class CarSlowDown : MonoBehaviour {
	[HideInInspector] public GameManager m_GameManager;

	private Rigidbody m_Rigidbody;
	private Vector3 m_AmountToSlowBy = Vector3.zero;

	void Start () {
		m_Rigidbody = GetComponent<Rigidbody> ();
		enabled = false;
	}

	void Update (){
		Debug.Log("Car: " + m_Rigidbody.velocity.magnitude.ToString());
		if (m_Rigidbody.velocity.magnitude > 5f) {
			m_Rigidbody.velocity *= .90f;

		} else if(m_Rigidbody.velocity.magnitude > .5f){
			if (m_AmountToSlowBy != Vector3.zero) {
				m_Rigidbody.velocity += m_AmountToSlowBy;

			} else {
				Debug.Log("Going to second phase of slow down");
				m_AmountToSlowBy = (m_Rigidbody.velocity / -10);
				m_Rigidbody.velocity += m_AmountToSlowBy;
			}
		}else{
			Debug.Log("Getting Out of Car");
			m_Rigidbody.velocity = Vector3.zero;
			m_GameManager.getOut ();
			m_AmountToSlowBy = Vector3.zero;
			enabled = false;
		}
	}
}
