using UnityEngine;
using System.Collections;

public class WallPositionHelper : MonoBehaviour {

    public Transform wantedPosition;
    public Vector3 originalPosition;

	void Start () {
        originalPosition = gameObject.transform.position;
	}

    public void up()
    {
        gameObject.transform.position = originalPosition;
    }

    public void down()
    {
        gameObject.transform.position = wantedPosition.position;
    }

}
