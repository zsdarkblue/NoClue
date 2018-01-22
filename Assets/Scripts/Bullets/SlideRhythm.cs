using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeatsFever;

public class SlideRhythm : RhythmBulletBase {

	public List<OnsetData> processDatas = new List<OnsetData> ();
	public List<OnsetData> datas = new List<OnsetData> ();
	public SlideRhythm(OnsetData data) : base(data.timeLine)
	{
		datas.Add (data);
	}

	public void AddNewData(OnsetData data)
	{
		datas.Add (data);
	}

	public void PrepareSlide()
	{
		processDatas.Clear();
		datas.ForEach(t => { processDatas.Add(t.Clone() as OnsetData); });

		int totalDataCount = processDatas.Count;
		float speed = 1f;
		Vector2 finalDir = new Vector2 (processDatas [totalDataCount - 1].positionX, processDatas [totalDataCount - 1].positionY) - 
			               new Vector2 (processDatas [totalDataCount - 2].positionX, processDatas [totalDataCount - 2].positionY);
		float totalTimeGap = processDatas [totalDataCount - 1].timeLine - processDatas [0].timeLine;
		totalTimeGap *= 0.001f;

		float totalDistanceToGo = totalTimeGap * speed;
		float nextNodeTotalDistance = 0;

		for (int i = 0; i < totalDataCount - 1; ++i) {
			Vector2 start = new Vector2 (processDatas [i].positionX, processDatas [i].positionY);
			Vector2 end = new Vector2 (processDatas [i + 1].positionX, processDatas [i + 1].positionY);
			float distanceToNextNode = Vector2.Distance (start, end);
			nextNodeTotalDistance += distanceToNextNode;

			Vector2 dir = (end - start).normalized;
			//can reach the node, the distance you can go is far than next node
			if (totalDistanceToGo > nextNodeTotalDistance) {
				//final vector, need go more distance to match then thythm'end time
				if (i == totalDataCount - 2) {
					float leftDistance = totalDistanceToGo - nextNodeTotalDistance;
					Vector2 finalPosition = end + dir * leftDistance;
					processDatas [totalDataCount - 1].positionX = finalPosition.x;
					processDatas [totalDataCount - 1].positionY = finalPosition.y;

					//Debug.Log (datas[0].slideID + " final vector, need go more distance to,index：" + i + "   pos:" + datas [datas.Count - 1].positionX + " " + datas [datas.Count - 1].positionY);
				}
				//change next node's time to adapt
				else {
					float timeGap = distanceToNextNode / speed;
					timeGap *= 1000f;
					processDatas [i + 1].timeLine = processDatas [i].timeLine + (int)timeGap;
					//Debug.Log (datas[0].slideID +" change next node's time to adapt,index：" + i + "  time" + datas [i + 1].timeLine);
				}
			}
			//can't reach the next node's position, go as far as you can, and set the final node's position there
			else {
				float leftTime = processDatas [i + 1].timeLine - processDatas [i].timeLine;
				leftTime *= 0.001f;
				float leftDistance = leftTime * speed;
				Vector2 finalPosition = start + dir * leftDistance;
				processDatas [i + 1].positionX = finalPosition.x;
				processDatas [i + 1].positionY = finalPosition.y;

				//have more node left, remove then all
				if (i < totalDataCount - 2) {
					int lastIndex = i + 1;
					processDatas.RemoveRange (lastIndex + 1,totalDataCount - lastIndex - 1);
					//Debug.Log (datas[0].slideID +" remove the left points：" + i + "  lastIndex" + lastIndex);
					break;
				}	
			}
		}
		processDatas.Sort(new CompareChain<OnsetData>().Add(item => item.timeLine));
	}

	public override void ReleaseBullet(float currentTime)
	{
		//int minTimeStep = (int)(App.Game.RhythmMgr.GetBPM () * RhyThmDefine.SliedPointGapParam);
		int minTimeStep = 22;

		float slideSections = processDatas.Count - 1;
		float slideSectionGap = 1 / slideSections;

		int itemIndex = 0;
		float slideDelaySeconds = App.Game.RhythmMgr.BulletPreShowSeconds * 0.6f;

		int gapTime = processDatas [processDatas.Count - 1].timeLine - processDatas [0].timeLine;
		int totalSlideItemNum = gapTime / minTimeStep;
		float currentSpeed = 0.69f / totalSlideItemNum;
		float scaleSpeed = Mathf.Min (0.025f,currentSpeed);
		//gapTime *= 0.001f;

		List<Vector3> lineEffectPos = new List<Vector3> ();
		MusicBulletBase firstBullet = null;
		for (int i = 0; i < processDatas.Count - 1; ++i) {

			var startPos = new Vector2 (processDatas [i].positionX, processDatas [i].positionY);
			var endPos = new Vector2 (processDatas [i + 1].positionX, processDatas [i + 1].positionY);

			for (int hitTime = processDatas [i].timeLine; hitTime < processDatas [i + 1].timeLine; hitTime += minTimeStep) {
				var go = RenderPoolManager.Instance.Create(RhythmBulletBase.BulletPrefabName);

				var remainTime = hitTime * 0.001f - currentTime;
				float totalGap = processDatas [i + 1].timeLine - processDatas [i].timeLine;
				float progress = 1 -( (processDatas [i + 1].timeLine - hitTime) / totalGap);

				var totalPorgress = slideSectionGap * i + progress * slideSectionGap;
				Vector2 targetGlassPos = Vector2.Lerp (startPos, endPos,progress);
				float showDelay = totalPorgress * RhyThmDefine.SlideItemShowDelay;

				var script = go.GetComponent<MusicBulletBase> ();
				if (firstBullet == null) {
					firstBullet = script;
				}
					
				var hitPos = script.StartMove (remainTime, targetGlassPos.x ,targetGlassPos.y , showDelay + slideDelaySeconds,true,itemIndex,scaleSpeed);
				lineEffectPos.Add (hitPos);
				itemIndex++;
			}
		}

		firstBullet.SetIndicateLine (lineEffectPos,gapTime * 0.001f);
	}
}
