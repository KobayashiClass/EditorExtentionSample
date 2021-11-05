using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

/// <summary>
/// 簡易単体テストを行うエディタ拡張
/// </summary>
public class UnitTestWindow : EditorWindow
{
    //ウインドウをメニューから呼び出す
    [MenuItem("Window/Unit Test Window")]
    static void Init()
    {
        UnitTestWindow window = GetWindow<UnitTestWindow>();
        window.Show();
    }

    GameObject m_selectedGameObject;
    bool IsGameObjectSelected { get => m_selectedGameObject != null; }

    MonoBehaviour[] SelectedComponents { get => m_selectedGameObject.GetComponents<MonoBehaviour>(); }
    string[] ComponentNames { get => SelectedComponents.Select((com) => com.GetType().Name).ToArray(); }
    
    int m_selectComponentIndex;
    MonoBehaviour SelectComponent { get => SelectedComponents[m_selectComponentIndex]; }
    System.Type SelectComponentType { get => SelectedComponents[m_selectComponentIndex].GetType(); }
    bool HasComponent { get => SelectedComponents != null && SelectedComponents.Length > 0; }

    System.Reflection.MethodInfo[] SelectedMethods { get => SelectComponentType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly); }
    string[] MethodNames { get => SelectedMethods.Select((method) => method.Name).ToArray();  }

    int m_selectMethodIndex;
    System.Reflection.MethodInfo SelectMethod { get => SelectedMethods[m_selectMethodIndex]; }
    bool HasMedhod { get => SelectComponent != null && SelectedMethods.Length > 0; }

    System.Reflection.ParameterInfo[] Parameters { get => SelectMethod.GetParameters(); }


    void OnSelectionChange()
    {
        m_selectedGameObject = Selection.activeGameObject;
        Repaint();
    }

    void Awake()
    {
        m_selectComponentIndex = 0;
        m_selectMethodIndex = 0;
        m_selectedGameObject = Selection.activeGameObject;
    }

    void OnGUI()
    {
        var canRun =
            ShowComponentPulldown() &&
            ShowMethodPulldown() &&
            ShowArgumentList();

        if (canRun) TestRun();
    }

    void NonSelectedGUI(string message)
    {
        EditorGUILayout.LabelField(message);
    }

    object[] m_arguments;

    bool ShowComponentPulldown()
    {
        if (!IsGameObjectSelected || !HasComponent)
        {
            NonSelectedGUI("[ MonoBehaviour ] を継承したコンポーネントがアタッチされたゲームオブジェクトを選択してください");
            return false;
        }
        m_selectComponentIndex = EditorGUILayout.Popup(m_selectComponentIndex, ComponentNames);
        return true;
    }

    bool ShowMethodPulldown()
    {
        if (!HasMedhod)
        {
            NonSelectedGUI("実行できるメソッドが存在しません");
            return false;
        }
        m_selectMethodIndex = EditorGUILayout.Popup(m_selectMethodIndex, MethodNames);
        return true;
    }

    bool ShowArgumentList()
    {
        var parameterTypes = Parameters;
        if (m_arguments == null || parameterTypes.Length != m_arguments.Length)
        {
            m_arguments = new object[parameterTypes.Length];
        }

        EditorGUILayout.BeginHorizontal();
        bool result = true;
        for (int i = 0; i < parameterTypes.Length; i++)
        {
            result = ShowParameters(parameterTypes[i], ref m_arguments[i]) && result;
        }
        EditorGUILayout.EndHorizontal();

        if (!result)
        {
            NonSelectedGUI("シリアライズ不可能な引数が存在します。ごめんね");
        }

        return result;
    }

    bool ShowParameters(System.Reflection.ParameterInfo paramInfo, ref object current)
    {
        if (paramInfo.ParameterType == typeof(int))
        {
            if (current == null || current.GetType() != typeof(int)) current = 0;
            var currentValue = (int)current;
            current = EditorGUILayout.IntField(paramInfo.Name, currentValue);
        }
        else if (paramInfo.ParameterType == typeof(float))
        {
            if (current == null || current.GetType() != typeof(float)) current = 0f;
            var currentValue = (float)current;
            current = EditorGUILayout.FloatField(paramInfo.Name, currentValue);
        }
        else if (current == null || paramInfo.ParameterType == typeof(string))
        {
            if (current.GetType() != typeof(string)) current = 0;
            var currentValue = (string)current;
            current = EditorGUILayout.TextField(paramInfo.Name, currentValue);
        }
        else
        {
            return false;
        }
        return true;
    }

    void TestRun()
    {
        if (GUILayout.Button(SelectMethod.Name + $"( {String.Join(", ", m_arguments.Select((arg) => $"{arg}: {arg.GetType().Name}"))} )"))
        {
            SelectMethod.Invoke(SelectComponent, m_arguments);
        }
    }
}