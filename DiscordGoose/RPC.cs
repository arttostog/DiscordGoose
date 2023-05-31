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

		private string getRand(string[] s)
		{
			return s[this.rand.Next(s.Length)];
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
				this.speedTier = ((this.Goose.currentSpeed == walkSpeed) ? (this.getRand(this.Walk) + " (Walking)") : ((this.Goose.currentSpeed == runSpeed) ? (this.getRand(this.Run) + " (Running)") : ((this.Goose.currentSpeed == chargeSpeed) ? (this.getRand(this.Charge) + " (Charging)") : string.Format("{0} (Speed: {1})", this.getRand(this.Custom), this.Goose.currentSpeed))));
				foreach (FieldInfo fieldInfo in typeof(RPC.Taskz).GetFields())
				{
					if (fieldInfo.Name.Replace("ing", "").Replace("bb", "b").Replace("mes", "me").Replace("pads", "pad") == this.tasks[this.Goose.currentTask] || fieldInfo.Name == this.tasks[this.Goose.currentTask])
					{
						this.currentTask = this.getRand((string[])fieldInfo.GetValue(RPC.t)) + " (" + fieldInfo.Name + ")";
						break;
					}
					this.currentTask = this.getRand(this.Custom) + " (" + this.tasks[this.Goose.currentTask] + ")";
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
					Size = playersCount,
					Max = int.MaxValue
				},
			});
		}

		private Timestamps Now;

		private DiscordRpcClient Client;

		private GooseEntity Goose;

		private string[] tasks;

		private string prevTask;

		private float prevSpeed;

		private string speedTier;

		private string currentTask;

		private bool isRunning;

		public static RPC.Taskz t = new RPC.Taskz();

		private Random rand = new Random();

		private static int playersCount = 1;

		private string[] Custom = new string[]
		{
				"?"
		};

		public string[] Run = new string[]
		{
				"Бегу.",
				"Убегаю от хорошего выбора.",
				"Пытаюсь похудеть.",
				"Стоп, почему я бегу?"
		};

		public string[] Walk = new string[]
		{
				"Бегаю в медленном темпе.",
				"Делаю обратную лунную походку.",
				"Топ-топ-топ-топ!",
				"Вышел на прогулку.",
				"Почему мои ноги такие громкие?",
				"Изучаю монитор.",
				"Громко шагаю."
		};

		public string[] Charge = new string[]
		{
				"Бегу в невероятно быстром темпе.",
				"Превышаю скоростной режим.",
				"Скорость. Я — скорость.",
				"Злюсь."
		};

		public class Taskz
		{
			public string[] Wandering = new string[]
			{
				"Обдумываю жизненный выбор.",
				"Размышляю о мемах.",
				"Думаю о том, чтобы вызвать ещё больший хаос.",
				"Ищу больше способов создать проблемы.",
				"Что-то ищу..."
			};

			public string[] NabbingMouse = new string[]
			{
				"Кусаю мышь.",
				"Хрум-хрум.",
				"Выясняю, является ли мышь мышью.",
				"Гоняюсь за мышью."
			};

			public string[] CollectingMemes = new string[]
			{
				"Перетаскиваю мемы.",
				"Отправляю мемы.",
				"Кидаю несколько гусиных-мемов.",
				"Мм, да. Гоготание сделанно из гудка."
			};

			public string[] CollectingNotepads = new string[]
			{
				"Перетаскиваю заметку.",
				"Становлюсь удивительным поэтом.",
				"Даю очень важные жизненные советы.",
				"Печатаю двумя ногами.",
				"Тормошу пишущую машинку.",
				"Печатаю с закрытыми глазами."
			};

			public string[] TrackingMud = new string[]
			{
				"Делаю экран грязнее.",
				"Я это почищу, наверное.",
				"Теперь это мой монитор.",
				"Удобрения для экрана."
			};

			public string[] CustomMouseNab = new string[]
			{
				"ONE PUNCHH!!",
				"Погоня за мышью на невероятно быстрой скорости.",
				"Why do I hear boss music?"
			};

			public string[] Sleeping = new string[]
			{
				"Zzzzzz...",
				"Мечтаю получать бесконечные колокольчики.",
				"Вызываю хаос во сне.",
				"О, удобненько.",
				"Сплю.",
				"Расслабляюсь."
			};

			public string[] RunToBed = new string[]
			{
				"Иду спать.",
				"Нашёл удобную кровать.",
				"Собираюсь вздремнуть.",
				"Думаю о сне.",
				"Иду к комфортной зоне."
			};

			public string[] ChargeToStick = new string[]
			{
				"О! Палка!",
				"Бегу за палкой.",
				"ДАЙТЕ! ДАЙТЕ! ДАЙТЕ!",
				"Обнаруженно движение палки!"
			};

			public string[] ReturnStick = new string[]
			{
				"Возвращаю палку!",
				"Палка успешно захвачена!"
			};

			public string[] ChaseLaser = new string[]
			{
				"Лазер обнаружен!",
				"Бесконечно гоняюсь за красной точкой."
			};

			public string[] ChargeToBall = new string[]
			{
				"Играю с мячиком.",
				"Пытаюсь укусить красный шар.",
				"Пытаюсь стать великим футболистом."
			};
		}
	}
}
