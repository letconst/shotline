using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// JoystickのInspectorを変えるエディター
/// </summary>
[CustomEditor(typeof(Joystick))]
public class JoystickEditor : Editor
{

    //プロパティーを編集する用
    private SerializedProperty _radiusProperty;
    private SerializedProperty _shouldResetPositionProperty;
    private SerializedProperty _positionProperty;

    //=================================================================================
    //初期化
    //=================================================================================

    private void OnEnable()
    {
        //プロパティ読み込み
        _radiusProperty = serializedObject.FindProperty("_radius");
        _shouldResetPositionProperty = serializedObject.FindProperty("_shouldResetPosition");
        _positionProperty = serializedObject.FindProperty("_position");
    }

    //=================================================================================
    //更新
    //=================================================================================

    //Inspectorを更新する
    public override void OnInspectorGUI()
    {
        //更新開始
        serializedObject.Update();

        //スティックが動く範囲の半径
        float radius = EditorGUILayout.FloatField("動作範囲の半径", _radiusProperty.floatValue);
        if (radius != _radiusProperty.floatValue)
        {
            _radiusProperty.floatValue = radius;
        }

        //指を離した時にスティックが中心に戻るか
        bool shouldResetPosition = EditorGUILayout.Toggle("Stickが中心に戻るか", _shouldResetPositionProperty.boolValue);
        if (shouldResetPosition != _shouldResetPositionProperty.boolValue)
        {
            _shouldResetPositionProperty.boolValue = shouldResetPosition;
        }

        //現在地を表示
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(
          "Position(-1~1)   X : " +
          _positionProperty.vector2Value.x.ToString("F2") + ",  Y : " +
          _positionProperty.vector2Value.y.ToString("F2")
        );
        EditorGUILayout.EndVertical();

        //更新終わり
        serializedObject.ApplyModifiedProperties();
    }

}