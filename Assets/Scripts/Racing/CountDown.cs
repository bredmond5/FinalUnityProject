using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDown : MonoBehaviour {

	private Text m_Text;
	private Animator m_Anim;

	void OnEnable () {
		m_Text = GetComponent<Text> ();
		m_Anim = GetComponent<Animator> ();
	}

	public IEnumerator countDown()
	{
		int numSeconds = 3;
		string animTrigger = "NextSecond";
		m_Text.text = numSeconds.ToString ();
		m_Anim.SetTrigger (animTrigger);
		for (int i = numSeconds - 1; i >= 0; i--) {
			m_Anim.SetTrigger ("GoBackToEmpty");
			yield return new WaitForSeconds (1);
			m_Text.text = i.ToString ();
			if (i == 0) {
				m_Text.text = "Go!";
			}
			m_Anim.SetTrigger (animTrigger);
		}
	}
}
