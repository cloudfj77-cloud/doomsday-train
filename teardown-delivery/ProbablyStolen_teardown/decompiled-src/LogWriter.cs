using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LogWriter : MonoBehaviour
{
	private static string logFilePath;

	private static List<string> logBuffer = new List<string>();

	private static float flushInterval = 10f;

	private float lastFlushTime;

	private static readonly object fileGate = new object();

	private static string persistentPath;

	public static LogWriter Instance { get; private set; }

	public static string LogFilePath => logFilePath;

	public static void CachePersistentPath()
	{
		if (persistentPath == null)
		{
			persistentPath = Application.persistentDataPath;
		}
	}

	public static void AppendDirect(string entry)
	{
		string text = logFilePath;
		if (text == null)
		{
			if (persistentPath == null)
			{
				return;
			}
			text = Path.Combine(persistentPath, "GameLog_watchdog.txt");
		}
		try
		{
			lock (fileGate)
			{
				File.AppendAllText(text, $"[{DateTime.Now:HH:mm:ss}] {entry}{Environment.NewLine}");
			}
		}
		catch
		{
		}
	}

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		CachePersistentPath();
		string path = "GameLog_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
		logFilePath = Path.Combine(persistentPath, path);
		Application.logMessageReceived += HandleLog;
		DeleteOldLogs(Application.persistentDataPath, 10);
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= HandleLog;
		FlushLogsToDisk();
	}

	private void Update()
	{
		if (Time.unscaledTime - lastFlushTime >= flushInterval)
		{
			FlushLogsToDisk();
			lastFlushTime = Time.unscaledTime;
		}
	}

	private void HandleLog(string logString, string stackTrace, LogType type)
	{
		string text = $"[{DateTime.Now:HH:mm:ss}] [{type}] {logString}";
		if (type == LogType.Error || type == LogType.Exception)
		{
			text = text + "\n" + stackTrace;
		}
		logBuffer.Add(text);
	}

	private void DeleteOldLogs(string directory, int maxLogs)
	{
		string[] files = System.IO.Directory.GetFiles(directory, "GameLog_*.txt");
		if (files.Length <= maxLogs)
		{
			return;
		}
		Array.Sort(files);
		for (int i = 0; i < files.Length - maxLogs; i++)
		{
			try
			{
				File.Delete(files[i]);
			}
			catch (Exception ex)
			{
				Debug.LogError("Failed to delete old log: " + ex.Message);
			}
		}
	}

	private void FlushLogsToDisk()
	{
		if (logBuffer.Count == 0)
		{
			return;
		}
		try
		{
			lock (fileGate)
			{
				File.AppendAllLines(logFilePath, logBuffer);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to write log buffer: " + ex.Message);
		}
		logBuffer.Clear();
	}
}
