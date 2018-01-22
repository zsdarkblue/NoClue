using UnityEngine;
using System.Collections;
using BeatsFever;
using BeatsFever.UI;

public class FrameBaseUI : MonoBehaviour {

	protected GUIFrame frame;
	private int nextFrameID;
	public void Init(GUIFrame _frame)
	{
		frame = _frame;
	}

	public virtual void FadeIn()
	{
		
	}

	public virtual void FadeOut()
	{
		
	}

	public void SetNextFrameID(int id)
	{
		nextFrameID = id;
	}

	public void DeActive()
	{
		if (nextFrameID > 0) {
			App.Game.GUIFrameMgr.Active (nextFrameID);
			nextFrameID = 0;		
		}
		frame.IsCloseAniDone = true;
		App.Game.GUIFrameMgr.DeActive (frame.ID);
	}

	void OnDisable()
	{
		if (null != frame) {
			frame.IsCloseAniDone = false;		
		}
	}
}
