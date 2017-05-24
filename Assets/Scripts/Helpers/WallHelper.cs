using UnityEngine;
using System.Collections;

public class WallHelper : MonoBehaviour {

    [HideInInspector] public WallManager parent;
    public WallPositionHelper[] physicalWalls;

    public void down()
    {
        foreach (WallPositionHelper wall in physicalWalls)
        {
            wall.down();
        }
    }

    public void up()
    {
        foreach (WallPositionHelper wall in physicalWalls)
        {
            wall.up();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            parent.playerPassedThrough();
        }
    }
}
