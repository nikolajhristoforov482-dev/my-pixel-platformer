using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class LevelGenerator : EditorWindow
{
    [MenuItem("Автоматизация Игры/3. Подготовить пустой холст для рисования")]
    public static void CreateEmptyCanvas()
    {
        // Удаляем старый Grid, если он был, чтобы не плодить копии
        GameObject oldGrid = GameObject.Find("Grid");
        if (oldGrid != null) DestroyImmediate(oldGrid);

        // 1. Создаем правильную структуру сетки
        GameObject gridObj = new GameObject("Grid");
        gridObj.AddComponent<Grid>();

        GameObject tilemapObj = new GameObject("Tilemap_Terrain");
        tilemapObj.transform.SetParent(gridObj.transform);

        Tilemap tilemap = tilemapObj.AddComponent<Tilemap>();
        tilemapObj.AddComponent<TilemapRenderer>();

        // 2. Железно настраиваем физику склейки
        TilemapCollider2D tilemapCollider = tilemapObj.AddComponent<TilemapCollider2D>();
        tilemapObj.AddComponent<CompositeCollider2D>();
        tilemapCollider.usedByComposite = true;

        Rigidbody2D rb = tilemapObj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.bodyType = RigidbodyType2D.Static;

        // 3. Выставляем слой Ignore Raycast, чтобы Ниндзя Фрог идеально прыгал
        tilemapObj.layer = 2;

        Selection.activeGameObject = tilemapObj;
        Debug.Log("Холст готов! Открывайте окно Window -> 2D -> Tile Palette и рисуйте уровень вашей мечты!");
    }
}
