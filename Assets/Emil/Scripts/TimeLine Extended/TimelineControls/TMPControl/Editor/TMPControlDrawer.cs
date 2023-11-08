using UnityEditor;
using UnityEngine;
using LandRocker.TimelineTrack;

[CustomPropertyDrawer(typeof(TMPBehaviour))]
public class TMPControlDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int fieldCount = 4;
        return fieldCount * EditorGUIUtility.singleLineHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty colorProp = property.FindPropertyRelative("color");
        SerializedProperty textProp = property.FindPropertyRelative("text");
        SerializedProperty isModifyingCharacterSpacingProp = property.FindPropertyRelative("isModifyingCharacterSpacing");
        SerializedProperty characterSpacingProp = property.FindPropertyRelative("characterSpacing");
        SerializedProperty lineSpacingProp = property.FindPropertyRelative("lineSpacing");
        SerializedProperty paragraphSpacingProp = property.FindPropertyRelative("paragraphSpacing");
        SerializedProperty wordSpacingProp = property.FindPropertyRelative("wordSpacing");

        Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(singleFieldRect, colorProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, textProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, isModifyingCharacterSpacingProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, characterSpacingProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, lineSpacingProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, paragraphSpacingProp);

        singleFieldRect.y += EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(singleFieldRect, wordSpacingProp);
    }
}