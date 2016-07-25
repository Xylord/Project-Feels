using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGrid))]
public class GridInspector : Editor
{

    private LevelGrid grid;

	// Use this for initialization
	void Start () {
	
	}

    // Update is called once per frame
    void Update() {

    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();
        grid = target as LevelGrid;

        EditorGUI.BeginChangeCheck();

        if (GUILayout.Button("Generate Empty Grid"))
        {
            Undo.RecordObject(grid, "Generate Empty Grid");
            grid.GenerateEmptyGridEditor();
            EditorUtility.SetDirty(grid);
        }

        if (GUILayout.Button("Clear Grid"))
        {
            Undo.RecordObject(grid, "Clear Grid");
            grid.ClearGridEditor();
            EditorUtility.SetDirty(grid);
        }
    }

}
