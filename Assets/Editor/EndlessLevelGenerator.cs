using UnityEngine;
using UnityEditor;

public class EndlessLevelGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Level")]
    public static void ShowWindow()
    {
        GetWindow<EndlessLevelGenerator>("Level Generator");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Сгенерировать карту с шипами"))
        {
            CreateLevel();
        }
    }

    private void CreateLevel()
    {
        GameObject root = GameObject.Find("Level_Root");
        if (root != null)
        {
            DestroyImmediate(root);
        }

        root = new GameObject("Level_Root");

        for (int i = 0; i < 20; i++)
        {
            GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = "Platform_" + i;
            platform.transform.parent = root.transform;
            platform.transform.position = new Vector3(i * 5, -2, 0);
            platform.transform.localScale = new Vector3(4, 1, 1);

            DestroyImmediate(platform.GetComponent<BoxCollider>());
            platform.AddComponent<BoxCollider2D>();

            if (i > 0 && i % 2 == 0)
            {
                GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cube);
                spike.name = "Spike_" + i;
                spike.transform.parent = root.transform;
                spike.transform.position = new Vector3(i * 5, -1.2f, 0);
                spike.transform.localScale = new Vector3(0.6f, 0.6f, 1);
                spike.tag = "Trap";

                Renderer spikeRenderer = spike.GetComponent<Renderer>();
                if (spikeRenderer != null)
                {
                    spikeRenderer.sharedMaterial.color = Color.red;
                }

                DestroyImmediate(spike.GetComponent<BoxCollider>());
                BoxCollider2D col2D = spike.AddComponent<BoxCollider2D>();
                col2D.isTrigger = true;

                spike.AddComponent<SpikeTrap>();
            }
        }
    }
}