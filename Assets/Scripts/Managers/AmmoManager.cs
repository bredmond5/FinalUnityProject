using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoManager : MonoBehaviour {

	public static int m_AmmoInClip;
	public static int m_AmmoLeft;

	private Text m_AmmoText;

	void Start () {
		m_AmmoText = GetComponent<Text> ();
		m_AmmoText.text = m_AmmoInClip.ToString();
	}

	void Update()
	{
		m_AmmoText.text = m_AmmoInClip + "/" + m_AmmoLeft;
	}
}
