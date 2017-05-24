using UnityEngine;
using System.Collections;

public class EnemyDropsHealth : MonoBehaviour {

    public float m_PercentDrop = 100;
    public GameObject healthCrate;

    public void dropHealth()
    {
        bool shouldDrop = findIfShouldDrop();
        if (shouldDrop)
        {
            Vector3 position = new Vector3(transform.position.x, healthCrate.transform.localScale.y / 2, transform.position.z);
            Instantiate(healthCrate, position, transform.rotation);
        }
    }

    private bool findIfShouldDrop()
    {
        int random = Random.Range(0, 100);
        return random <= m_PercentDrop;
    }

}
