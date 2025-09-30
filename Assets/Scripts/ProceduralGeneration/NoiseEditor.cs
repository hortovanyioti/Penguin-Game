using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Noise))]
public class NoiseEditor : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);

		// Draw primary label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Find properties
		SerializedProperty freqProp = property.FindPropertyRelative("Frequency");
		SerializedProperty ampProp = property.FindPropertyRelative("Amplitude");

		// Draw fields and retrieve new values
		var labels = new GUIContent[] { new GUIContent("Frequency"), new GUIContent("Amplitude") };
		var values = new float[] { freqProp.floatValue, ampProp.floatValue };

		EditorGUI.MultiFloatField(position, labels, values);

		freqProp.floatValue = values[0];
		ampProp.floatValue = values[1];

		EditorGUI.EndProperty();
	}
}
