using UnityEngine;
using System.Collections;

public class WallManager : MonoBehaviour, Timeable {

    public WallHelper m_wall;
    RoundManager m_parent;
    bool passed = false;

	void Start () {
        m_wall.parent = this;
        begin();
	}

    public void begin()
    {
        m_wall.down();
    }

    public void reset()
    {
        m_wall.up();
    }

    public int enemiesLeft()
    {
        return 0;
    }

    public void setParentManager(RoundManager parent)
    {
        m_parent = parent;
    }

    public void playerPassedThrough()
    {
        if (passed)
        {
            return;
        }

        passed = true;
        m_wall.up();
        m_parent.nextRound();
    }

}
