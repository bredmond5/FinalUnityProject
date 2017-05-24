using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountUp : EnemiesLeftTimeManager {

	private Text m_CountText;
	private int m_Timerr;
	private float m_Time;

	private void Start()
	{
		m_CountText = GetComponent<Text> ();
	}

	// Update is called once per frame
	private void Update () {
		m_Time += Time.deltaTime;
		if (m_Time >= m_Timerr + 1) {
			m_Timerr++;
			m_CountText.text = "Time Passed " + changeText (m_Timerr);
		}
	}
}
