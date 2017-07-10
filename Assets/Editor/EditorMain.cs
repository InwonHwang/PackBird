using UnityEngine;
using UnityEditor;

public class EditorMain {

    [MenuItem("MapEditor/ObjectWindow")]
    public static void ShowObjectWindow()
    {
        EditorWindow.GetWindow(typeof(ObjectWindow));
    }
}
