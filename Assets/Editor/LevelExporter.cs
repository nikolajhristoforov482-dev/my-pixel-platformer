using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;
using System.Text;

public class LevelExporter : EditorWindow
{
    [MenuItem("Автоматизация Игры/Выгрузить мой рисунок для ИИ")]
    public static void ExportMapToText()
    {
        GameObject tilemapObj = GameObject.Find("Tilemap_Terrain");
        if (tilemapObj == null)
        {
            Debug.LogError("Ошибка: Не найден объект Tilemap_Terrain!");
            return;
        }

        Tilemap tilemap = tilemapObj.GetComponent<Tilemap>();
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        StringBuilder sb = new StringBuilder();

        // Сканируем строго сверху вниз
        for (int y = bounds.yMax - 1; y >= bounds.yMin; y--)
        {
            StringBuilder row = new StringBuilder();
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile(pos);

                if (tile == null)
                {
                    row.Append("0");
                }
                else
                {
                    string name = tile.name;
                    if (name.Contains("_6")) row.Append("G");
                    else if (name.Contains("_5")) row.Append("L");
                    else if (name.Contains("_7")) row.Append("R");
                    else if (name.Contains("_22")) row.Append("W");
                    else if (name.Contains("_24")) row.Append("E");
                    else if (name.Contains("_23")) row.Append("C");
                    else if (name.Contains("_38")) row.Append("B");
                    else if (name.Contains("_37")) row.Append("X");
                    else if (name.Contains("_39")) row.Append("Z");
                    else if (name.Contains("_31")) row.Append("[");
                    else if (name.Contains("_32")) row.Append("=");
                    else if (name.Contains("_33")) row.Append("]");
                    else row.Append("1");
                }
            }
            sb.AppendLine(row.ToString());
        }

        string path = Application.dataPath + "/MyHanddrawnLevel.txt";
        File.WriteAllText(path, sb.ToString());

        Debug.Log("Успешно! База данных комнат обновлена в файле Assets/MyHanddrawnLevel.txt");
        AssetDatabase.Refresh();
    }
}
