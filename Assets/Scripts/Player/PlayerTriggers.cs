using UnityEngine;
using System.Collections;

public class PlayerTriggers : MonoBehaviour {

	[HideInInspector] public bool m_IsInsideCarRange;
	[HideInInspector] public GameObject m_Car;
	[HideInInspector] public GameManager m_GameManager;

	void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.CompareTag("DriverDoor")) {
            m_IsInsideCarRange = true;
            m_Car = other.transform.parent.gameObject;

        } else if (other.gameObject.CompareTag("AmmoCrate")) {
            AmmoManager.m_AmmoLeft += GetComponentInChildren<PlayerShooting>().m_AmountGivenAmmoCrate;
            Destroy(other.gameObject);

        } else if (other.gameObject.CompareTag("HealthCrate"))
        {
            GameObject.Find("Player").GetComponentInChildren<PlayerHealth>().addHealth(other.GetComponentInChildren<HealthDropAmount>().healthDrop);
            Destroy(other.gameObject);

		} else if (other.gameObject.CompareTag ("EndBreak")) {
			m_GameManager.endBreak ();
			Destroy(other.gameObject);

		}else if(other.gameObject.CompareTag("EndFightBreak"))
		{
			m_GameManager.endFightBreak ();
			Destroy (other.gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("DriverDoor"))
			m_IsInsideCarRange = false;
	}
}
