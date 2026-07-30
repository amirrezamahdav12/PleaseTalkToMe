using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DialogueNodeEditorWindow : EditorWindow
{
    private DialogueDatabase database;
    private List<EditorNode> editorNodes = new();
    private bool isDirty;

    private Vector2 scrollPosition;
    private const float NodeWidth = 280f;

    private GUIStyle nodeHeaderStyle;
    private GUIStyle nodeFooterStyle;
    private GUIStyle portStyle;
    private GUIStyle choicePortStyle;
    private GUIStyle smallTextStyle;
    private GUIStyle titleTextStyle;

    private EditorNode selectedNode;
    private int draggingChoiceIndex = -1;
    private bool isDraggingConnection;

    private Rect canvasRect;

    [MenuItem("SwitchPrime/Dialogue Node Editor")]
    public static void OpenWindow()
    {
        var window = GetWindow<DialogueNodeEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnEnable()
    {
        wantsMouseMove = true;
        wantsMouseEnterLeaveWindow = true;
        LoadDatabaseFromSelection();
    }

    private void OnDisable()
    {
        if (isDirty && database != null)
            Save();
    }

    private void OnSelectionChange()
    {
        LoadDatabaseFromSelection();
        Repaint();
    }

    private void LoadDatabaseFromSelection()
    {
        if (Selection.activeObject is DialogueDatabase db)
        {
            SetDatabase(db);
        }
    }

    public void SetDatabase(DialogueDatabase db)
    {
        database = db;
        LoadNodes();
        isDirty = false;
        Repaint();
    }

    private void LoadNodes()
    {
        editorNodes.Clear();

        if (database == null) return;

        foreach (var node in database.nodes)
        {
            if (node == null) continue;
            editorNodes.Add(new EditorNode(node, NodeWidth));
        }

        UpdateConnectionCache();
    }

    private void UpdateConnectionCache()
    {
        foreach (var editorNode in editorNodes)
        {
            editorNode.connections.Clear();

            for (int i = 0; i < editorNode.node.choices.Count; i++)
            {
                string targetID = editorNode.node.choices[i].nextNodeID;
                if (string.IsNullOrEmpty(targetID)) continue;

                var target = editorNodes.FirstOrDefault(n => n.node.nodeID == targetID);
                if (target != null)
                {
                    editorNode.connections.Add(new EditorConnection
                    {
                        sourceNode = editorNode,
                        sourceChoiceIndex = i,
                        targetNode = target
                    });
                }
            }
        }
    }

    private void OnGUI()
    {
        InitStyles();

        DrawToolbar();

        if (database == null)
        {
            EditorGUILayout.HelpBox("Select a DialogueDatabase asset to begin.", MessageType.Info);
            return;
        }

        canvasRect = new Rect(0, EditorGUIUtility.singleLineHeight + 4f, position.width, position.height - EditorGUIUtility.singleLineHeight - 24f);

        BeginWindows();

        scrollPosition = GUI.BeginScrollView(
            new Rect(0, EditorGUIUtility.singleLineHeight + 4f, position.width, position.height - EditorGUIUtility.singleLineHeight - 24f),
            scrollPosition,
            new Rect(-5000, -5000, 10000, 10000));

        DrawGrid(20, 0.12f, Color.gray);
        DrawGrid(100, 0.24f, Color.gray);

        DrawConnections();

        ProcessCanvasEvents();

        for (int i = 0; i < editorNodes.Count; i++)
        {
            var editorNode = editorNodes[i];
            var nodeRect = editorNode.rect;

            GUI.color = editorNode == selectedNode ? new Color(0.6f, 0.7f, 1f) : Color.white;
            editorNode.rect = GUI.Window(i, nodeRect, DrawNodeWindow, GUIContent.none, nodeHeaderStyle);
            GUI.color = Color.white;
        }

        if (isDraggingConnection && draggingChoiceIndex >= 0 && selectedNode != null)
        {
            var portRect = GetChoicePortRect(selectedNode, draggingChoiceIndex);
            Vector2 start = new Vector2(portRect.xMax, portRect.center.y);
            Vector2 end = Event.current.mousePosition - scrollPosition + new Vector2(5000, 5000);
            DrawBezier(start, end, Color.white * 0.7f);
            Repaint();
        }

        GUI.EndScrollView();

        EndWindows();

        DrawFooter();

        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
            Repaint();
    }

    private void InitStyles()
    {
        if (nodeHeaderStyle != null) return;

        nodeHeaderStyle = new GUIStyle(GUI.skin.window)
        {
            padding = new RectOffset(6, 6, 6, 6),
            fontSize = 11
        };

        nodeFooterStyle = new GUIStyle(nodeHeaderStyle);

        portStyle = new GUIStyle
        {
            normal = { background = MakeTex(12, 12, new Color(0.3f, 0.7f, 1f)) },
            border = new RectOffset(4, 4, 4, 4),
            alignment = TextAnchor.MiddleCenter
        };

        choicePortStyle = new GUIStyle(portStyle);
        choicePortStyle.normal.background = MakeTex(10, 10, Color.white);

        smallTextStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            fontSize = 9,
            wordWrap = true
        };

        titleTextStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 12
        };
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("New Node", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            CreateNewNode();
        }

        if (GUILayout.Button("Auto Layout", EditorStyles.toolbarButton, GUILayout.Width(80)))
        {
            AutoLayout();
        }

        GUILayout.Space(10);

        if (isDirty)
        {
            GUI.color = new Color(1f, 0.8f, 0.3f);
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
                Save();
            GUI.color = Color.white;
        }
        else
        {
            GUI.enabled = database != null;
            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(50)))
                Save();
            GUI.enabled = true;
        }

        GUILayout.FlexibleSpace();

        if (database != null)
        {
            GUILayout.Label($"Database: {database.name}", EditorStyles.miniLabel);
        }

        GUILayout.EndHorizontal();
    }

    private void DrawFooter()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label($"{editorNodes.Count} nodes | Drag from a choice ○ to connect", EditorStyles.miniLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color color)
    {
        int widthDivisions = Mathf.RoundToInt(10000 / gridSpacing);
        int heightDivisions = Mathf.RoundToInt(10000 / gridSpacing);

        Handles.color = new Color(color.r, color.g, color.b, gridOpacity);

        for (int i = 0; i <= widthDivisions; i++)
        {
            float x = -5000 + i * gridSpacing;
            Handles.DrawLine(new Vector3(x, -5000, 0), new Vector3(x, 5000, 0));
        }

        for (int i = 0; i <= heightDivisions; i++)
        {
            float y = -5000 + i * gridSpacing;
            Handles.DrawLine(new Vector3(-5000, y, 0), new Vector3(5000, y, 0));
        }
    }

    private void DrawConnections()
    {
        foreach (var editorNode in editorNodes)
        {
            foreach (var conn in editorNode.connections)
            {
                var portRect = GetChoicePortRect(conn.sourceNode, conn.sourceChoiceIndex);
                Vector2 start = new Vector2(portRect.xMax, portRect.center.y);

                Vector2 end = new Vector2(conn.targetNode.rect.xMin, conn.targetNode.rect.center.y);

                Color lineColor = Color.Lerp(Color.white, Color.cyan, 0.3f);
                DrawBezier(start, end, lineColor);

                Vector2 mid = Vector2.Lerp(start, end, 0.5f);
                GUI.Label(new Rect(mid.x - 40, mid.y - 8, 80, 16), conn.targetNode.node.nodeID, new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white * 0.7f },
                    fontSize = 9
                });
            }
        }
    }

    private void DrawBezier(Vector2 start, Vector2 end, Color color)
    {
        float distance = Mathf.Abs(end.x - start.x);
        float tangent = Mathf.Max(50, distance * 0.5f);

        Vector2 startTan = new Vector2(start.x + tangent, start.y);
        Vector2 endTan = new Vector2(end.x - tangent, end.y);

        Handles.DrawBezier(start, end, startTan, endTan, color, null, 2.5f);
    }

    private void DrawNodeWindow(int id)
    {
        var editorNode = editorNodes[id];
        var node = editorNode.node;

        Rect headerRect = new Rect(0, 0, editorNode.rect.width, 28);
        GUI.Box(headerRect, "", new GUIStyle { normal = { background = MakeTex(1, 1, new Color(0.2f, 0.2f, 0.25f)) } });

        GUI.Label(new Rect(8, 4, editorNode.rect.width - 50, 20), node.nodeID, titleTextStyle);

        if (GUI.Button(new Rect(editorNode.rect.width - 28, 3, 22, 22), "X", EditorStyles.toolbarButton))
        {
            if (EditorUtility.DisplayDialog("Delete Node", $"Delete '{node.nodeID}'?", "Delete", "Cancel"))
            {
                DeleteNode(editorNode);
                return;
            }
        }

        float y = 30;

        Rect speakerRect = new Rect(6, y, 130, 18);
        GUI.Label(speakerRect, "Speaker:", EditorStyles.miniLabel);
        EditorGUI.BeginChangeCheck();
        string newSpeaker = EditorGUI.TextField(new Rect(70, y, editorNode.rect.width - 76, 18), node.speaker);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(node, "Change Speaker");
            node.speaker = newSpeaker;
            isDirty = true;
        }
        y += 22;

        Rect moodRect = new Rect(6, y, 50, 18);
        GUI.Label(moodRect, "Mood:", EditorStyles.miniLabel);
        EditorGUI.BeginChangeCheck();
        OmidMood newMood = (OmidMood)EditorGUI.EnumPopup(new Rect(52, y, editorNode.rect.width - 58, 18), node.mood);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(node, "Change Mood");
            node.mood = newMood;
            isDirty = true;
        }
        y += 24;

        string preview = node.dialogue;
        if (preview.Length > 80) preview = preview[..77] + "...";
        GUI.Label(new Rect(6, y, editorNode.rect.width - 12, 32), preview, smallTextStyle);
        if (Event.current.type == EventType.MouseDown && new Rect(6, y, editorNode.rect.width - 12, 32).Contains(Event.current.mousePosition))
        {
            Selection.activeObject = node;
        }
        y += 34;

        if (node.isEndingNode && !string.IsNullOrEmpty(node.endingMessage))
        {
            string endingPreview = node.endingMessage;
            if (endingPreview.Length > 60) endingPreview = endingPreview[..57] + "...";
            GUI.Label(new Rect(6, y, editorNode.rect.width - 12, 20), "End: " + endingPreview, new GUIStyle(EditorStyles.miniLabel)
            { normal = { textColor = Color.yellow }, fontSize = 9 });
            y += 20;
        }

        EditorGUI.BeginChangeCheck();
        bool isEnding = GUI.Toggle(new Rect(6, y, 80, 18), node.isEndingNode, "Ending");
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(node, "Toggle Ending");
            node.isEndingNode = isEnding;
            isDirty = true;
        }

        EditorGUI.BeginChangeCheck();
        string notif = EditorGUI.TextField(new Rect(90, y, editorNode.rect.width - 96, 18), node.notificationText);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(node, "Change Notification");
            node.notificationText = notif;
            isDirty = true;
        }
        y += 22;

        string chatLabel = node.chatMessages.Count == 0 ? "No chat messages" : $"{node.chatMessages.Count} chat message(s)";
        GUI.Label(new Rect(6, y, editorNode.rect.width - 12, 16), chatLabel, new GUIStyle(EditorStyles.miniLabel)
        { normal = { textColor = new Color(0.6f, 0.6f, 0.8f) } });
        y += 20;

        GUI.Box(new Rect(2, y - 2, editorNode.rect.width - 4, 2), "");

        y += 6;

        GUI.Label(new Rect(6, y, editorNode.rect.width - 12, 16), "Choices:", EditorStyles.boldLabel);
        y += 18;

        for (int i = 0; i < node.choices.Count; i++)
        {
            var choice = node.choices[i];
            float choiceBlockHeight = 44f;
            Rect choiceRect = new Rect(4, y, editorNode.rect.width - 8, choiceBlockHeight);

            GUI.Box(choiceRect, "", new GUIStyle { normal = { background = MakeTex(1, 1, new Color(0.18f, 0.18f, 0.2f)) } });

            string choicePreview = choice.text;
            if (choicePreview.Length > 40) choicePreview = choicePreview[..37] + "...";
            GUI.Label(new Rect(22, y + 2, choiceRect.width - 28, 18), $"{i + 1}. {choicePreview}", EditorStyles.miniLabel);

            Rect portRect = new Rect(4, y + 2, 14, 14);
            GUI.Box(portRect, "○", new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.cyan }
            });

            if (!string.IsNullOrEmpty(choice.nextNodeID))
            {
                GUI.Label(new Rect(22, y + 18, choiceRect.width - 28, 16), $"→ {choice.nextNodeID}", new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = new Color(0.4f, 0.7f, 1f) },
                    fontSize = 9
                });
            }

            if (GUI.Button(new Rect(choiceRect.width - 20, y + 2, 16, 16), "×", EditorStyles.toolbarButton))
            {
                Undo.RecordObject(node, "Remove Choice");
                node.choices.RemoveAt(i);
                isDirty = true;
                UpdateConnectionCache();
                return;
            }

            y += choiceBlockHeight + 2;
        }

        if (GUI.Button(new Rect(6, y, editorNode.rect.width - 12, 20), "+ Add Choice", EditorStyles.miniButton))
        {
            Undo.RecordObject(node, "Add Choice");
            node.choices.Add(new ChoiceData { text = "New choice..." });
            isDirty = true;
            UpdateConnectionCache();
            return;
        }
        y += 26;

        GUI.DragWindow(new Rect(0, 0, editorNode.rect.width, 28));

        editorNode.rect.height = y + 4;
    }

    private Rect GetChoicePortRect(EditorNode editorNode, int choiceIndex)
    {
        var node = editorNode.node;
        float y = 30;

        y += 22;
        y += 24;
        y += 34;

        if (node.isEndingNode && !string.IsNullOrEmpty(node.endingMessage))
            y += 20;

        y += 22;
        y += 20;
        y += 8;
        y += 18;

        for (int i = 0; i < choiceIndex; i++)
        {
            y += 46;
        }

        return new Rect(editorNode.rect.x + 4, editorNode.rect.y + y + 2, 14, 14);
    }

    private void ProcessCanvasEvents()
    {
        if (Event.current.type == EventType.MouseDown && canvasRect.Contains(Event.current.mousePosition))
        {
            var mousePos = Event.current.mousePosition - new Vector2(canvasRect.x, canvasRect.y) + scrollPosition - new Vector2(5000, 5000);

            foreach (var editorNode in editorNodes)
            {
                if (!editorNode.rect.Contains(mousePos)) continue;

                for (int i = 0; i < editorNode.node.choices.Count; i++)
                {
                    var portRect = GetChoicePortRect(editorNode, i);

                    if (portRect.Contains(mousePos))
                    {
                        selectedNode = editorNode;
                        draggingChoiceIndex = i;
                        isDraggingConnection = true;
                        Event.current.Use();
                        return;
                    }
                }
            }

            selectedNode = null;

            foreach (var editorNode in editorNodes)
            {
                var headerRect = new Rect(editorNode.rect.x, editorNode.rect.y, editorNode.rect.width, 28);
                if (headerRect.Contains(mousePos))
                {
                    selectedNode = editorNode;
                    Selection.activeObject = editorNode.node;
                    Event.current.Use();
                    return;
                }

                if (editorNode.rect.Contains(mousePos))
                {
                    selectedNode = editorNode;
                    Selection.activeObject = editorNode.node;
                    Event.current.Use();
                    return;
                }
            }

            isDraggingConnection = false;
            draggingChoiceIndex = -1;
        }

        if (Event.current.type == EventType.MouseUp && isDraggingConnection)
        {
            var mousePos = Event.current.mousePosition - new Vector2(canvasRect.x, canvasRect.y) + scrollPosition - new Vector2(5000, 5000);

            foreach (var editorNode in editorNodes)
            {
                if (editorNode == selectedNode) continue;

                if (editorNode.rect.Contains(mousePos))
                {
                    var choice = selectedNode.node.choices[draggingChoiceIndex];
                    Undo.RecordObject(selectedNode.node, "Connect Choice");
                    choice.nextNodeID = editorNode.node.nodeID;
                    isDirty = true;
                    UpdateConnectionCache();
                    break;
                }
            }

            isDraggingConnection = false;
            draggingChoiceIndex = -1;
            Event.current.Use();
        }

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && canvasRect.Contains(Event.current.mousePosition))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Create Node"), false, () => CreateNewNode());
            menu.AddSeparator("");

            if (selectedNode != null)
            {
                menu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(selectedNode));
                menu.AddItem(new GUIContent("Duplicate Node"), false, () => DuplicateNode(selectedNode));
            }

            menu.ShowAsContext();
            Event.current.Use();
        }


    }

    private void CreateNewNode()
    {
        if (database == null) return;

        string baseName = "NewNode";
        int counter = 1;
        while (database.nodes.Any(n => n.nodeID == baseName + counter))
            counter++;
        string nodeID = baseName + counter;

        var newNode = CreateInstance<DialogueNode>();
        newNode.nodeID = nodeID;
        newNode.name = nodeID;
        newNode.speaker = "Omid";

        string path = AssetDatabase.GetAssetPath(database);
        string dir = System.IO.Path.GetDirectoryName(path);
        string assetPath = System.IO.Path.Combine(dir, $"{nodeID}.asset");
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(newNode, assetPath);

        Undo.RecordObject(database, "Add Node");
        database.nodes.Add(newNode);
        isDirty = true;

        var editorNode = new EditorNode(newNode, NodeWidth);
        editorNode.rect.position = GetViewportCenter() + new Vector2(Random.Range(-50, 50), Random.Range(-50, 50));
        newNode.nodePosition = editorNode.rect.position;
        editorNodes.Add(editorNode);

        selectedNode = editorNode;
        Selection.activeObject = newNode;
        Repaint();
    }

    private void DuplicateNode(EditorNode source)
    {
        Undo.RecordObject(database, "Duplicate Node");

        var newNode = Instantiate(source.node);
        newNode.nodeID = source.node.nodeID + "_copy";
        newNode.name = newNode.nodeID;

        string path = AssetDatabase.GetAssetPath(database);
        string dir = System.IO.Path.GetDirectoryName(path);
        string assetPath = System.IO.Path.Combine(dir, $"{newNode.nodeID}.asset");
        assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);

        AssetDatabase.CreateAsset(newNode, assetPath);

        database.nodes.Add(newNode);
        isDirty = true;

        var editorNode = new EditorNode(newNode, NodeWidth);
        editorNode.rect.position = source.rect.position + new Vector2(30, 30);
        newNode.nodePosition = editorNode.rect.position;
        editorNodes.Add(editorNode);

        selectedNode = editorNode;
        Selection.activeObject = newNode;
        Repaint();
    }

    private void DeleteNode(EditorNode editorNode)
    {
        Undo.RecordObject(database, "Delete Node");

        foreach (var other in database.nodes)
        {
            if (other == null) continue;
            foreach (var choice in other.choices)
            {
                if (choice.nextNodeID == editorNode.node.nodeID)
                {
                    Undo.RecordObject(other, "Clear Connection");
                    choice.nextNodeID = null;
                }
            }
        }

        database.nodes.Remove(editorNode.node);
        editorNodes.Remove(editorNode);

        string path = AssetDatabase.GetAssetPath(editorNode.node);
        AssetDatabase.DeleteAsset(path);

        if (selectedNode == editorNode)
            selectedNode = null;

        isDirty = true;
        UpdateConnectionCache();
        Repaint();
    }

    private void Save()
    {
        if (database == null) return;

        foreach (var editorNode in editorNodes)
        {
            var node = editorNode.node;
            node.nodePosition = editorNode.rect.position;
            EditorUtility.SetDirty(node);
        }

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();
        isDirty = false;
        Repaint();
    }

    private void AutoLayout()
    {
        if (editorNodes.Count == 0) return;

        var roots = editorNodes.Where(n =>
            !editorNodes.Any(other => other.connections.Any(c => c.targetNode == n))
        ).ToList();

        if (roots.Count == 0)
            roots.Add(editorNodes[0]);

        HashSet<EditorNode> visited = new HashSet<EditorNode>();
        float ySpacing = 180f;
        float xSpacing = 320f;

        void LayoutNode(EditorNode node, int depth, ref float xPos)
        {
            if (visited.Contains(node)) return;
            visited.Add(node);

            node.rect.position = new Vector2(xPos + depth * xSpacing, ySpacing * visited.Count * 0.5f);

            var targets = node.connections.Select(c => c.targetNode).ToList();
            for (int i = 0; i < targets.Count; i++)
            {
                float childX = xPos;
                LayoutNode(targets[i], depth + 1, ref childX);
            }

            if (targets.Count > 1)
                xPos += xSpacing * 0.5f;
        }

        float startX = 0;
        foreach (var root in roots)
            LayoutNode(root, 0, ref startX);

        isDirty = true;
        Repaint();
    }

    private Vector2 GetViewportCenter()
    {
        return new Vector2(position.width * 0.5f, position.height * 0.5f) - scrollPosition + new Vector2(5000, 5000);
    }

    private Texture2D MakeTex(int w, int h, Color c)
    {
        var pixels = new Color[w * h];
        System.Array.Fill(pixels, c);
        var tex = new Texture2D(w, h);
        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    private class EditorNode
    {
        public DialogueNode node;
        public Rect rect;
        public List<EditorConnection> connections = new();

        public EditorNode(DialogueNode node, float width)
        {
            this.node = node;
            rect = new Rect(node.nodePosition, new Vector2(width, 120));
        }
    }

    private class EditorConnection
    {
        public EditorNode sourceNode;
        public int sourceChoiceIndex;
        public EditorNode targetNode;
    }
}
