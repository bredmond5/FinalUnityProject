using UnityEngine;
using System.Collections;

public class EnemyDropsAmmo : MonoBehaviour {

	public GameObject m_AmmoCrate;
	public int m_PercentDrop;

	public void dropAmmo()
	{
		bool shouldDrop = findIfShouldDrop ();
		if (shouldDrop) {
			Vector3 position = new Vector3 (transform.position.x, m_AmmoCrate.transform.localScale.y / 2, transform.position.z);
			Instantiate (m_AmmoCrate, position, transform.rotation);
		}
	}

	private bool findIfShouldDrop()
	{
		int random = Random.Range (0, 100); 
		return random <= m_PercentDrop;
	}
}
