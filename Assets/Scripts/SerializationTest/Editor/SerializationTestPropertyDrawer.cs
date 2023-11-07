using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ClassInstance<>))]
public class ClassInstancePropertyDrawer : PropertyDrawer
{
    private static Dictionary<string, TextAsset> typeToAssets = new Dictionary<string, TextAsset>();
    
    private Type[] types = null;
    private string[] options = null;
    private TextAsset textAsset;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUIContent c = new GUIContent(label);
        string typeName = property.FindPropertyRelative("instance").managedReferenceFieldTypename.Split(' ')[1];
        
        SerializedProperty p = property;
        string pattern = "<([^>]+)>";
        MatchCollection matches = Regex.Matches(p.FindPropertyRelative("instance").type, pattern);
        string l = matches.Count > 0 ? matches[0].Groups[1].Value : "Set Class";

        if (!typeToAssets.ContainsKey(l))
        {
            string[] assetGuids = AssetDatabase.FindAssets("t:Script " + l);
            if (assetGuids.Length > 0)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[0]);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);
                textAsset = script;
                typeToAssets.Add(l, script);
            }
            else
            {
                typeToAssets.Add(l, null);
            }
        }
        else
        {
            textAsset = typeToAssets[l];
        }

        GatherSubTypes(typeName);
        position.x += EditorGUIUtility.labelWidth;
        position.width -= EditorGUIUtility.labelWidth;
        UnityEngine.Object o = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width / 2.0f - 2, EditorGUIUtility.singleLineHeight), "", textAsset, typeof(TextAsset), true);
        if (o != textAsset)
        {
            TextAsset asset = (TextAsset)o;
            if (asset != null)
            {
                Type baseType = FindTypeByName(typeName);
                var classTypes = GetClassesInScript(asset);
                foreach (var type in classTypes)
                {
                    if (!baseType.IsAssignableFrom(type)) continue;
                    if (type.IsAbstract || type.IsInterface) continue;
                    property.FindPropertyRelative("instance").managedReferenceValue = Activator.CreateInstance(type);
                    break;
                }
            }
        }

            
        EditorGUIExtensions.SearchableDropdown(new Rect(position.x + position.width / 2.0f + 2.0f, position.y, position.width / 2.0f - 2, EditorGUIUtility.singleLineHeight), l, options.ToArray(), (s) =>
        {
            Type t = FindTypeByName(s);
            if (t != null && !t.IsAbstract && !t.IsInterface)
            {
				p.FindPropertyRelative("instance").managedReferenceValue = Activator.CreateInstance(t);
				p.serializedObject.ApplyModifiedProperties();
			}
			
        });
        //int selected = EditorGUI.Popup(new Rect(position.x + position.width / 2.0f + 2.0f, position.y, position.width / 2.0f - 2, EditorGUIUtility.singleLineHeight), -1, options);
        //if (selected >= 0)
        //{
        //    property.FindPropertyRelative("element").managedReferenceValue = Activator.CreateInstance(types[selected]);
        //}
        position.x -= EditorGUIUtility.labelWidth;
        position.width += EditorGUIUtility.labelWidth;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("instance"), c, true);

    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("instance"), true);
    }

    private Type FindTypeByName(string typeName)
    {
        Type type = null;
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (Assembly assembly in assemblies)
        {
            type = assembly.GetTypes().FirstOrDefault(t => t.Name == typeName);
            if (type != null) break;
        }
        return type;
    }

    private List<Type> GetClassesInScript(TextAsset textAsset)
    {
        List<Type> classList = new List<Type>();
        string scriptContent = textAsset.text;

        string pattern = @"class\s+([A-Za-z_][A-Za-z0-9_]*)";
        MatchCollection matches = Regex.Matches(scriptContent, pattern);

        foreach (Match match in matches)
        {
            string className = match.Groups[1].Value;
            Type t = FindTypeByName(className);
            if (t != null && !t.IsAbstract && !t.IsInterface)
                classList.Add(t);
        }


        return classList;
    }
    
    void GatherSubTypes(string baseTypeString)
    {
        if (options == null || types == null)
        {
            Type baseType = FindTypeByName(baseTypeString);
                
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            types = assemblies.SelectMany(assembly => assembly.GetTypes()).Where(type => baseType.IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface).ToArray();
                
            options = new string[types.Length];
            for (var i = 0; i < types.Length; i++)
            {
                options[i] = types[i].Name;
            }
        }
    }
}