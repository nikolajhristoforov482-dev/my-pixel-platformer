using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class LevelAnalyzer : EditorWindow
{
    [MenuItem("Автоматизация Игры/8. Просканировать мой уровень")]
    public static void AnalyzePlayerLevel()
    {
        GameObject tilemapObj = GameObject.Find("Tilemap_Terrain");
        if (tilemapObj == null)
        {
            Debug.LogError("Ошибка: Не найден объект Tilemap_Terrain!");
            return;
        }

        Tilemap tilemap = tilemapObj.GetComponent<Tilemap>();
        BoundsInt bounds = tilemap.cellBounds;

        bool foundSpawnPoint = false;
        Vector3Int surfaceTilePos = Vector3Int.zero;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMax; y >= bounds.yMin; y--)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(tilePos))
                {
                    surfaceTilePos = tilePos;
                    foundSpawnPoint = true;
                    break;
                }
            }
            if (foundSpawnPoint) break;
        }

        if (foundSpawnPoint)
        {
            Vector3 worldSpawnPos = tilemap.CellToWorld(surfaceTilePos);
            worldSpawnPos.y += 1.6f; // Ваша идеальная высота над травой
            worldSpawnPos.x += 0.5f;

            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = worldSpawnPos;

                // АВТОМАТИЗАЦИЯ КАМЕРЫ: Находим Main Camera на сцене
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    // Безопасно добавляем скрипт следования камеры по его типу
                    System.Type camScriptType = System.Type.GetType("CameraFollow, Assembly-CSharp");
                    if (camScriptType != null)
                    {
                        Component followComp = mainCam.gameObject.GetComponent(camScriptType);
                        if (followComp == null) followComp = mainCam.gameObject.AddComponent(camScriptType);

                        // Через рефлексию привязываем игрока в поле 'target' скрипта камеры
                        camScriptType.GetField("target").SetValue(followComp, player.transform);
                    }

                    // Сразу ставим камеру в точку игрока, чтобы на старте не было резкого рывка
                    mainCam.transform.position = worldSpawnPos + new Vector3(0f, 1f, -10f);
                }

                Selection.activeGameObject = player;
                SceneView.FrameLastActiveSceneView();

                Debug.Log("Шаг 7.3: Игрок на месте, и плавная камера успешно настроена на него!");
            }
        }
    }
}
