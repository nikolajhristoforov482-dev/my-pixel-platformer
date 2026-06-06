using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class PlayerAnimationGenerator : EditorWindow
{
    [MenuItem("Автоматизация Игры/5. Сгенерировать Анимации Игрока")]
    public static void CreatePlayerAnimations()
    {
        string idlePath = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Idle (32x32).png";
        string runPath = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Run (32x32).png";

        // ДОБАВЛЕНО: Путь к падению, чтобы генератор принудительно исправил его масштаб на 32
        string fallPath = "Assets/Pixel Adventure 1/Assets/Main Characters/Ninja Frog/Fall (32x32).png";

        SetupTextureForAnimation(idlePath);
        SetupTextureForAnimation(runPath);
        SetupTextureForAnimation(fallPath); // Запускаем авто-исправление масштаба 32 для падения!

        if (!AssetDatabase.IsValidFolder("Assets/Generated_Game/Animations"))
            AssetDatabase.CreateFolder("Assets/Generated_Game", "Animations");

        AnimationClip idleClip = BuildClipForBase(idlePath, "Assets/Generated_Game/Animations/Player_Idle.anim", 11);
        AnimationClip runClip = BuildClipForBase(runPath, "Assets/Generated_Game/Animations/Player_Run.anim", 12);

        if (idleClip == null || runClip == null) return;

        var controller = AnimatorController.CreateAnimatorControllerAtPath("Assets/Generated_Game/Animations/Player_Controller.controller");

        // Исправленный синтаксис слоев для Unity 2019
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;

        AnimatorState idleState = rootStateMachine.AddState("Idle");
        idleState.motion = idleClip;

        AnimatorState runState = rootStateMachine.AddState("Run");
        runState.motion = runClip;

        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);

        AnimatorStateTransition toRun = idleState.AddTransition(runState);
        toRun.AddCondition(AnimatorConditionMode.If, 0, "IsRunning");
        toRun.hasExitTime = false; toRun.duration = 0;

        AnimatorStateTransition toIdle = runState.AddTransition(idleState);
        toIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsRunning");
        toIdle.hasExitTime = false; toIdle.duration = 0;

        GameObject player = GameObject.Find("Player");
        if (player != null)
        {
            Animator animComponent = player.GetComponent<Animator>();
            if (animComponent == null) animComponent = player.AddComponent<Animator>();
            animComponent.runtimeAnimatorController = controller;
            Debug.Log("Базовые анимации и масштаб падения успешно обновлены!");
        }
        AssetDatabase.SaveAssets();
    }

    private static void SetupTextureForAnimation(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.spritePixelsPerUnit = 32; // Железно фиксируем пиксели!

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

    private static AnimationClip BuildClipForBase(string assetPath, string savePath, int frameCount)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        System.Collections.Generic.List<Sprite> sprites = new System.Collections.Generic.List<Sprite>();
        foreach (Object o in assets) { if (o is Sprite) sprites.Add((Sprite)o); }
        if (sprites.Count == 0) return null;

        AnimationClip clip = new AnimationClip();
        clip.frameRate = 20;
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[Mathf.Min(frameCount, sprites.Count)];
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
