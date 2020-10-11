// Copyright © 2017, Nathan Chapman

using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(TriggerEvents))]
public class TriggerEventsEditor : Editor
{
    private GUIStyle guiStyle = new GUIStyle(); //create a new variable

    override public void OnInspectorGUI()
     {
        var triggerEvents = target as TriggerEvents;

        guiStyle.fontSize = 12; //change the font size
        guiStyle.normal.textColor = Color.white; //change the font size

        GUILayout.Space(10);
        GUILayout.Label("Type", guiStyle);
        GUILayout.Space(5);

        triggerEvents.oneTimeActivate = GUILayout.Toggle(triggerEvents.oneTimeActivate, " Activate Only 1 Time");

        GUILayout.Space(20);

        triggerEvents.interactable = GUILayout.Toggle(triggerEvents.interactable, " Is Interactable");
        using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(triggerEvents.interactable)))
        {
            if (group.visible == true)
             {
                EditorGUI.indentLevel++;
                GUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Interaction Text");
                triggerEvents.interactText = EditorGUILayout.TextArea(triggerEvents.interactText);
                GUILayout.EndHorizontal();
                EditorGUI.indentLevel--;
             }

        }
        GUILayout.Space(20);

        triggerEvents.subtitles = GUILayout.Toggle(triggerEvents.subtitles, " Is Subtitles");
        using (var sub = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(triggerEvents.subtitles)))
        {
            if (sub.visible == true)
             {
                EditorGUI.indentLevel++;
                
                GUILayout.BeginHorizontal();
                GUILayout.Space(20); triggerEvents.autoSubtitles = GUILayout.Toggle(triggerEvents.autoSubtitles, " Automatic Start");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20); triggerEvents.buttonProgression = GUILayout.Toggle(triggerEvents.buttonProgression, " Progress With 'Enter'");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20); triggerEvents.pauseWhileSubtitles = GUILayout.Toggle(triggerEvents.pauseWhileSubtitles, " Pause Game While Subtitles");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20); triggerEvents.triggerOnSubs = GUILayout.Toggle(triggerEvents.triggerOnSubs, " Trigger First Actions On Subtitles Start");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(20); triggerEvents.triggerAfterSubs = GUILayout.Toggle(triggerEvents.triggerAfterSubs, " Trigger Second Actions On Subtitles End");
                GUILayout.EndHorizontal();
                
                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.Space(20); GUILayout.Label("Pages", guiStyle);
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                DrawDefaultInspector();
                EditorGUI.indentLevel--;

                GUILayout.Space(20);

                GUILayout.BeginHorizontal();
                GUILayout.Space(20); GUILayout.Label("Cycle Text Speed", guiStyle);
                GUILayout.EndHorizontal();
                
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Space(20); triggerEvents.dynamicDelay = GUILayout.Toggle(triggerEvents.dynamicDelay, " Dynamic Delay");
                GUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                using (var delay = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(triggerEvents.dynamicDelay)))
                {
                    if (delay.visible == false)
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Custom (Sec)");
                        triggerEvents.textDelay = EditorGUILayout.FloatField(triggerEvents.textDelay);
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                    if (delay.visible == true)
                    {
                        EditorGUI.indentLevel++;
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel("Words Per Sec");
                        triggerEvents.wordsPerSecond = EditorGUILayout.IntSlider(triggerEvents.wordsPerSecond, 1, 15);
                        GUILayout.EndHorizontal();
                        EditorGUI.indentLevel--;
                    }
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
             }
        }
     }
}
