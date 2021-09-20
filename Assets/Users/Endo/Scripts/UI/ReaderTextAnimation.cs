using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ReaderTextAnimation : MonoBehaviour
{
    private TextMeshProUGUI _textMesh;
    private bool            _isAnimate;

    private void Start()
    {
        _textMesh      = GetComponent<TextMeshProUGUI>();
        _textMesh.text = "...";
    }

    private void OnEnable()
    {
        _isAnimate = true;

        Animate();
    }

    private void OnDisable()
    {
        _isAnimate = false;
    }

    /// <summary>
    /// 三点リーダーアニメーショを行う
    /// </summary>
    private async void Animate()
    {
        // TMPが読み込まれるまで待機
        if (_textMesh == null)
        {
            await UniTask.WaitUntil(() => _textMesh != null);
        }

        _textMesh.firstVisibleCharacter = 3;

        while (true)
        {
            if (!_isAnimate) break;

            _textMesh.alignment = TextAlignmentOptions.Left;

            // 三点リーダー展開
            for (int i = 0; i < _textMesh.text.Length; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(.5));

                _textMesh.firstVisibleCharacter--;
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1));

            _textMesh.alignment = TextAlignmentOptions.Right;

            // 三点リーダー収縮
            for (int i = 0; i < _textMesh.text.Length; i++)
            {
                _textMesh.firstVisibleCharacter++;

                if (i == _textMesh.text.Length - 1) break;

                await UniTask.Delay(TimeSpan.FromSeconds(.5));
            }

            await UniTask.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
