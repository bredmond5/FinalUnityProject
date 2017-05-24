using UnityEngine;
using System.Collections;

public class CylinderOnTriggerEnter : MonoBehaviour
{ 

    private UnityStandardAssets.Characters.FirstPerson.FirstPersonController m_FirstPersonController;

    void Start()
    {
        m_FirstPersonController = GetComponentInParent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        {
            m_FirstPersonController.changeJumpingAbility(false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain"))
        {
            m_FirstPersonController.changeJumpingAbility(true);
        }
    }
}