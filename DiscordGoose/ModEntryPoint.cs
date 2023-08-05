using System;
using System.Diagnostics;
using GooseShared;

namespace DiscordGoose
{
	public class ModEntryPoint : IMod
	{
		private static RPC GoosePresence = new RPC("1113388907606855740");

		private static int delay = 120;

		void IMod.Init()
		{
			InjectionPoints.PreTickEvent += new InjectionPoints.PreTickEventHandler(PreTick);
			GoosePresence.Init();
			Process.GetCurrentProcess().Exited += OnProcessExit;
		}

		public static void PreTick(GooseEntity g)
		{
			if (delay == 0)
			{
				GoosePresence.Update(g);
				delay = 120;
				return;
			}
			delay--;
		}

		public static void OnProcessExit(object sender, EventArgs e)
		{
			GoosePresence.Disconnect();
		}
	}
}
