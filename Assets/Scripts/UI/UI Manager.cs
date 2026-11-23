using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField] private GameObject uiGameObject;   // panel toggled by the UIcontrol input action
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("HUD")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text weaponText;

    private PlayerControls controls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        if (Instance != this) return;

        controls.Gameplay.Enable();
        controls.Gameplay.UIcontrol.performed += OnUIControlPerformed;

        GameEvents.OnScoreChanged += UpdateScore;
        GameEvents.OnPlayerHealthChanged += UpdateHealth;
        GameEvents.OnGameStateChanged += HandleGameStateChanged;
    }

    private void OnDisable()
    {
        if (Instance != this) return;

        controls.Gameplay.UIcontrol.performed -= OnUIControlPerformed;
        controls.Gameplay.Disable();

        GameEvents.OnScoreChanged -= UpdateScore;
        GameEvents.OnPlayerHealthChanged -= UpdateHealth;
        GameEvents.OnGameStateChanged -= HandleGameStateChanged;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            controls?.Dispose();
        }
    }

    private void Start()
    {
        SetActiveSafe(pausePanel, false);
        SetActiveSafe(gameOverPanel, false);
    }

    private void Update()
    {
        // Pause toggle (new Input System, no extra action needed in the asset)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TogglePause();
            }
        }
    }


    private void OnUIControlPerformed(InputAction.CallbackContext ctx)
    {
        if (uiGameObject == null) return;

        // Only allow opening/closing the panel while actually playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) return;

        uiGameObject.SetActive(!uiGameObject.activeSelf);
    }


    private void UpdateScore(int totalScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {totalScore}";
        }
    }

    private void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(currentHealth)} / {Mathf.CeilToInt(maxHealth)}";
        }
    }

    public void SetCurrentWeapon(string weaponName)
    {
        if (weaponText != null)
        {
            weaponText.text = weaponName;
        }
    }

    private void HandleGameStateChanged(GameState state)
    {
        SetActiveSafe(pausePanel, state == GameState.Paused);
        SetActiveSafe(gameOverPanel, state == GameState.GameOver);
    }

    private static void SetActiveSafe(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }
}