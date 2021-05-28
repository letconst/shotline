using UnityEngine;
using UnityEditor;
using System.Collections;

/// <summary>
/// Joystick��Inspector��ς���G�f�B�^�[
/// </summary>
[CustomEditor(typeof(Joystick))]
public class JoystickEditor : Editor
{

    //�v���p�e�B�[��ҏW����p
    private SerializedProperty _radiusProperty;
    private SerializedProperty _shouldResetPositionProperty;
    private SerializedProperty _positionProperty;

    //=================================================================================
    //������
    //=================================================================================

    private void OnEnable()
    {
        //�v���p�e�B�ǂݍ���
        _radiusProperty = serializedObject.FindProperty("_radius");
        _shouldResetPositionProperty = serializedObject.FindProperty("_shouldResetPosition");
        _positionProperty = serializedObject.FindProperty("_position");
    }

    //=================================================================================
    //�X�V
    //=================================================================================

    //Inspector���X�V����
    public override void OnInspectorGUI()
    {
        //�X�V�J�n
        serializedObject.Update();

        //�X�e�B�b�N�������͈͂̔��a
        float radius = EditorGUILayout.FloatField("����͈͂̔��a", _radiusProperty.floatValue);
        if (radius != _radiusProperty.floatValue)
        {
            _radiusProperty.floatValue = radius;
        }

        //�w�𗣂������ɃX�e�B�b�N�����S�ɖ߂邩
        bool shouldResetPosition = EditorGUILayout.Toggle("Stick�����S�ɖ߂邩", _shouldResetPositionProperty.boolValue);
        if (shouldResetPosition != _shouldResetPositionProperty.boolValue)
        {
            _shouldResetPositionProperty.boolValue = shouldResetPosition;
        }

        //���ݒn��\��
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField(
          "Position(-1~1)   X : " +
          _positionProperty.vector2Value.x.ToString("F2") + ",  Y : " +
          _positionProperty.vector2Value.y.ToString("F2")
        );
        EditorGUILayout.EndVertical();

        //�X�V�I���
        serializedObject.ApplyModifiedProperties();
    }

}