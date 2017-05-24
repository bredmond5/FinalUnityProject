using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemiesLeftTimeManager : MonoBehaviour
{
	public CarRaceManager m_CarRaceManager;
    public static int m_NumEnemiesLeft;       
	public static float m_TimeLeft;
	public static int m_Timer;
	[HideInInspector] public RoundManager m_RoundManager;
	[HideInInspector] public bool m_IsDriving;

    private Text text;                      // Reference to the Text component

	//Gets the reference to the text, makes it an empty string and turns numEnemiesLeft to 0.
    void Awake ()
	{
		// Set up the reference.
		text = GetComponent <Text> ();
		text.text = "";
		m_NumEnemiesLeft = 0;            
	}

    void Update ()
	{
		if (m_IsDriving) {
			updateTime ();
		}else{
			text.text = "Enemies Left: " + m_NumEnemiesLeft.ToString ();
			if (m_NumEnemiesLeft == 0) {
				text.text = "";
			}
		}		
	}

	//Handles changing the amount of time left.
	private void updateTime()
	{
		m_TimeLeft -= Time.deltaTime;
		if (m_TimeLeft <= m_Timer - 1) {
			m_Timer--;
			text.text = changeText (m_Timer);
		}

		if (m_TimeLeft <= -0.5) {
			m_CarRaceManager.outOfTime ();
			enabled = false;
		}
	}

	//Splits up the text, 100 seconds = 1:40. 9 seconds = 09.
	public string changeText(int time)
	{
		if (time / 60 != 0) {
			int numMins = time / 60;
			int numSeconds = time % 60;
			if (numSeconds > 9) {
				return numMins + ":" + numSeconds;
			} else {
				return numMins + ":0" + numSeconds;
			}
		} else {
			if (time > 9) {
				return time.ToString ();
			} else {
				return "0" + time.ToString();
			}
		}
	}
}
