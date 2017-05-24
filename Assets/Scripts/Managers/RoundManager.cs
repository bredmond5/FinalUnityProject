using UnityEngine;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 This class handles one whole fight. It handles setting up each enemy manager, giving the EnemiesLeftTimeManager
 the amount of enemies left to fight, restarting should the player die, and sending the information 
 that the player has won when there are no more enemies. 
 */

public class RoundManager : MonoBehaviour {

	[HideInInspector] public GameManager m_GameManager;
	[HideInInspector] public GameObject m_EnemiesLeftTimeManager;
	[HideInInspector] public GameObject m_RoundTextObject;
	[HideInInspector] public PlayerHealth m_PlayerHealth;
	public bool m_RoundDisplayWanted;
	public bool [] m_BreakWantedBetweenRounds;
	//	public bool m_BreakWantedBeforeNextRound;

	private GameObject[] m_EnemyManagers;
	private int m_CurManager = 0;
	private Timeable [] m_CurScripts;
	private Animator m_RoundTextAnim;
	private Text m_RoundText;
    private int enemiesLeft = 20;
    private int visualRound = 1;

	//Sets the managers and the amount of enemies in the text.
	public void setUpReferences() {
		m_RoundTextAnim = m_RoundTextObject.GetComponentInChildren<Animator> ();
		m_RoundText = m_RoundTextObject.GetComponentInChildren<Text> ();

		EnemiesLeftTimeManager enemiesLeftTime = m_EnemiesLeftTimeManager.GetComponentInChildren<EnemiesLeftTimeManager> ();

		enemiesLeftTime.m_RoundManager = this;
		enemiesLeftTime.m_IsDriving = false;
		enemiesLeftTime.enabled = true;

		setManagers ();
		setEnemiesAmount ();
	}

	//Turns off all of the enemyManagers besides the first one to signify the first round.
	private void setManagers()
	{
		m_EnemyManagers = new GameObject[transform.childCount];

		for (int i = 0; i < m_EnemyManagers.Length; i++) {
			m_EnemyManagers [i] = transform.GetChild (i).gameObject;
            foreach (Timeable script in m_EnemyManagers[i].GetComponentsInChildren<Timeable>())
            {
                script.setParentManager(this);
            }
        }

		m_EnemyManagers [m_CurManager].SetActive (true);
		for (int i = 1; i < m_EnemyManagers.Length; i++) {
			m_EnemyManagers [i].SetActive (false);
		}

        m_CurScripts = m_EnemyManagers [m_CurManager].GetComponentsInChildren<Timeable> ();

		if (m_RoundDisplayWanted) {
			StartCoroutine (displayRound ());
		} else {
			m_EnemiesLeftTimeManager.SetActive (true);
			m_EnemiesLeftTimeManager.GetComponentInChildren<EnemiesLeftTimeManager> ().m_IsDriving = false;
		}
	}

	//Sends to the EnemiesLeftTimeManager how many enemies there are.
	private void setEnemiesAmount()
	{
		int numEnemies = 0;
		for (int i = 0; i < m_CurScripts.Length; i++) {
			numEnemies += m_CurScripts[i].enemiesLeft();
		}
        enemiesLeft = numEnemies;
		EnemiesLeftTimeManager.m_NumEnemiesLeft = numEnemies;
	}

	//Sets up the next round by turning on the next enemyManager gameobject.
	public void nextRound()
	{
		m_EnemyManagers [m_CurManager].SetActive (false);

		if (!m_BreakWantedBetweenRounds [m_CurManager])
			startTheRound ();
	}

	public void startTheRound()
	{
		if (m_CurManager < m_EnemyManagers.Length - 1)
		{
			m_CurManager++;
		}else{
			playerFinished ();
			return;
		}

		m_EnemyManagers [m_CurManager].SetActive (true);
        m_CurScripts = m_EnemyManagers[m_CurManager].GetComponentsInChildren<Timeable>();

        if (m_RoundDisplayWanted)
			StartCoroutine(displayRound ());


		setEnemiesAmount ();
	}

	//Displays one set of the round. 
	private IEnumerator displayRound()
	{
		yield return new WaitForSeconds (1f);
		m_EnemiesLeftTimeManager.SetActive (true);
		m_EnemiesLeftTimeManager.GetComponentInChildren<EnemiesLeftTimeManager> ().m_IsDriving = false;
		m_RoundTextObject.SetActive(true);

        if (m_EnemyManagers[m_CurManager].GetComponentInChildren<WallManager>() == null)
        {
            m_RoundText.text = "Round " + visualRound;
            visualRound++;
        } else
        {
            m_RoundText.text = "Proceed";
        }
		
		m_RoundTextAnim.SetTrigger ("NewRound");
		yield return new WaitForSeconds (1.5f); //the animation durates 1.5 seconds.
		m_RoundTextObject.SetActive (false);
	}

	public void restart()
	{
		for (int i = 0; i < m_CurScripts.Length; i++) {
            m_CurScripts[i].reset();
		}

		m_EnemyManagers [m_CurManager].SetActive (false);
		m_CurManager = 0;
		m_EnemyManagers [m_CurManager].SetActive (true);

        m_CurScripts = m_EnemyManagers [m_CurManager].GetComponentsInChildren<Timeable> ();

		for (int i = 0; i < m_CurScripts.Length; i++) {
            m_CurScripts[i].begin();
		}

		if(m_RoundDisplayWanted)
			StartCoroutine(displayRound ());

		setEnemiesAmount ();
	}

	//Tells the gamemanager that the player finished all of the rounds and is ready for the next race.
	private void playerFinished()
	{
		Debug.Log ("hi");
		m_GameManager.incrementFightPhase ();
		m_RoundTextObject.SetActive(false);
		this.gameObject.SetActive(false);
		m_GameManager.startNextPhase ();
	}

    public void dudeDied()
    {
        enemiesLeft--;
        EnemiesLeftTimeManager.m_NumEnemiesLeft = enemiesLeft;

        if (enemiesLeft == 0)
        {
            nextRound();
        }

    }
}
