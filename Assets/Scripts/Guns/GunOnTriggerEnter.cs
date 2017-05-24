using UnityEngine;
using System.Collections;

public class GunOnTriggerEnter : MonoBehaviour {

	public PlayerGunsManager m_PlayerGuns;
	public int m_WhichGunToSwitchFor; //Put -1 if you want a whole new gun slot.

	private bool m_IsInRange;

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
			m_IsInRange = true;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
			m_IsInRange = false;
	}

	void Update()
	{
		if (m_IsInRange && Input.GetKeyDown (KeyCode.Z)) {
			m_PlayerGuns.addGun (gameObject, m_WhichGunToSwitchFor);
		}
	}
}
