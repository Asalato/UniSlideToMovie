using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スライド表示，ページ送り，ナレーション再生を行う
/// </summary>
public class Presenter : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private AudioSource source;
    
    private int _currentIndex;
    private SlideData[] _slide;

    /// <summary>
    /// スライド情報を設定し，表示を開始する
    /// </summary>
    /// <param name="data"></param>
    public void StartPresentation(SlideData[] data)
    {
        Debug.Log("[Presenter] Start presentation.");
        _slide = data;
        _currentIndex = -1;
    }

    private void Update()
    {
        if (_slide == null || source.isPlaying) return;
        if (_currentIndex >= _slide.Length - 1) return;
        Debug.Log("[Presenter] Next slide.");
        ++_currentIndex;
        image.texture = _slide[_currentIndex].image;
        source.clip = _slide[_currentIndex].clip;
        source.Play();
    }
}