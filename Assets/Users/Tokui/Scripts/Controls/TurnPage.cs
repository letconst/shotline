using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SwipeGesture), typeof(HorizontalLayoutGroup))]
public class TurnPage : MonoBehaviour
{
    public float PageWidth = 2048;

    private RectTransform rectTransform;
    private SwipeGesture swipeGesture;
    private int pageCount;
    private int currentPage = 1;

    void OnEnable()
    {
        this.rectTransform = this.GetComponent<RectTransform>();
        this.swipeGesture = this.GetComponent<SwipeGesture>();
        this.pageCount = this.transform.childCount;

        // next
        this.swipeGesture
            .OnSwipeLeft
            .Where(_ => currentPage < pageCount) // 最大ページ以前である場合のみ進める
            .Subscribe(_ =>
            {
                this.currentPage++;
            });

        // back
        this.swipeGesture
            .OnSwipeRight
            .Where(_ => currentPage > 1) // 1ページ目以降である場合のみ戻れる
            .Subscribe(_ =>
            {
                this.currentPage--;
            });
    }
}
