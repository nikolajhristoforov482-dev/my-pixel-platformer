using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Панели")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;

    [Header("Кнопки")]
    public Button continueButton;
    public Button newGameButton;
    public Button settingsButton;
    public Button exitButton;
    public Button backButton;

    void Start()
    {
        // Если нет сохранений, блокируем кнопку "Продолжить"
        if (!PlayerPrefs.HasKey("SavedLevel"))
        {
            continueButton.interactable = false;
            continueButton.GetComponentInChildren<Text>().color = new Color(1f, 1f, 1f, 0.3f);
        }

        // Привязываем функции к кнопкам
        continueButton.onClick.AddListener(ClickContinue);
        newGameButton.onClick.AddListener(ClickNewGame);
        settingsButton.onClick.AddListener(ClickSettings);
        exitButton.onClick.AddListener(ClickExit);
        backButton.onClick.AddListener(ClickBack);
    }

    void ClickContinue()
    {
        // Загружаем сохраненный уровень (если его нет, загрузится GameplayLevel)
        string savedLevel = PlayerPrefs.GetString("SavedLevel", "GameplayLevel");
        SceneManager.LoadScene(savedLevel);
    }

    void ClickNewGame()
    {
        // Сбрасываем старые сохранения и загружаем уровень с нуля
        PlayerPrefs.DeleteKey("SavedLevel");
        SceneManager.LoadScene("GameplayLevel");
    }

    void ClickSettings()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    void ClickBack()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void ClickExit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
