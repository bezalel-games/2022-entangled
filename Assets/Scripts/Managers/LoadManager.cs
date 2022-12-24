using System;
using System.Collections;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
  #region Serialized Fields

  [SerializeField] private CanvasGroup _loadingCanvas;

  #endregion

  #region Non-Serialized Fields

  private static LoadManager _instance;
  private static readonly float _durationTime = 0.5f;
  private static readonly float _loadScreenTime = 0.5f;

  #endregion

  #region Properties

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
  }

  #endregion

  #region Public Methods

  public static void LoadScene(string name, LoadSceneMode mode, Action beforeFade = null, Action onLoad = null, Action afterFade = null)
  {
    _instance.StartCoroutine(_instance.LoadScene_Inner(
                              name, mode, _durationTime, _loadScreenTime, 
                              beforeFade, onLoad, afterFade));
  }
  
  public static void UnloadScene(string name, Action beforeFade = null, Action onLoad = null, Action afterFade = null)
  {
    _instance.StartCoroutine(_instance.LoadScene_Inner(
      name, LoadSceneMode.Single, _durationTime, _loadScreenTime, 
      beforeFade, onLoad, afterFade, true));
  }

  #endregion

  #region Private Methods

  private IEnumerator LoadScene_Inner(
    string name,
    LoadSceneMode mode, float fadeDuration = 0f, float loadScreenDuration = 0f,
    Action beforeFade = null, Action onLoad = null, Action afterFade = null, bool unload=false)
  {
    beforeFade?.Invoke();
    yield return null;

    yield return InterpolateLoadingAlpha(0, 1, fadeDuration);
    
    var loadTime = Time.time + loadScreenDuration;
    if (unload)
    {
      SceneManager.UnloadSceneAsync(name);
      while (Time.time < loadTime)
      {
        yield return new WaitForSeconds(0.1f);
      }
    }
    else
    {
      var scene = SceneManager.LoadSceneAsync(name, mode);
      while (!scene.isDone || Time.time < loadTime)
      {
        yield return new WaitForSeconds(0.1f);
      }
    }
    onLoad.Invoke();
    yield return null;
    
    yield return InterpolateLoadingAlpha(1, 0, fadeDuration);
    
    afterFade?.Invoke();
    yield return null;
  }

  private IEnumerator InterpolateLoadingAlpha(float from, float to, float duration)
  {
    float interpolationTime = Time.time + duration;
    
    _loadingCanvas.alpha = from;
    yield return null;
    
    float t = from;
    while (Time.time < interpolationTime)
    {
      t = 1 - (interpolationTime - Time.time) / duration;
      _loadingCanvas.alpha = (1 - t)*from + t*to;
      yield return null;
    }

    _loadingCanvas.alpha = to;
    yield return null;
  }

  #endregion
}

