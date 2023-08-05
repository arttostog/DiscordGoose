using System;
using System.Reflection;
using DiscordRPC;
using DiscordRPC.Logging;
using DiscordRPC.Message;
using GooseShared;


namespace DiscordGoose
{
    internal class RPC
	{
		private Timestamps Now;

		private DiscordRpcClient Client;

		private GooseEntity Goose;

		private string[] tasks;

		private string prevTask;

		private float prevSpeed;

		private string speedTier;

		private string currentTask;

		private bool isRunning;

		private static Tasks.Tasks_ t = new Tasks.Tasks_();

		public RPC(string ClientId)
		{
			this.Client = new DiscordRpcClient(ClientId);
			this.Now = Timestamps.Now;
		}

		public void Init()
		{
			this.tasks = API.TaskDatabase.getAllLoadedTaskIDs.Invoke();
			this.Client.Initialize();
			this.Client.Logger = new ConsoleLogger
			{
				Level = LogLevel.Warning
			};
			this.Client.OnConnectionEstablished += this.Client_OnConnectionEstablished;
			this.Client.OnConnectionFailed += this.Client_OnConnectionFailed;
			this.Client.OnReady += this.Client_OnReady;
		}

		private void Client_OnConnectionFailed(object sender, ConnectionFailedMessage args)
		{
			Console.WriteLine("[{1}] Трубное подключение не удалось. Нет соединения с трубой #{0}", args.FailedPipe, args.TimeCreated);
			this.isRunning = false;
		}

		private void Client_OnReady(object sender, ReadyMessage args)
		{
			Console.WriteLine("[{0}][{1}] Получена готовность от пользователя {2}", args.Version, args.TimeCreated, args.User);
			this.isRunning = true;
		}

		private void Client_OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
		{
			Console.WriteLine("[{1}] Соединение установленно! Подключено к трубе #{0}", args.ConnectedPipe, args.TimeCreated);
		}

		private static string getRand(string[] s)
		{
			return s[new Random().Next(s.Length)];
		}

		public void Disconnect()
		{
			this.Client.Dispose();
		}

		public void Update(GooseEntity g)
		{
			if (!this.isRunning)
			{
				return;
			}
			this.Goose = g;
			float walkSpeed = this.Goose.parameters.WalkSpeed;
			float runSpeed = this.Goose.parameters.RunSpeed;
			float chargeSpeed = this.Goose.parameters.ChargeSpeed;

			if (this.prevSpeed != this.Goose.currentSpeed || this.prevTask != this.tasks[this.Goose.currentTask])
			{
				this.speedTier = ((this.Goose.currentSpeed == walkSpeed) ? (getRand(Tasks.Walk) + " (Walking)") : ((this.Goose.currentSpeed == runSpeed) ? (getRand(Tasks.Run) + " (Running)") : ((this.Goose.currentSpeed == chargeSpeed) ? (getRand(Tasks.Charge) + " (Charging)") : string.Format("{0} (Speed: {1})", getRand(Tasks.Custom), this.Goose.currentSpeed))));
				foreach (FieldInfo fieldInfo in typeof(Tasks.Tasks_).GetFields())
				{
					if (fieldInfo.Name == this.tasks[this.Goose.currentTask])
					{
						this.currentTask = getRand((string[])fieldInfo.GetValue(RPC.t)) + " (" + fieldInfo.Name + ")";
						break;
					}
					this.currentTask = getRand(Tasks.Custom) + " (" + this.tasks[this.Goose.currentTask] + ")";
				}
			}
			this.prevSpeed = this.Goose.currentSpeed;
			this.prevTask = this.tasks[this.Goose.currentTask];
			this.Client.SetPresence(new RichPresence
			{
				Details = this.currentTask,
				State = this.speedTier,
				Timestamps = this.Now,

				Assets = new Assets
				{
					LargeImageKey = "icon",
					LargeImageText = "Гусь",
					SmallImageKey = null
				},
				Party = new Party
				{
					ID = "goose",
					Size = 1,
					Max = 2
				},
				Buttons = new Button[]
				{
					new Button()
					{
						Label = "Присоединиться",
						Url = "https://samperson.itch.io/desktop-goose"
					}
				}
			});
		}
	}
}
