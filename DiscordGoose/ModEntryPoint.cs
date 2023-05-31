using System;
using System.Diagnostics;
using GooseShared;

namespace DiscordGoose
{
	public class ModEntryPoint : IMod
	{
		private static int delay = 120;

		void IMod.Init()
		{
			InjectionPoints.PreTickEvent += new InjectionPoints.PreTickEventHandler(this.PreTick);
			this.GoosePresence.Init();
			Process.GetCurrentProcess().Exited += this.OnProcessExit;
		}

		public void PreTick(GooseEntity g)
		{
			if (delay == 0)
			{
				this.GoosePresence.Update(g);
				delay = 120;
				return;
			}
			delay--;
		}

		public void OnProcessExit(object sender, EventArgs e)
		{
			this.GoosePresence.Disconnect();
		}

		private RPC GoosePresence = new RPC("1113388907606855740");
	}
}
