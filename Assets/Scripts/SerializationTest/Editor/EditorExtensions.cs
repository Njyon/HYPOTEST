#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SearchableStringPopupWindow : EditorWindow
{
    static GUISkin skin;
    static string[] strings;
    static string[] filteredStrings;
    static string searchString;
    static Vector2 scrollViewPosition;
    static Action<string> selectOption;
    static bool takeFocus;
    static bool open;
    public static void Show(string[] options, Rect lastRect, Action<string> callback)
    {
        strings = options;
        filteredStrings = options;
        searchString = "";
        scrollViewPosition = Vector2.zero;
        selectOption = callback;
        takeFocus = true;
        if (open)
        {
            GetWindow<SearchableStringPopupWindow>().Close();
            open = false;
            return;
        }
        open = true;
        SearchableStringPopupWindow window = CreateInstance<SearchableStringPopupWindow>();
        window.ShowAsDropDown(lastRect, new Vector2(lastRect.width, 300));
        skin = Resources.Load<GUISkin>("SearchableSkin");
    }

    private void OnGUI()
    {
        Color c = GUI.color;
        GUI.color = new Color(0.15f, 0.15f, 0.15f, 1.0f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), skin.box.normal.background);
        GUI.color = new Color(0.22f, 0.22f, 0.22f, 1.0f);
        GUI.DrawTexture(new Rect(2, 2, Screen.width - 4, Screen.height - 4), skin.box.normal.background);
        GUI.color = c;
        GUILayout.Space(8);
        EditorGUIUtility.labelWidth = 50;
        string prevString = searchString;
        GUI.SetNextControlName("searchbox");
        searchString = EditorGUILayout.TextField("Search", searchString);
        if (takeFocus)
        {
            takeFocus = false;
            GUI.FocusControl("searchbox");
        }
        if (prevString != searchString)
        {
            if (searchString == "")
                filteredStrings = strings;
            else
            {
                List<string> tempFilteredStrings = new List<string>();
                for (int i = 0; i < strings.Length; ++i)
                {
                    if (strings[i].ToLower().Contains(searchString.ToLower()))
                        tempFilteredStrings.Add(strings[i]);
                }
                filteredStrings = tempFilteredStrings.ToArray();
            }
        }
        EditorGUIUtility.labelWidth = 0;
        EditorExtensions.HorizontalSeperator();
        scrollViewPosition = EditorGUILayout.BeginScrollView(scrollViewPosition);
        for (int i = 0; i < filteredStrings.Length; ++i)
        {
            if (GUILayout.Button(filteredStrings[i], skin.button))
            {
                selectOption?.Invoke(filteredStrings[i]);
                Close();
            }
        }
        EditorGUILayout.EndScrollView();
        Repaint();
    }

    private void OnDestroy()
    {
        open = false;
    }
}

public static class EditorExtensions
{
    public static void ShowSearchableStringPopup(List<string> options, Rect lastRect, Action<string> callback)
    {
        ShowSearchableStringPopup(options.ToArray(), lastRect, callback);
    }

    public static void ShowSearchableStringPopup(string[] options, Rect lastRect, Action<string> callback)
    {
        SearchableStringPopupWindow.Show(options, lastRect, callback);
    }

    public static void HorizontalSeperator(float height = 1, float space = 4)
    {
        GUILayout.Space(space);
        Rect rect = EditorGUILayout.GetControlRect(false, height);
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        GUILayout.Space(space);
    }

    public static Rect ToAbsolute(this Rect r)
    {
        Rect output = r;
        Vector2 screenPos = GUIUtility.GUIToScreenPoint(new Vector2(r.x, r.y));
        output.x = screenPos.x;
        output.y = screenPos.y;
        return output;
    }
}

public static class EditorGUIExtensions
{
    public static void SearchableDropdown(Rect rect, string currentOption, string[] options, Action<string> callback, params GUILayoutOption[] layoutOptions)
    {
        GUIContent c = new GUIContent(currentOption);
        if (EditorGUI.DropdownButton(rect, c, FocusType.Passive))
        {
            rect.width = Mathf.Max(rect.width, 200);
            EditorExtensions.ShowSearchableStringPopup(options, rect.ToAbsolute(), callback);
        }
    }
}
#endif