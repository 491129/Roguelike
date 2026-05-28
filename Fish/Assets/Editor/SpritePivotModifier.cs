using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SpritePivotModifier : EditorWindow
{
    private Vector2 pivotPoint = new Vector2(0.5f, 0.5f); // 0.5为中心

    [MenuItem("Tools/批量修改精灵轴心")]
    public static void ShowWindow()
    {
        GetWindow<SpritePivotModifier>("批量修改精灵轴心");
    }

    private void OnGUI()
    {
        GUILayout.Label("修改所有选中精灵的轴心点", EditorStyles.boldLabel);
        pivotPoint = EditorGUILayout.Vector2Field("轴心点坐标 (0-1):", pivotPoint);

        if (GUILayout.Button("应用到选中的精灵"))
        {
            ApplyPivotToSelectedSprites();
        }
    }

    // 改为非静态方法，这样就可以直接访问实例变量 pivotPoint
    private void ApplyPivotToSelectedSprites()
    {
        List<string> paths = new List<string>();
        foreach (string guid in Selection.assetGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            paths.Add(path);
        }

        int count = 0;
        foreach (string path in paths)
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType == TextureImporterType.Sprite)
            {
                importer.spritePivot = pivotPoint;
                importer.SaveAndReimport();
                count++;
                Debug.Log("修改完成: " + path);
            }
        }
        Debug.Log($"成功修改了 {count} 个精灵的轴心点。");
    }
}