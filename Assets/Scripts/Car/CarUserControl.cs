using System;
using UnityEngine;

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
		[HideInInspector] public bool carOn = false;
        private CarController m_Car; // the car controller we want to use

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }


        private void FixedUpdate()
		{
			float h = 0f;
			float v = 0f;
				// pass the input to the car!

			if (carOn) {
				h = Input.GetAxis ("Horizontal");
				v = Input.GetAxis ("Vertical");
			}
#if !MOBILE_INPUT
				float handbrake = Input.GetAxis ("Jump");
				m_Car.Move (h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
        }
	}
}
