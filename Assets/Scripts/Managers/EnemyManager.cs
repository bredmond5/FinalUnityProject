using UnityEngine;

/*
 This class controls the spawning of one type of enemy for a set number of spawns.
 */

public class EnemyManager : MonoBehaviour, Timeable
{
    [HideInInspector] public PlayerHealth m_PlayerHealth;       // Reference to the player's heatlh.
    public GameObject m_Enemy;                // The m_Enemy prefab to be spawned.
    public float m_SpawnTime = 3f;            // How long between each spawn.
    public Transform[] m_SpawnPoints;         // An array of the spawn points this m_Enemy can spawn from.
	public int m_NumSpawnsWanted = 20; 		  // The amount of spawns wanted
	public int m_ExtraTimeBeforeSpawnStarts = 0;				  // The amount of time before spawns start.

	private int m_NumSpawned = 0;			  // The amount that have been spawned.
	private GameObject [] m_Enemies;		  // All enemies are instantiated into this so that they can be destroyed should the player die.
    private RoundManager m_parent;

    public void begin()
    {
        m_NumSpawned = 0;
        m_Enemies = new GameObject[m_NumSpawnsWanted];
        m_Enemy.GetComponentInChildren<EnemyHealth>().m_parent = this;

        // Call the Spawn function after a delay of the m_SpawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", m_SpawnTime + m_ExtraTimeBeforeSpawnStarts, m_SpawnTime);
    }

    public void Start ()
    {
        begin();
    }


    void Spawn ()
    {
        // If the player has no health left or if numSpawned has reached numSpawnsWanted.
		if(GameObject.Find("Player").GetComponentInChildren<PlayerHealth>().m_CurrentHealth <= 0f || m_NumSpawned == m_NumSpawnsWanted - 1)
        {
			CancelInvoke ("Spawn");
        }

        // Find a random index between zero and one less than the number of spawn points.
        int spawnPointIndex = Random.Range (0, m_SpawnPoints.Length);

        // Create an instance of the m_Enemy prefab at the randomly selected spawn point's position and rotation.
		m_Enemies[m_NumSpawned] = Instantiate (m_Enemy, m_SpawnPoints[spawnPointIndex].position, m_SpawnPoints[spawnPointIndex].rotation)as GameObject;
		m_NumSpawned++;
    }

	public void reset()
	{
		for (int i = 0; i < m_NumSpawnsWanted; i++) {
			if (m_Enemies [i] != null) {
				Destroy(m_Enemies[i]);
			} else {
				CancelInvoke ("Spawn");
				enabled = false;
				break;
			}
		}
	}

    public int enemiesLeft()
    {
        return m_NumSpawnsWanted;
    }

    public void setParentManager(RoundManager parent)
    {
        m_parent = parent;
    }

    public void dudeDied()
    {
        m_parent.dudeDied();
    }
}
