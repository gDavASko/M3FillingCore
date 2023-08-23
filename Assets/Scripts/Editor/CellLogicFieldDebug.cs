using UnityEngine;
using UnityEditor;
using UnityEditor.Graphs;


[CustomEditor(typeof(CellLogicBase))]
public class CellLogicDebugInfo : Editor
{
    public override void OnInspectorGUI()
    {
        ICellLogic component = (ICellLogic)target;
        DrawDefaultInspector();

        string debugInfo = "";
        EditorGUILayout.LabelField("Cell references!");

        GUILayout.Label($"Current Chip: {(component.Slot.Chip == null ? "null" : component.Slot.Chip.transform.name)}");
        GUILayout.Label($"Current Cover: {(component.Slot.Cover == null ? "null" : component.Slot.Cover.transform.name)}");
        GUILayout.Label($"Current Generator: {(component.Slot.Generator == null ? "null" : component.Slot.Generator.transform.name)}");
        GUILayout.Label($"Can Put Chip: {component.Slot.CanPutChip}");

        debugInfo = component.UpCell != null
            ? $"[{component.UpCell.Slot.Info.Position.x},{component.UpCell.Slot.Info.Position.y}]"
            : "null";
        GUILayout.Label($"Up cell: {debugInfo}");

        debugInfo = component.RightCell != null
            ? $"[{component.RightCell.Slot.Info.Position.x},{component.RightCell.Slot.Info.Position.y}]"
            : "null";
        GUILayout.Label($"Right cell: {debugInfo}");

        debugInfo = component.DownCell != null
            ? $"[{component.DownCell.Slot.Info.Position.x},{component.DownCell.Slot.Info.Position.y}]"
            : "null";
        GUILayout.Label($"Down cell: {debugInfo}");

        debugInfo = component.LeftCell != null
            ? $"[{component.LeftCell.Slot.Info.Position.x},{component.LeftCell.Slot.Info.Position.y}]"
            : "null";
        GUILayout.Label($"Left cell: {debugInfo}");

    }
}