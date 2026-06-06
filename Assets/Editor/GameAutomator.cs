using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.SceneManagement;

public class GameAutomator : EditorWindow
{
    [MenuItem("Автоматизация Игры/1. Создать Главное Меню")]
    public static void CreateMainMenu()
    {
        var sc = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        if (Camera.main != null) Camera.main.backgroundColor = Color.black;

        GameObject canv = new GameObject("MainMenuCanvas");
        Canvas c = canv.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
        canv.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canv.AddComponent<GraphicRaycaster>();

        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            GameObject ev = new GameObject("EventSystem");
            ev.AddComponent<EventSystem>(); ev.AddComponent<StandaloneInputModule>();
        }

        MainMenuController mc = canv.AddComponent<MainMenuController>();
        GameObject bg = new GameObject("MenuBackground"); bg.transform.SetParent(canv.transform, false);
        RectTransform bgR = bg.AddComponent<RectTransform>(); bgR.anchorMin = Vector2.zero; bgR.anchorMax = Vector2.one; bgR.sizeDelta = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/TooCubeForest/backgrounds/BG_mountains_sunset_1024.png");
        if (sp != null) { bgImg.sprite = sp; bgImg.type = Image.Type.Simple; } else { bgImg.color = new Color(0.1f, 0.1f, 0.2f, 1f); }

