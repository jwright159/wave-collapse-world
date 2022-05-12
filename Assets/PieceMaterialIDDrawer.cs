using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(PieceMaterialID))]
public class PieceMaterialIDDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		EditorGUI.PropertyField(position, property.FindPropertyRelative("id"), label);

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("id"), label);
	}
}