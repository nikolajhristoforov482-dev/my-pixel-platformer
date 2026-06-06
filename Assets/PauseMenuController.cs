using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Панели паузы")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    [Header("Кнопки")]
    public Button resumeButton;
    public Button settingsButton;
    public Button toMenuButton;
    public Button settingsBackButton;

    private bool isPaused = false;

    void Start()
    {
        // Привязываем действия к кнопкам паузы
        resumeButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        toMenuButton.onClick.AddListener(QuitToMainMenu);
        settingsBackButton.onClick.AddListener(CloseSettings);
    }

    void Update()
    {
        // Если игрок нажал клавишу ESC (Escape)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        pauseMenuPanel.SetActive(true);
        settingsPanel.SetActive(false);
        Time.timeScale = 0f; // Полностью останавливаем время и физику в игре!
    }

    public void ResumeGame()
    {
        isPaused = false;
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(false);
        Time.timeScale = 1f; // Возвращаем нормальный ход времени
    }

    void OpenSettings()
    {
        pauseMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuPanel.SetActive(true);
    }

    void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Обязательно включаем время обратно перед выходом!
        // Сохраняем имя текущего уровня, чтобы кнопка "Продолжить" в меню стала активной
        PlayerPrefs.SetString("SavedLevel", SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("MainMenu"); // Возвращаемся в Главное меню
    }
}