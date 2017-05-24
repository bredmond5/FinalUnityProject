using UnityEngine;
using System.Collections;

public class PlayerShootingShotgun : PlayerShooting {

	public float m_Spread;
	public int m_NumPelletsPerShell;

	public void setUpShotgun() {
		m_LineRenderers = GetComponentsInChildren<LineRenderer> ();
		for (int i = 0; i < m_LineRenderers.Length; i++) {
			m_LineRenderers [i].enabled = false;
		}
	}
	
	public void shootShotgun()
	{
		shootTheShotgun (m_Spread, m_NumPelletsPerShell);
	}
}
