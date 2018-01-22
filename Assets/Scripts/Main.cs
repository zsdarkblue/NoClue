using UnityEngine;
using BeatsFever;

public class Main : MonoBehaviour
{
    private const float ForceQuitDuration = 5f;
    private bool errorReportSent;


    private void Awake()
    {
		Application.targetFrameRate = 300;
		Application.runInBackground = true;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

		MultilanguageMgr.InitMultiConf();

        try
        {
            if (App.Instance == null)
            {
                App.Create(gameObject);
                App.Instance.Start();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("App Start Exception: " + ex.Message + " " + ex.StackTrace);
        }
    }

    private void Update()
    {
		App.Instance.Update();

//        try
//        {
//            App.Instance.Update();
//        }
//        catch (System.Exception ex)
//        {
//            string errorInfo = "App Update Exception: " + ex.Message + " " + ex.StackTrace;
//            Debug.LogError(errorInfo);
//
//            Invoke("ForceQuitOnError", ForceQuitDuration);
//        }
    }

    private void LateUpdate()
    {
        try
        {
            App.Instance.LateUpdate();
        }
        catch (System.Exception ex)
        {
            string errorInfo = "App LateUpdate Exception: " + ex.Message + " " + ex.StackTrace;
            Debug.LogError(errorInfo);

            if (!errorReportSent)
            {
                errorReportSent = true;
                Invoke("ForceQuitOnError", ForceQuitDuration);
            }
        }
    }


    private void ForceQuitOnError()
    {
        Application.Quit();
    }

    void OnApplicationQuit() 
    {
        App.Destroy();
        PlayerPrefs.Save();
    }
}
