using UnityEngine;
using System.Collections;

namespace BeatsFever
{
	public enum InputType
	{
		Click,
		Pause,
		Resume,
	}
	
	public class InputMgr
	{
		public delegate void OnUserInputHandler(InputType type);
		public OnUserInputHandler OnUserInput;

		public void Start()
		{
			
		}

		public void Shutdown()
		{
			
		}

		public void BroadcastResumeInput()
		{
			OnUserInput (InputType.Resume);
		}

		public void Update()
		{
			if (OnUserInput == null) {
				return;
			}

			if (Input.GetMouseButtonDown (0)) {
				if (null != OnUserInput) {
					OnUserInput (InputType.Click);
				}	
			}
			else if (Input.GetKeyDown (KeyCode.Escape)) {
				if (Time.timeScale == 1) {
					OnUserInput (InputType.Pause);
				}
				else if (Time.timeScale == 0) {
					OnUserInput (InputType.Resume);
				}
				else {
				
				}
			}
		}

		public void MenuButtonClick()
		{
			if (OnUserInput == null) {
				return;
			}
				
			if (Time.timeScale == 1) {
				OnUserInput (InputType.Pause);
			}
			else if (Time.timeScale == 0) {
				OnUserInput (InputType.Resume);
			}
		}
	}
}