using UnityEngine;
using System.Collections;

public class HorseAnimation : MonoBehaviour {

    public Animator m_Animator;
    public AudioClip m_Gallop;
    public AudioClip m_Walk;

    private Rigidbody m_Rigidbody;
    private bool m_isMoving = false;
    private AudioSource m_HorseAudio;
    private bool m_Walking = false;

	void Start () {
        m_HorseAudio = GameObject.Find("Horse").GetComponentInChildren<AudioSource>();
        
		m_Rigidbody = GetComponent<Rigidbody> ();
        m_isMoving = false;

	}

	void Update (){

        if (m_Rigidbody.velocity.magnitude > 0.2f)
        {
            m_isMoving = true;
            StartMoving();
        }
        else if (m_Rigidbody.velocity.magnitude <= 0.2f)
        {
            m_isMoving = false;
            StopMoving();
        }
        if (m_isMoving)
        {
            float spd = m_Rigidbody.velocity.magnitude/10f;
            changeAudioClip(spd);
            m_Animator.SetFloat("Speed", spd);
        }


    }

    void changeAudioClip(float spd)
    {
        if(spd < 1 && !m_Walking)
        {
            m_Walking = true;
            m_HorseAudio.clip = m_Walk;
            m_HorseAudio.Play();
        }
        else if(spd > 1 && m_Walking)
        {
            m_Walking = false;
            m_HorseAudio.clip = m_Gallop;
            m_HorseAudio.Play();
        }
    }

    void StartMoving()
    {
        m_HorseAudio.Play();
        m_Animator.SetBool("MOVING", true);
    }
    void StopMoving()
    {
        m_HorseAudio.Stop();
        m_Animator.SetBool("MOVING", false);
    }
}
