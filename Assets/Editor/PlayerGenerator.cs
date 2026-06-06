using UnityEngine;
using UnityEditor;

public class PlayerGenerator : EditorWindow
{
    [MenuItem("Автоматизация Игры/4. Создать Основу Героя")]
    public static void CreatePlayerBase()
    {
        string playerPath = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Idle (32x32).png";

        TextureImporter importer = AssetImporter.GetAtPath(playerPath) as TextureImporter;
        if (importer != null)
        {
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 32;
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }

        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(playerPath);
        Sprite startSprite = null;
        foreach (Object asset in allAssets)
        {
            if (asset is Sprite) { startSprite = (Sprite)asset; break; }
        }

        if (startSprite == null)
        {
            Debug.LogError("Ошибка: Не удалось найти спрайт Ниндзя Фрога!");
            return;
        }

        GameObject player = new GameObject("Player");
        player.tag = "Player";
        player.transform.position = new Vector3(5, 2, 0);

        SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = startSprite;
        sr.sortingOrder = 5;

        Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        // НАСТРОЙКА: Делаем физику еще плотнее и тяжелее строго по вашей просьбе!
        rb.gravityScale = 3.5f;

        CapsuleCollider2D capsule = player.AddComponent<CapsuleCollider2D>();
        capsule.direction = CapsuleDirection2D.Vertical;
        capsule.size = new Vector2(0.75f, 0.95f);
        capsule.offset = new Vector2(0f, -0.02f);

        // ИСПРАВЛЕНО: Профессиональный и безопасный поиск типа скрипта без вызова API Updater
        System.Type scriptType = System.Type.GetType("PlayerMovement, Assembly-CSharp");
        if (scriptType != null)
        {
            player.AddComponent(scriptType);
        }
        else
        {
            Debug.LogWarning("Предупреждение: Скрипт PlayerMovement еще не скомпилирован. Перейдите в Unity, чтобы проект обновился.");
        }

        Selection.activeGameObject = player;
        SceneView.FrameLastActiveSceneView();

        Debug.Log("Шаг 3.2: Герой Player с капсульным коллайдером успешно создан без ошибок устаревшего API!");
    }
}
