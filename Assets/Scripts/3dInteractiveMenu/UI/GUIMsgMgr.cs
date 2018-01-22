using System.Collections.Generic;
using UnityEngine;

namespace BeatsFever.UI
{
    /** base class for all gui msg
     *
     * you can inherit this class to implement your own event type
     */
    public class GUIMsg
    {
		public static int StringToHash(string ss)
		{
			// todo: need a better solution for this hash function
			char[] str = ss.ToCharArray();
			uint hash = 0;
			uint x = 0;
			uint i = 0;
			uint seed = 131;
			while(i < str.Length)
			{
				hash = (hash << 4) * seed + (str[i++]);
				if((x = hash & 0xF0000000) != 0)
				{
					hash ^= (x >> 24);
					hash &= ~x;
				}
			}
			return (int)(hash & 0x7FFFFFFF);
		}

        public GUIMsg(string name)
        {
            ID = StringToHash(name); 
        }

        public virtual void Execute(Dictionary<int, System.Delegate> handlerDic)
        {
            if (handlerDic.ContainsKey(ID))
            {
                ((GUIMsgMgr.GUIMsgHandler)handlerDic[ID])(this);
            }
            else
            {
				Debug.LogWarning("unknown msg: " + this.ID);
            }
        }

        /** the msg id */
        public int ID { get; set; }

        public bool NeedLockSend;
    }

    public class GUIAutoSpellMsg : GUIMsg
    {
        public static readonly string name = "OpenLeftAutoSpell";
        public bool AutoSpell;

        public GUIAutoSpellMsg(bool autoSpell)
            : base(name)
        {
            this.AutoSpell = autoSpell;
        }
    }

    public class GUISendUpwardsMsg : GUIMsg
    {
        public GUISendUpwardsMsg(GameObject go, string functionName, object param, SendMessageOptions options = SendMessageOptions.DontRequireReceiver)
            : base(functionName)
        {
            this.go = go;
            this.functionName = functionName;
            this.param = param;
            this.options = options;
            
            NeedLockSend = true;
        }

        public override void Execute(Dictionary<int, System.Delegate> handlerDic)
        {
			if(null == go)
			{
				return;
			}

            if (param == null)
            {
                go.SendMessageUpwards(functionName, options);
            }
            else
            {
                go.SendMessageUpwards(functionName, param, options);
            }
        }

        private GameObject go;
        private string functionName;
        private object param;
        private SendMessageOptions options;
    }
    
    /** global gui msg manager
     *
     * receive the msg from gui module and send it to the handler
     */
    public class GUIMsgMgr
    {
        /** the delegate to handle a specified gui msg */
        public delegate void GUIMsgHandler(GUIMsg msg);

        /** msg queue */
        private Queue<GUIMsg> msgs = new Queue<GUIMsg>();

        /** msg dictionary */
        private Dictionary<int,int> msgsDic = new Dictionary<int, int>();

        /** handler dictionary */
        private Dictionary<int, System.Delegate > handlerDic = new Dictionary<int, System.Delegate >();

		private float minMessageIntervalTime = 0.2f;
		private float timeCounter = 0;
		private bool lockMessageQueue = false;
        
        /** the start method
         *
         * you must invoke this method first
         */
        public void Start()
        {
        }

        /** the clean method
         *
         * you must invoke this method at last
         */
        public void Shutdown()
        {
            msgs.Clear();
            msgsDic.Clear();
            handlerDic.Clear();
        }

        /** register msg handler
         *
         * @param[in] id msg id
         * @param[in] handler msg handler
         */
        public void RegisterHandler(int id, GUIMsgHandler handler)
        {
            if(handler == null)
            {
				Debug.LogWarning("handler is null");
                return;
            }
            
            if(!handlerDic.ContainsKey(id))
            {
                handlerDic.Add(id, null);
            }
            
			handlerDic[id] = (GUIMsgHandler)handlerDic[id] + handler;
        }

		public bool IsRegisteredHandler(int id)
		{
			return handlerDic.ContainsKey(id);
		}

        /** unregister the specific handler for msg which has the 'id'
         *
         * @param[in] id msg id
         * @param[in] handler msg handler
         */
        public void UnRegisterHandler(int id, GUIMsgHandler handler)
        {
            if(handler == null)
            {
                return;
            }
            
            if(!handlerDic.ContainsKey(id))
            {
                return;
            }
            
            handlerDic[id] = (GUIMsgHandler)handlerDic[id] - handler;
            
            if(handlerDic[id] == null)
            {
                handlerDic.Remove(id);
            }
        }

        /** queue the msg, handle it next frame
         *
         * @param[in] msg the sent msg
         */
        public void SendMsg(GUIMsg msg)
        {
			if(msg == null || (msg.NeedLockSend && lockMessageQueue))
            { 
                return;
            }

            msgs.Enqueue(msg);
			lockMessageQueue = true;
        }

        bool InteruptMsg(GUIMsg msg)
        {
            if(App.Game.GUIFrameMgr.IsActive(GUIFrameID.NetWaitUIFrame))
            {
                return true;
            }

            return false;
        }

        /** handle the msg immediate
         *
         * @param[in] msg the handled msg
         */

        public void HandleMsg(GUIMsg msg)
        {
            if(msg == null)
            { 
                return;
            }

            if(InteruptMsg(msg))
            {
                return;
            }

            msg.Execute(handlerDic);
        }

        /** main loop. update every frame
         */
        public void Update()
        {
			if(lockMessageQueue)
			{
				timeCounter += Time.deltaTime;
				if(timeCounter > minMessageIntervalTime)
				{
					timeCounter = 0;
					lockMessageQueue = false;
				}
			}

            while (msgs.Count > 0)
            {
                GUIMsg msg = msgs.Dequeue();
                HandleMsg(msg);
            }
        }
    }
}