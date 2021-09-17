using UnityEditor;

[CustomEditor(typeof(ScrollSnapSelector))]
public sealed class ScrollRectSnapEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}