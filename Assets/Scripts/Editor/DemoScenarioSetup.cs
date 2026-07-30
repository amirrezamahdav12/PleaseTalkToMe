using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DemoScenarioSetup : EditorWindow
{
    [MenuItem("SwitchPrime/Setup Demo Scenario")]
    private static void Setup()
    {
        string dbPath = "Assets/Packages/SwitchPrime/DialogueDatabase.asset";
        var db = AssetDatabase.LoadAssetAtPath<DialogueDatabase>(dbPath);

        if (db == null)
        {
            Debug.LogError("DialogueDatabase not found at " + dbPath);
            return;
        }

        Undo.RecordObject(db, "Setup Demo Scenario");

        DeleteOldNodes(db);
        ClearDatabase(db);
        CreateFullScenario(db);

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        SetupGameFlow();

        Debug.Log($"Scenario ready! {db.nodes.Count} nodes in database.");
    }

    private static void SetupGameFlow()
    {
        var mangers = GameObject.Find("mangers");
        if (mangers == null) return;

        var flow = mangers.GetComponent<GameFlowManager>();
        if (flow == null)
            flow = mangers.AddComponent<GameFlowManager>();

        var dialogue = mangers.GetComponent<DialogueManager>();
        if (dialogue != null)
        {
            var uiField = typeof(DialogueManager).GetField("dialogueUI",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (uiField?.GetValue(dialogue) == null)
            {
                var dialogueUI = FindObjectOfType<DialogueUI>();
                if (dialogueUI != null)
                    uiField.SetValue(dialogue, dialogueUI);
            }
        }

        Debug.Log("GameFlowManager added to scene.");
    }

    private static void DeleteOldNodes(DialogueDatabase db)
    {
        foreach (var node in db.nodes.ToList())
        {
            if (node != null)
            {
                string path = AssetDatabase.GetAssetPath(node);
                AssetDatabase.DeleteAsset(path);
            }
        }
    }

    private static void ClearDatabase(DialogueDatabase db)
    {
        db.nodes.Clear();
    }

    private static void CreateFullScenario(DialogueDatabase db)
    {
        string dir = Path.GetDirectoryName(AssetDatabase.GetAssetPath(db));

        var nodes = new Dictionary<string, DialogueNode>();

        string[,] nodeDefs = new string[,]
        {
            // nodeID, speaker, dialogue, mood(0-6), isEnding
            {"intro_001",     "Omid", "If no one gives a shit\nin the next 60 seconds...\nI'm done. For real this time.",       "0", "0"},
            {"engagement",    "Omid", "Took you long enough.\nWhat do you want?",                                              "1", "0"},
            {"concern",       "Omid", "Don't give me that 'I care' bullshit.\nYou don't know me.",                             "3", "0"},
            {"venting",       "Omid", "Every day feels the same.\nWake up, pretend, sleep, repeat.\nWhat's the point?",        "5", "0"},
            {"distraction",   "Omid", "Fun?\nYou think I remember\nwhat fun feels like?",                                      "0", "0"},
            {"trust_build",   "Omid", "Sorry.\nI'm not used to people\nactually sticking around.",                             "4", "0"},
            {"self_doubt",    "Omid", "Why are you even still here?\nI'd have left by now.",                                   "6", "0"},
            {"practical",     "Omid", "Therapy? Pills?\nYeah, because that's worked\nso far.",                                 "4", "0"},
            {"comfort",       "Omid", "...I don't know what to say.\nNo one's ever just...\nlistened before.",                  "1", "0"},
            {"good_ending",   "Omid", "Maybe... maybe tonight's\nnot the night.\nThanks for being\na stubborn bastard.",         "1", "1"},
            {"collapse_end",  "Omid", "See?\nEveryone leaves eventually.\nEven the ones who\npretend to care.",                  "6", "1"},
            {"ghosted",       "Omid", "Figures.\nNot even 'goodbye'.\nJust... nothing.",                                        "2", "1"},
        };

        for (int i = 0; i < nodeDefs.GetLength(0); i++)
        {
            var node = ScriptableObject.CreateInstance<DialogueNode>();
            node.name = nodeDefs[i, 0];
            node.nodeID = nodeDefs[i, 0];
            node.speaker = nodeDefs[i, 1];
            node.dialogue = nodeDefs[i, 2];
            node.mood = (OmidMood)int.Parse(nodeDefs[i, 3]);
            node.isEndingNode = int.Parse(nodeDefs[i, 4]) == 1;
            node.nodePosition = new Vector2(100 + i * 30, 100 + i * 90);

            string assetPath = Path.Combine(dir, node.nodeID + ".asset");
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(node, assetPath);

            nodes[node.nodeID] = node;
            db.nodes.Add(node);
        }

        // intro_001
        nodes["intro_001"].notificationText = "1 new viewer joined.";
        AddChoiceW(nodes["intro_001"], "I'm listening.",            "engagement",     5, -5,  5, 15);
        AddChoiceW(nodes["intro_001"], "You don't have to do this.", "concern",        0, 10, 10, 10);
        AddChoiceW(nodes["intro_001"], "...",                        "ghosted",      -15, 15, -15, -20);
        AddChat(nodes["intro_001"], "Ali", "I'm here. You're not alone.", ViewerType.Subscriber, 0.3f);
        AddChat(nodes["intro_001"], "Ali", "I see you, man.", ViewerType.Normal, 1.5f);

        // engagement
        AddChoiceW(nodes["engagement"], "Tell me what's wrong.",      "venting",       10, -10, 10, 20);
        AddChoiceW(nodes["engagement"], "Let's talk about something else.", "distraction", 5, -5,  5, 15);
        AddChat(nodes["engagement"], "Ali", "Rough night?", ViewerType.Normal, 0.5f);

        // concern
        AddChoiceW(nodes["concern"], "I want to know you.",         "trust_build",  5,  5, 15, 20);
        AddChoiceW(nodes["concern"], "Someone has to care.",        "venting",      10, -5, 10, 10);
        AddChat(nodes["concern"], "Ali", "I don't care if I know you. I'm still here.", ViewerType.Normal, 0.5f);

        // venting
        AddChoiceW(nodes["venting"], "You're not alone in this.",   "comfort",      15, -15, 15, 25);
        AddChoiceW(nodes["venting"], "Have you tried getting help?", "practical",   10,   5, 10, 15);
        AddChat(nodes["venting"], "Ali", "I hear you. Keep talking.", ViewerType.Normal, 1f);

        // distraction
        AddChoiceW(nodes["distraction"], "Then let me remind you.",  "comfort",      5, -10,  5, 20);
        AddChoiceW(nodes["distraction"], "I know the feeling.",      "venting",      0,   0,  0,  5);
        AddChat(nodes["distraction"], "Ali", "We don't have to talk about deep stuff.", ViewerType.Normal, 0.5f);

        // trust_build
        AddChoiceW(nodes["trust_build"], "I'm not going anywhere.",  "comfort",      15, -10, 10, 20);
        AddChoiceW(nodes["trust_build"], "Yeah, I'd leave too.",     "self_doubt",  -10,  10, -10, -10);
        AddChat(nodes["trust_build"], "Ali", "I'm still here, aren't I?", ViewerType.Normal, 0.5f);

        // self_doubt
        AddChoiceW(nodes["self_doubt"], "Because you matter.",       "comfort",      20, -10, 20, 30);
        AddChoiceW(nodes["self_doubt"], "Maybe you're right.",       "collapse_end", -15,  15, -15, -30);
        AddChat(nodes["self_doubt"], "Ali", "I'm not leaving. So deal with it.", ViewerType.Normal, 0.5f);

        // practical
        AddChoiceW(nodes["practical"], "I'm glad you're still here.", "good_ending", 15, -10, 15, 30);
        AddChoiceW(nodes["practical"], "Let's stay on this call.",   "comfort",      5,  -5,  5, 20);
        AddChat(nodes["practical"], "Ali", "One step at a time. That's all.", ViewerType.Normal, 1f);

        // comfort
        AddChoiceW(nodes["comfort"], "I'll be here tomorrow too.",   "good_ending",  20, -10, 20, 60);
        AddChoiceW(nodes["comfort"], "One day at a time.",           "good_ending",  10, -5, 10, 30);
        AddChat(nodes["comfort"], "Ali", "I'm proud of you.", ViewerType.Normal, 1f);

        // good_ending
        AddChat(nodes["good_ending"], "Ali", "That's all I wanted to hear.", ViewerType.Normal, 1f);
        AddChat(nodes["good_ending"], "Ali", "I'll be here. Same time tomorrow.", ViewerType.Normal, 2.5f);

        // collapse_end
        AddChat(nodes["collapse_end"], "Ali", "Don't do this. Please.", ViewerType.Normal, 1f);

        // ghosted
        AddChat(nodes["ghosted"], "Ali", "Hey. I'm still here.", ViewerType.Normal, 0.5f);
        AddChat(nodes["ghosted"], "Ali", "Hello??", ViewerType.Normal, 2f);
    }

    private static void AddChoiceW(DialogueNode node, string text, string nextID,
        float hope, float stress, float trust, float time)
    {
        node.choices.Add(new ChoiceData
        {
            text = text,
            nextNodeID = nextID,
            hopeChange = hope,
            stressChange = stress,
            trustChange = trust,
            timeEffect = time,
        });
    }

    private static void AddChat(DialogueNode node, string username, string message,
        ViewerType viewerType, float delay)
    {
        node.chatMessages.Add(new ChatMessageData
        {
            username = username,
            message = message,
            viewerType = viewerType,
            delay = delay,
        });
    }
}
