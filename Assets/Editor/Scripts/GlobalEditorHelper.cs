using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AgeOfHeroes.Editor
{
    public static class GlobalEditorHelper
    {
        public static void DrawFields(FieldInfo[] fieldInfos, object instance, Vector2 scrollView, bool showLists = false)
        {
            // scrollView = EditorGUILayout.BeginScrollView(scrollView);
            foreach (var field in fieldInfos)
            {
                bool isString = field.FieldType.ToString() == "System.String";
                if (isString)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    var value = EditorGUILayout.TextField((string)field.GetValue(instance));
                    field.SetValue(instance, value);
                    EditorGUILayout.EndHorizontal();
                }

                bool isInteger = field.FieldType.ToString() == "System.Int32";
                if (isInteger)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    var value = EditorGUILayout.IntField((int)field.GetValue(instance));
                    field.SetValue(instance, value);
                    EditorGUILayout.EndHorizontal();
                }

                bool isBoolean = field.FieldType.ToString() == "System.Boolean";
                if (isBoolean)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    var value = EditorGUILayout.Toggle((bool)field.GetValue(instance));
                    field.SetValue(instance, value);
                    EditorGUILayout.EndHorizontal();
                }

                bool isFloat = field.FieldType.ToString() == "System.Single";
                if (isFloat)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    var value = EditorGUILayout.FloatField((float)field.GetValue(instance));
                    field.SetValue(instance, value);
                    EditorGUILayout.EndHorizontal();
                }

                bool isSprite = field.FieldType.ToString() == "UnityEngine.Sprite";
                if (isSprite)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    var value = (Sprite)EditorGUILayout.ObjectField((Sprite)field.GetValue(instance), typeof(Sprite),
                        true, GUILayout.MinHeight(64f), GUILayout.MaxWidth(64f));
                    field.SetValue(instance, value);
                    EditorGUILayout.EndHorizontal();
                }


                bool isEnum = field.FieldType.IsEnum;
                if (isEnum)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(field.Name);
                    var value = EditorGUILayout.EnumPopup((Enum)field.GetValue(instance));
                    field.SetValue(instance, value);
                    EditorGUILayout.EndHorizontal();
                }

                if (!showLists) continue;
                GUIStyle removeBtnStyle = new GUIStyle(GUI.skin.button);
                removeBtnStyle.normal.textColor = Color.red;
                GUIStyle addBtnStyle = new GUIStyle(GUI.skin.button);
                addBtnStyle.normal.textColor = Color.green;
                bool isStringsList = field.FieldType.ToString() == "System.Collections.Generic.List`1[System.String]";
                if (isStringsList)
                {
                    EditorGUILayout.LabelField(field.Name);
                    var list = field.GetValue(instance) as IList<string>;
                    for (int i = 0; i < list.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        list[i] = EditorGUILayout.TextField($"{i})", list[i]);
                        if (GUILayout.Button("-", removeBtnStyle, GUILayout.MaxWidth(64f)))
                        {
                            list.RemoveAt(i);
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    field.SetValue(instance, list);
                    EditorGUILayout.Space(5f);
                    if (GUILayout.Button("+", addBtnStyle, GUILayout.MaxWidth(128f)))
                    {
                        list.Add(string.Empty);
                    }
                }
            }
            // EditorGUILayout.EndScrollView();
        }
    }
}