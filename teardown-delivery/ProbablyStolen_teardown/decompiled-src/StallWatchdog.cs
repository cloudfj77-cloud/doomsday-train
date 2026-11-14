using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StallWatchdog : MonoBehaviour
{
	public const float STALL_SECONDS = 5f;

	private const float REPEAT_SECONDS = 30f;

	private const int CRUMB_COUNT = 24;

	private const int POLL_MS = 250;

	private static StallWatchdog instance;

	private static readonly object gate = new object();

	private static readonly Queue<string> crumbs = new Queue<string>();

	private static long lastFrameTicks;

	private static volatile int lastFrameCount;

	private static volatile bool focused = true;

	private static volatile string sceneName = "";

	private static volatile bool running;

	private static volatile bool stalled;

	private static long stallStartTicks;

	private Thread thread;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void Init()
	{
		if (!(instance != null))
		{
			GameObject obj = new GameObject("StallWatchdog");
			UnityEngine.Object.DontDestroyOnLoad(obj);
			instance = obj.AddComponent<StallWatchdog>();
		}
	}

	public static void Crumb(string what)
	{
		if (string.IsNullOrEmpty(what))
		{
			return;
		}
		string item = DateTime.Now.ToString("HH:mm:ss.fff") + " f" + lastFrameCount + " " + what;
		lock (gate)
		{
			crumbs.Enqueue(item);
			while (crumbs.Count > 24)
			{
				crumbs.Dequeue();
			}
		}
	}

	public static string Short(string text, int max = 60)
	{
		if (string.IsNullOrEmpty(text))
		{
			return "";
		}
		text = text.Replace("\r", " ").Replace("\n", " ");
		if (text.Length > max)
		{
			return text.Substring(0, max) + "...";
		}
		return text;
	}

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		instance = this;
		LogWriter.CachePersistentPath();
		Interlocked.Exchange(ref lastFrameTicks, Stopwatch.GetTimestamp());
		sceneName = SceneManager.GetActiveScene().name;
		SceneManager.sceneLoaded += OnSceneLoaded;
		running = true;
		thread = new Thread(Watch)
		{
			IsBackground = true,
			Name = "StallWatchdog"
		};
		thread.Start();
	}

	private void OnDestroy()
	{
		if (!(instance != this))
		{
			running = false;
			SceneManager.sceneLoaded -= OnSceneLoaded;
		}
	}

	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		sceneName = scene.name;
		Crumb("scene " + scene.name);
	}

	private void Update()
	{
		Interlocked.Exchange(ref lastFrameTicks, Stopwatch.GetTimestamp());
		lastFrameCount = Time.frameCount;
		focused = Application.isFocused;
		if (stalled)
		{
			stalled = false;
			string text = "[StallWatchdog] Main thread resumed after " + ((double)(Stopwatch.GetTimestamp() - Interlocked.Read(ref stallStartTicks)) / (double)Stopwatch.Frequency).ToString("0.0") + " s";
			UnityEngine.Debug.LogWarning(text);
			LogWriter.AppendDirect(text);
		}
	}

	private void Watch()
	{
		double num = 0.0;
		while (running)
		{
			try
			{
				Thread.Sleep(250);
			}
			catch
			{
				break;
			}
			long num2 = Interlocked.Read(ref lastFrameTicks);
			double num3 = (double)(Stopwatch.GetTimestamp() - num2) / (double)Stopwatch.Frequency;
			if (num3 < 5.0)
			{
				num = 0.0;
				continue;
			}
			if (!stalled)
			{
				Interlocked.Exchange(ref stallStartTicks, num2);
				stalled = true;
			}
			else if (num3 - num < 30.0)
			{
				continue;
			}
			num = num3;
			Report(num3);
		}
	}

	private static void Report(double since)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[StallWatchdog] No frame finished for ").Append(since.ToString("0.0")).Append(" s (frame ")
			.Append(lastFrameCount)
			.Append(", scene ")
			.Append(sceneName)
			.Append(", focused ")
			.Append(focused);
		try
		{
			stringBuilder.Append(", managed heap ").Append(GC.GetTotalMemory(forceFullCollection: false) / 1048576).Append(" MB");
		}
		catch
		{
		}
		try
		{
			stringBuilder.Append(", working set ").Append(Process.GetCurrentProcess().WorkingSet64 / 1048576).Append(" MB");
		}
		catch
		{
		}
		stringBuilder.Append("). Last breadcrumbs, oldest first:");
		lock (gate)
		{
			foreach (string crumb in crumbs)
			{
				stringBuilder.Append("\n    ").Append(crumb);
			}
		}
		string text = stringBuilder.ToString();
		LogWriter.AppendDirect(text);
		try
		{
			UnityEngine.Debug.LogWarning(text);
		}
		catch
		{
		}
	}
}