        GameObject p = new GameObject("ButtonsPanel"); p.transform.SetParent(canv.transform, false);
        RectTransform pR = p.AddComponent<RectTransform>(); pR.anchorMin = new Vector2(0.5f, 0.5f); pR.anchorMax = new Vector2(0.5f, 0.5f); pR.pivot = new Vector2(0.5f, 0.5f); pR.sizeDelta = new Vector2(300, 360); pR.anchoredPosition = Vector2.zero;
        p.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.65f);

        CreateUiText("Text_GameTitle", "PIXEL PLATFORMER", p.transform, new Vector2(0, 130), 24, new Color(1f, 0.75f, 0f, 1f));
        Button bCont = CreateUiButton("Btn_Continue", "Продолжить", p.transform, new Vector2(0, 50));
        Button bNew = CreateUiButton("Btn_NewGame", "Новая игра", p.transform, new Vector2(0, -10));
        Button bSet = CreateUiButton("Btn_Settings", "Настройки", p.transform, new Vector2(0, -70));
        Button bExit = CreateUiButton("Btn_Exit", "Выход", p.transform, new Vector2(0, -130));

        GameObject sP = new GameObject("SettingsPanel"); sP.transform.SetParent(canv.transform, false);
        RectTransform sR = sP.AddComponent<RectTransform>(); sR.anchorMin = new Vector2(0.5f, 0.5f); sR.anchorMax = new Vector2(0.5f, 0.5f); sR.pivot = new Vector2(0.5f, 0.5f); sR.sizeDelta = new Vector2(380, 380); sR.anchoredPosition = Vector2.zero;
        sP.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.90f);

        CreateUiText("Text_Title", "НАСТРОЙКИ", sP.transform, new Vector2(0, 130), 22, new Color(1f, 0.75f, 0f, 1f));
        CreateUiText("Text_Sound", "Звук: <color=#00d9ff>[ Слайдер ]</color>", sP.transform, new Vector2(0, 60), 16, Color.white);
        CreateUiText("Text_Control", "Управление: <color=#00d9ff>[ Клавиатура ]</color>", sP.transform, new Vector2(0, 10), 16, Color.white);
        CreateUiText("Text_Graphics", "Графика: <color=#00d9ff>[ Высокая ]</color>", sP.transform, new Vector2(0, -40), 16, Color.white);
        Button bBack = CreateUiButton("Btn_Back", "Назад", sP.transform, new Vector2(0, -110));

        mc.mainMenuPanel = p; mc.settingsPanel = sP; mc.continueButton = bCont; mc.newGameButton = bNew; mc.settingsButton = bSet; mc.exitButton = bExit; mc.backButton = bBack;
        sP.SetActive(false); EditorUtility.SetDirty(canv);
        if (!AssetDatabase.IsValidFolder("Assets/Generated_Game")) AssetDatabase.CreateFolder("Assets", "Generated_Game");
        if (!AssetDatabase.IsValidFolder("Assets/Generated_Game/Scenes")) AssetDatabase.CreateFolder("Assets/Generated_Game", "Scenes");
        EditorSceneManager.SaveScene(sc, "Assets/Generated_Game/Scenes/MainMenu.unity");
    }

    [MenuItem("Автоматизация Игры/2. Создать Игровой Уровень и Паузу")]
    public static void CreateGameplayLevel()
    {
        var sc = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        if (Camera.main != null) Camera.main.backgroundColor = new Color(0.2f, 0.4f, 0.6f, 1f);

        GameObject canv = new GameObject("HUDCanvas");
        Canvas c = canv.AddComponent<Canvas>(); c.renderMode = RenderMode.ScreenSpaceOverlay;
        canv.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canv.AddComponent<GraphicRaycaster>();

        if (GameObject.FindObjectOfType<EventSystem>() == null)
        {
            GameObject ev = new GameObject("EventSystem");
            ev.AddComponent<EventSystem>(); ev.AddComponent<StandaloneInputModule>();
        }

        PauseMenuController pc = canv.AddComponent<PauseMenuController>();
        GameObject pP = new GameObject("PauseMenuPanel"); pP.transform.SetParent(canv.transform, false);
        RectTransform pR = pP.AddComponent<RectTransform>(); pR.anchorMin = new Vector2(0.5f, 0.5f); pR.anchorMax = new Vector2(0.5f, 0.5f); pR.pivot = new Vector2(0.5f, 0.5f); pR.sizeDelta = new Vector2(320, 360); pR.anchoredPosition = Vector2.zero;
        pP.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 0.85f);

        CreateUiText("Text_PauseTitle", "ПАУЗА", pP.transform, new Vector2(0, 120), 22, new Color(1f, 0.75f, 0f, 1f));
        Button bRes = CreateUiButton("Btn_Resume", "Продолжить игру", pP.transform, new Vector2(0, 40));
        Button bSet = CreateUiButton("Btn_PauseSettings", "Настройки", pP.transform, new Vector2(0, -20));
        Button bToM = CreateUiButton("Btn_ToMenu", "Выйти в меню", pP.transform, new Vector2(0, -80));

        GameObject pS = new GameObject("PauseSettingsPanel"); pS.transform.SetParent(canv.transform, false);
        RectTransform psR = pS.AddComponent<RectTransform>(); psR.anchorMin = new Vector2(0.5f, 0.5f); psR.anchorMax = new Vector2(0.5f, 0.5f); psR.pivot = new Vector2(0.5f, 0.5f); psR.sizeDelta = new Vector2(360, 360); psR.anchoredPosition = Vector2.zero;
        pS.AddComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.95f);

        CreateUiText("Text_PSTitle", "НАСТРОЙКИ ПАУЗЫ", pS.transform, new Vector2(0, 120), 20, new Color(1f, 0.75f, 0f, 1f));
        CreateUiText("Text_PSSound", "Звук уровня: [ Слайдер ]", pS.transform, new Vector2(0, 50), 15, Color.white);
        CreateUiText("Text_PSGraph", "Графика уровня: [ Ок ]", pS.transform, new Vector2(0, -10), 15, Color.white);
        Button bBack = CreateUiButton("Btn_PauseBack", "Назад", pS.transform, new Vector2(0, -90));

        pc.pauseMenuPanel = pP; pc.settingsPanel = pS; pc.resumeButton = bRes; pc.settingsButton = bSet; pc.toMenuButton = bToM; pc.settingsBackButton = bBack;
        pP.SetActive(false); pS.SetActive(false);
        EditorUtility.SetDirty(canv); EditorSceneManager.SaveScene(sc, "Assets/Generated_Game/Scenes/GameplayLevel.unity");
        Debug.Log("Сцена уровня создана!");
    }

    private static Button CreateUiButton(string name, string txt, Transform parent, Vector2 pos)
    {
        GameObject bObj = new GameObject(name); bObj.transform.SetParent(parent, false);
        RectTransform r = bObj.AddComponent<RectTransform>(); r.anchorMin = new Vector2(0.5f, 0.5f); r.anchorMax = new Vector2(0.5f, 0.5f); r.pivot = new Vector2(0.5f, 0.5f); r.sizeDelta = new Vector2(220, 42); r.anchoredPosition = pos;
        bObj.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.25f, 1f);
        Button btn = bObj.AddComponent<Button>();
        ColorBlock cb = btn.colors; cb.normalColor = new Color(0.15f, 0.15f, 0.25f, 1f); cb.highlightedColor = new Color(1f, 0.75f, 0f, 1f); cb.pressedColor = new Color(0.7f, 0.5f, 0f, 1f); btn.colors = cb;
        GameObject tObj = new GameObject("Text"); tObj.transform.SetParent(bObj.transform, false);
        RectTransform tR = tObj.AddComponent<RectTransform>(); tR.anchorMin = Vector2.zero; tR.anchorMax = Vector2.one; tR.sizeDelta = Vector2.zero;
        Text t = tObj.AddComponent<Text>(); t.text = txt;
        Font[] f = Resources.FindObjectsOfTypeAll<Font>(); if (f.Length > 0) t.font = f[0];
        t.color = Color.white; t.alignment = TextAnchor.MiddleCenter; t.fontSize = 15;
        return btn;
    }

    private static void CreateUiText(string name, string content, Transform parent, Vector2 pos, int size, Color textColor)
    {
        GameObject tObj = new GameObject(name); tObj.transform.SetParent(parent, false);
        RectTransform r = tObj.AddComponent<RectTransform>(); r.anchorMin = new Vector2(0.5f, 0.5f); r.anchorMax = new Vector2(0.5f, 0.5f); r.pivot = new Vector2(0.5f, 0.5f); r.sizeDelta = new Vector2(340, 35); r.anchoredPosition = pos;
        Text t = tObj.AddComponent<Text>(); t.text = content; t.supportRichText = true;
        Font[] f = Resources.FindObjectsOfTypeAll<Font>(); if (f.Length > 0) t.font = f[0];
        textColor.a = 1f; t.color = textColor; t.alignment = TextAnchor.MiddleCenter; t.fontSize = size;
    }
}
