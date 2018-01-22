using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BeatsFever.UnityEx
{
    public static class UIGridExtension
    {
        /*
        public static void ForceReposition(this UIGrid grid)
        {
            //grid.enabled = true;

            Transform myTrans = grid.gameObject.transform;
            
            int x = 0;
            int y = 0;
            
            for (int i = 0; i < myTrans.childCount; ++i)
            {
                Transform t = myTrans.GetChild(i);
                
				if(!t.gameObject.activeSelf) continue;
                if (!NGUITools.GetActive(t.gameObject) && grid.hideInactive) continue;
                
                float depth = t.localPosition.z;
                Vector3 pos = (grid.arrangement == UIGrid.Arrangement.Horizontal) ?
                    new Vector3(grid.cellWidth * x, -grid.cellHeight * y, depth) :
                        new Vector3(grid.cellWidth * y, -grid.cellHeight * x, depth);
                
                if (grid.animateSmoothly && Application.isPlaying)
                {
                    SpringPosition.Begin(t.gameObject, pos, 15f);
                }
                else t.localPosition = pos;
                
                if (++x >= grid.maxPerLine && grid.maxPerLine > 0)
                {
                    x = 0;
                    ++y;
                }
            }
        }

		public static void ResetScrollAbility(this UIGrid grid,int noScrollMaxChildNum)
		{
			Transform myTrans = grid.gameObject.transform;
			int totalCount = 0;
			
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				if(t.gameObject.activeSelf)
				{
					totalCount++;
				}
			}
			if(0 == totalCount)
			{
				return;
			}

			UIScrollView drag = NGUITools.FindInParents<UIScrollView>(grid.gameObject);
			if (drag != null)
			{
				drag.ResetPosition();
				drag.enabled = (totalCount > noScrollMaxChildNum);

				grid.ForceReposition();
			}
		}
     */   
    }
}
