using System;


namespace BeatsFever.GameState
{
    public abstract class GameState
    {
        private int id;
        private GameStateMgr mgr;
    
        public int ID { get { return this.id; } }

        public GameStateMgr Mgr { get { return this.mgr; } }

        public virtual bool NeedSwitchingScene
        {
            get { return true; }
        }
    
        public GameState(int id)
        {
            this.id = id;
        }
       
        internal void SetMgr(GameStateMgr gsm)
        {
            this.mgr = gsm;
        }
    
        public virtual void Release()
        {
        }

        public virtual void Enter(GameStateParam param)
        {
        }

        public virtual void Exit()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void OnGUI()
        {
        }
    }
}