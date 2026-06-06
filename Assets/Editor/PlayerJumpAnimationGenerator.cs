using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class PlayerJumpAnimationGenerator : EditorWindow
{
    [MenuItem("Автоматизация Игры/6. Сгенерировать Прыжки Игрока")]
    public static void CreateJumpAnimations()
    {
        string jumpPath = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Jump (32x32).png";
        string doubleJumpPath = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Double Jump (32x32).png";

        SetupJumpTexture(jumpPath);
        SetupJumpTexture(doubleJumpPath);

        if (!AssetDatabase.IsValidFolder("Assets/Generated_Game/Animations/Player"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Generated_Game/Animations"))
                AssetDatabase.CreateFolder("Assets/Generated_Game", "Animations");

            AssetDatabase.CreateFolder("Assets/Generated_Game/Animations", "Player");
        }

        AnimationClip jumpClip = BuildJumpClip(jumpPath, "Assets/Generated_Game/Animations/Player/Player_Jump.anim", false);
        AnimationClip doubleJumpClip = BuildJumpClip(doubleJumpPath, "Assets/Generated_Game/Animations/Player/Player_DoubleJump.anim", false);

        if (jumpClip == null || doubleJumpClip == null) return;

        string[] guids = AssetDatabase.FindAssets("Player_Controller t:AnimatorController");
        AnimatorController controller = null;

        if (guids.Length > 0)
        {
            string actualPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(actualPath);
        }

        if (controller != null)
        {
            // Исправлено для Unity 2019: берем базовый слой массива слоев
            AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

            foreach (var state in rootStateMachine.states)
            {
                if (state.state.name == "Jump" || state.state.name == "DoubleJump")
                {
                    rootStateMachine.RemoveState(state.state);
                }
            }

            AnimatorState jumpState = rootStateMachine.AddState("Jump");
            jumpState.motion = jumpClip;

            AnimatorState doubleJumpState = rootStateMachine.AddState("DoubleJump");
            doubleJumpState.motion = doubleJumpClip;
            doubleJumpState.speed = 2.0f; // Быстрое сальто

            foreach (var state in rootStateMachine.states)
            {
                if (state.state.name == "Idle")
                {
                    // ИСПРАВЛЕНО: Включаем Exit Time для ОБОИХ состояний прыжка.
                    // Теперь аниматор Unity 2019 не будет выдавать предупреждений, 
                    // а скрипт движения сможет корректно управлять картинками!
                    var fromDouble = doubleJumpState.AddTransition(state.state);
                    fromDouble.hasExitTime = true;
                    fromDouble.exitTime = 1.0f;
                    fromDouble.duration = 0f;

                    var fromJump = jumpState.AddTransition(state.state);
                    fromJump.hasExitTime = true;
                    fromJump.exitTime = 1.0f;
                    fromJump.duration = 0f;
                }
            }

            Debug.Log("Шаг 5.6: Граф анимации прыжков успешно приведен в идеальный порядок для Unity 2019!");
        }
        AssetDatabase.SaveAssets();
    }

    private static void SetupJumpTexture(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 32;

            int spriteSize = 32;
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (tex != null)
            {
                int countX = tex.width / spriteSize;
                System.Collections.Generic.List<SpriteMetaData> metaData = new System.Collections.Generic.List<SpriteMetaData>();
                for (int i = 0; i < countX; i++)
                {
                    SpriteMetaData meta = new SpriteMetaData();
                    meta.rect = new Rect(i * spriteSize, 0, spriteSize, spriteSize);
                    meta.name = System.IO.Path.GetFileNameWithoutExtension(path) + "_" + i;
                    metaData.Add(meta);
                }
                importer.spritesheet = metaData.ToArray();
            }
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }

    private static AnimationClip BuildJumpClip(string assetPath, string savePath, bool isLooping)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        System.Collections.Generic.List<Sprite> sprites = new System.Collections.Generic.List<Sprite>();
        foreach (Object o in assets) { if (o is Sprite) sprites.Add((Sprite)o); }
        if (sprites.Count == 0) return null;

        AnimationClip clip = new AnimationClip();
        clip.frameRate = 20;
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = isLooping;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[sprites.Count];
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i] = new ObjectReferenceKeyframe { time = i * (1f / 20f), value = sprites[i] };
        }

        EditorCurveBinding binding = new EditorCurveBinding { type = typeof(SpriteRenderer), path = "", propertyName = "m_Sprite" };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        AssetDatabase.DeleteAsset(savePath);
        AssetDatabase.CreateAsset(clip, savePath);
        return clip;
    }
}
