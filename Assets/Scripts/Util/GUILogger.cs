using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A console to display Unity's debug logs in-game.
/// </summary>
public class GUILogger : MonoBehaviour
{
#if DEBUG
	struct Log
	{
		public string message;
		public string stackTrace;
		public LogType type;
	}

	public static GUILogger Current;

	private const int kMaxLogCount = 200;
	private List<Log> logs = new List<Log>();
	private Vector2 scrollPosition;
	private bool show;
	private bool collapse;
	private bool autoScroll = true;

	public UILabel textOutPut;
	// Visual elements:

	static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>()
	{
		{ LogType.Assert, Color.white },
		{ LogType.Error, Color.red },
		{ LogType.Exception, Color.red },
		{ LogType.Log, Color.white },
		{ LogType.Warning, Color.yellow },
	};

	private const int margin = 5;
	private const int WIDTH = 960;
	private const int HEIGHT = 640;

	private Rect windowRect = new Rect(margin, margin, WIDTH - (margin * 2), HEIGHT - (margin * 2));
	private Rect titleBarRect = new Rect(0, 0, 10000, 20);
	private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
	private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
	private GUIContent showLabel = new GUIContent("Show/Hide", "Show/Hide Console.");
	private GUIContent autoScrollLabel = new GUIContent("Scroll", "Auto Scroll");

	/// <summary>
	/// The hotkey to show and hide the console window.
	/// </summary>
	public KeyCode toggleKey = KeyCode.BackQuote;

	void Awake()
	{
		Current = this;
	}

	void Update ()
	{
		if (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(KeyCode.UpArrow)) {
			show = !show;
		}

		if (Input.touchCount >= 3) {
			show = true;
		}
	}

	void OnEnable ()
	{
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable ()
	{
		Application.logMessageReceived -= HandleLog;
	}

	void OnGUI ()
	{
		if (!show) {
			return;
		}

		AutoResize(WIDTH, HEIGHT);
		windowRect = GUILayout.Window(123456, windowRect, ConsoleWindow, "Console");
	}

	public static void AutoResize(int screenWidth, int screenHeight)
	{
		Vector2 resizeRatio = new Vector2((float)Screen.width / screenWidth, (float)Screen.height / screenHeight);
		GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(resizeRatio.x, resizeRatio.y, 1.0f));
	}

	/// <summary>
	/// A window that displayss the recorded logs.
	/// </summary>
	/// <param name="windowID">Window ID.</param>
	void ConsoleWindow (int windowID)
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition);

		// Iterate through the recorded logs.
		for (int i = 0; i < logs.Count; i++) {
			var log = logs[i];

			// Combine identical messages if collapse option is chosen.
			if (collapse) {
				var messageSameAsPrevious = i > 0 && log.message == logs[i - 1].message;

				if (messageSameAsPrevious) {
					continue;
				}
			}

			GUI.contentColor = logTypeColors[log.type];
			GUILayout.Label(log.message);
		}
		GUILayout.EndScrollView();


		GUI.contentColor = Color.white;

		GUILayout.BeginHorizontal();

		if (GUILayout.Button(clearLabel)) {
			logs.Clear();
		}

		collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));	
		autoScroll = GUILayout.Toggle(autoScroll, autoScrollLabel, GUILayout.ExpandWidth(false));	

		if (GUILayout.Button(showLabel, GUILayout.ExpandWidth(false))) {
			show = !show;
		}

		GUILayout.EndHorizontal();

		// Allow the window to be dragged by its title bar.
		GUI.DragWindow(titleBarRect);
	}

	/// <summary>
	/// Records a log from the log callback.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <param name="stackTrace">Trace of where the message came from.</param>
	/// <param name="type">Type of message (error, exception, warning, assert).</param>
	void HandleLog (string message, string stackTrace, LogType type)
	{
		logs.Add(new Log() {
			message = message,
			stackTrace = stackTrace,
			type = type,
		});

		textOutPut.text = textOutPut.text + message;
		textOutPut.text = textOutPut.text + stackTrace + "\n";

		if (logs.Count > kMaxLogCount) {
			logs.RemoveAt(0);
		}

		if (autoScroll) {
			scrollPosition.y = Mathf.Infinity;	
		}
	}
#endif
}