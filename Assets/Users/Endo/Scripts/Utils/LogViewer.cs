using UnityEngine;
using UnityEngine.UI;

public class LogViewer : MonoBehaviour
{
    [SerializeField]
    private GameObject text;

    [SerializeField]
    private ScrollRect DebugWindow;

    public int logcnt = 0;

    private Text _text;

    private void Awake()
    {
        _text                          =  text.GetComponent<Text>();
    }

    private void OnEnable()
    {
        Application.logMessageReceived += LoggedCb;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= LoggedCb;
    }

    public void LoggedCb(string logstr, string stacktrace, LogType type)
    {
        if (_text == null)
        {
            Debug.Log(text);
            _text = text.GetComponent<Text>();
        }

        if (logcnt > 200)
        {
            int index = _text.text.IndexOf("\n");
            _text.text = _text.text.Substring(index + 1);
        }
        else
        {
            logcnt++;
        }

        _text.text += logstr;
        _text.text += "\n";
        // 常にTextの最下部（最新）を表示するように強制スクロール
        DebugWindow.verticalNormalizedPosition = 0;
    }
}
