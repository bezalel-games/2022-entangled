using Rooms;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    #region Non-Serialized Fields

    private static MenuManager _instance;
    private CharacterMap _controls;

    #endregion

    #region Function Events

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        _controls = new CharacterMap();
        var controller = GetComponent<MenuController>();
        _controls.Menu.SetCallbacks(controller);
        _controls.Menu.Enable();
    }
    
    private void OnDestroy()
    {
        _controls.Menu.Disable();
        _controls.Menu.SetCallbacks(null);
    }

    #endregion

    #region Public Methods

    public static void StartRun()
    {
        SceneManager.LoadScene("Run Scene");
    }

    public static void StartEndless()
    {
        SceneManager.LoadScene("Endless Scene");
    }
    
    public static void StartBoss()
    {
        SceneManager.LoadScene("Boss Scene");
    }

    #endregion
}