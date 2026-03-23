using CatCode.InteractionLocking;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(InteractionLayersAsset))]
public class InteractionLayersAssetEditor : Editor
{
    private SerializedProperty _layersProp;
    private ReorderableList _reorderableList;
    private int _editingIndex = -1;
    private string _editingName;

    private void OnEnable()
    {
        _layersProp = serializedObject.FindProperty("_layers");

        _reorderableList = new(serializedObject, _layersProp, true, true, true, true)
        {
            drawHeaderCallback = DrawHeader,
            drawElementCallback = DrawElement,
            onAddCallback = AddLayer,
            onRemoveCallback = RemoveLayer
        };
    }

    private void DrawHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Interaction Layers");
    }

    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = _layersProp.GetArrayElementAtIndex(index);
        var layer = element.objectReferenceValue as ScriptableObject;

        if (layer == null)
            return;

        rect.height = EditorGUIUtility.singleLineHeight;
        var nameRect = new Rect(rect.x, rect.y, rect.width - 120, rect.height);
        var renameButtonRect = new Rect(rect.x + rect.width - 115, rect.y, 55, rect.height);

        if (_editingIndex == index)
        {
            _editingName = EditorGUI.TextField(nameRect, _editingName);

            if (GUI.Button(renameButtonRect, "OK"))
            {
                layer.name = string.IsNullOrEmpty(_editingName) ? layer.name : _editingName;
                EditorUtility.SetDirty(layer);
                AssetDatabase.SaveAssets();
                _editingIndex = -1;
            }
        }
        else
        {
            EditorGUI.LabelField(nameRect, layer.name);
            if (GUI.Button(renameButtonRect, "Rename"))
            {
                _editingIndex = index;
                _editingName = layer.name;
            }
        }
    }

    private void AddLayer(ReorderableList list)
    {
        var asset = (InteractionLayersAsset)target;
        var newLayer = ScriptableObject.CreateInstance<InteractionLayerAsset>();
        newLayer.name = "NewLayer";
        AssetDatabase.AddObjectToAsset(newLayer, asset);
        AssetDatabase.SaveAssets();

        _layersProp.arraySize++;
        _layersProp.GetArrayElementAtIndex(_layersProp.arraySize - 1).objectReferenceValue = newLayer;
    }

    private void RemoveLayer(ReorderableList list)
    {
        var element = _layersProp.GetArrayElementAtIndex(list.index);
        var layer = element.objectReferenceValue as ScriptableObject;

        if (layer != null)
        {
            ScriptableObject.DestroyImmediate(layer, true);
            AssetDatabase.SaveAssets();
        }

        _layersProp.DeleteArrayElementAtIndex(list.index);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}