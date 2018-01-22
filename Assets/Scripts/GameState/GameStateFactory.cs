using System;


namespace BeatsFever.GameState
{
    class GameStateFactory
    {
        public static void Build(GameStateMgr gsm)
        {
            gsm.Register(new BootState(GameStateID.ST_Boot));
			gsm.Register(new LobbyState(GameStateID.ST_Lobby));
			gsm.Register(new BeatState(GameStateID.ST_Beat));
			gsm.Register(new RoomSwitchMiddleState(GameStateID.ST_RoomMiddle));

			gsm.Register(new SwitchingState(GameStateID.ST_Switching));
        }
    }
}
