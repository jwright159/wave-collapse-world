using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(PieceMaterialID))]
public class PieceMaterialIDDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		float space = EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
		Event ev = Event.current;

		bool dirty = false;

		bool wasExpanded = property.isExpanded;
		property.isExpanded = EditorGUI.Foldout(new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), wasExpanded, label);
		dirty |= property.isExpanded != wasExpanded;
		if (property.isExpanded)
		{
			SerializedProperty id = property.FindPropertyRelative("id");
			PieceMaterialID actualid = (PieceMaterialID)fieldInfo.GetValue(property.serializedObject.targetObject);

			int idwidth = property.FindPropertyRelative("width").intValue;
			int newidwidth = EditorGUI.DelayedIntField(new(position.x, position.y + space, position.width, EditorGUIUtility.singleLineHeight), "Width", idwidth);
			if (newidwidth < 1)
				newidwidth = 1;

			for (int x = 0; x < idwidth; x++)
				for (int y = 0; y < idwidth; y++)
				{
					SerializedProperty value = id.GetArrayElementAtIndex(x + y * idwidth);
					Rect rect = new(
						position.x + x * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing),
						position.y + space * 2 + y * space,
						EditorGUIUtility.singleLineHeight,
						EditorGUIUtility.singleLineHeight
					);
					EditorGUI.DrawRect(rect, new Color(((value.intValue >> 16) & 0xFF) / (float)0xFF, ((value.intValue >> 8) & 0xFF) / (float)0xFF, (value.intValue & 0xFF) / (float)0xFF));
					if (ev.type == EventType.MouseDown && ev.button == 0 && rect.Contains(ev.mousePosition))
					{
						GenericMenu context = new GenericMenu();
						foreach (PieceMaterial material in Enum.GetValues(typeof(PieceMaterial)))
							context.AddItem(new GUIContent(material.ToString()), value.intValue == (int)material, material => { value.intValue = (int)material; value.serializedObject.ApplyModifiedProperties(); }, material);
						context.ShowAsContext();
					}
				}

			if (idwidth != newidwidth)
				actualid.Resize(newidwidth);
		}

		EditorGUI.EndProperty();
		if (dirty)
			EditorUtility.SetDirty(property.serializedObject.targetObject);
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		int idwidth = (int)Mathf.Sqrt(property.FindPropertyRelative("id").arraySize);
		return property.isExpanded ? EditorGUIUtility.singleLineHeight * (idwidth + 2) + EditorGUIUtility.standardVerticalSpacing * (idwidth + 1) : EditorGUIUtility.singleLineHeight;
	}
}