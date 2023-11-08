using UnityEditor;
using UnityEngine;
using LandRocker.TimelineTrack;

[CustomPropertyDrawer(typeof(ImageControlBehaviour))]
public class ImageControlDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty alphaProp = property.FindPropertyRelative("alpha");
        SerializedProperty isUsingBehaviourAlphaProp = property.FindPropertyRelative("isUsingBehaviourAlpha");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, alphaProp);
        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, isUsingBehaviourAlphaProp);
    }
}
