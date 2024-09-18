using System;
using UnityEngine;
using UnityEditor;
using System.Reflection;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;

#else
using UnityEngine.Experimental.UIElements;
#endif

[InitializeOnLoad]
public class ToolbarExtend {
	private static Type m_toolbarType = typeof(Editor).Assembly.GetType("UnityEditor.Toolbar");
	private static Type m_guiViewType = typeof(Editor).Assembly.GetType("UnityEditor.GUIView");

#if UNITY_2020_1_OR_NEWER
	private static Type m_iWindowBackendType = typeof(Editor).Assembly.GetType("UnityEditor.IWindowBackend");

	private static PropertyInfo m_windowBackend = m_guiViewType.GetProperty("windowBackend",
		BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

	private static PropertyInfo m_viewVisualTree = m_iWindowBackendType.GetProperty("visualTree",
		BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#else
	private static PropertyInfo m_viewVisualTree = m_guiViewType.GetProperty("visualTree",
        BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
#endif
	private static FieldInfo m_imguiContainerOnGui = typeof(IMGUIContainer).GetField("m_OnGUIHandler",
		BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

	private static ScriptableObject m_currentToolbar;
	private static GUIStyle m_commonBtnStyle;

	private static GUIContent m_restartBtnContent;

	static ToolbarExtend() {
		EditorApplication.update -= OnUpdate;
		EditorApplication.update += OnUpdate;

		m_restartBtnContent = EditorGUIUtility.IconContent("Refresh");
	}

	private static void OnUpdate() {
		// Relying on the fact that toolbar is ScriptableObject and gets deleted when layout changes
		if (m_currentToolbar == null) {
			// Find toolbar
			var toolbars = Resources.FindObjectsOfTypeAll(m_toolbarType);
			m_currentToolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
			if (m_currentToolbar != null) {
#if UNITY_2021_1_OR_NEWER
				FieldInfo root = m_currentToolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
				VisualElement rawRoot = root.GetValue(m_currentToolbar) as VisualElement;
				VisualElement toolbarZone = rawRoot.Q("ToolbarZoneRightAlign");
				VisualElement parent = new VisualElement() {
					style = {
						flexGrow = 1,
						flexDirection = FlexDirection.Row,
					}
				};
				IMGUIContainer container = new IMGUIContainer();
				container.onGUIHandler += OnGUI;
				parent.Add(container);
				toolbarZone.Add(parent);

#else
	#if UNITY_2020_1_OR_NEWER
				var windowBackend = m_windowBackend.GetValue(m_currentToolbar);

				// Get it's visual tree
				var visualTree = (VisualElement) m_viewVisualTree.GetValue(windowBackend, null);
	#else
                // Get it's visual tree
                var visualTree = (VisualElement)m_viewVisualTree.GetValue(m_currentToolbar, null);
	#endif
                // Get first child which 'happens' to be toolbar IMGUIContainer
                var container = (IMGUIContainer)visualTree[0];

                // (Re)attach handler
                var handler = (Action)m_imguiContainerOnGui.GetValue(container);
                handler -= OnGUI;
                handler += OnGUI;
                m_imguiContainerOnGui.SetValue(container, handler);
#endif
			}
		}
	}

	private static void CompileAndRestart() {
		// 编译代码
		AssetDatabase.Refresh();

		// 启动播放模式
		EditorApplication.isPlaying = true;
	}

	private static void OnGUI() {
		if (PlayerPrefs.GetInt("ToolbarActiveButton", 1) == 0) {
			return;
		}
 
#if UNITY_2021_1_OR_NEWER
		GUILayout.BeginHorizontal();
		if (GUILayout.Button(new GUIContent(EditorGUIUtility.FindTexture("Refresh")))) {
			if (EditorApplication.isPlaying) {
				// LuaModule.Instance.RestartGameForEditor();
				// EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
				// EditorApplication.isPlaying = false; // 停止播放
				Application.Quit();
				CompileAndRestart();
			}

			else {
				AssetDatabase.Refresh();
				EditorApplication.isPlaying = true;
			}
		}

		GUILayout.EndHorizontal();

#else
	    if (m_commonBtnStyle == null) {
			m_commonBtnStyle = new GUIStyle("Command");
		}

	    var screenWidth = EditorGUIUtility.currentViewWidth;
	    float playButtonsPosition = Mathf.RoundToInt ((screenWidth - 100) / 2);
	    Rect rightRect = new Rect(0, 0, screenWidth, Screen.height);
	    rightRect.xMin = playButtonsPosition;
	    rightRect.xMin += 100;
	    rightRect.xMax = screenWidth;
	    rightRect.yMin = 5;

	    GUILayout.BeginArea(rightRect);
	    GUILayout.BeginHorizontal();
	    if (GUILayout.Button(m_restartBtnContent, m_commonBtnStyle))
	    {
		    if (EditorApplication.isPlaying) {
			    // LuaModule.Instance.RestartGameForEditor();

			    
		    }
	    }
	    GUILayout.EndHorizontal();
	    GUILayout.EndArea();
#endif
	}
}