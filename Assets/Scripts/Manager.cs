using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// 処理の進行を制御
/// </summary>
/// <remarks>
/// Start関数で初期化から各リクエストをすべて実行
/// </remarks>>
public class Manager : MonoBehaviour
{
    [SerializeField] private LoadSlide loadSlide;
    [SerializeField] private GenerateAudio generateAudio;
    [SerializeField] private Presenter presenter;

    private const int SEPARATE_THRESHOLD = 150;

    private SynchronizationContext _context;

    private void Start()
    {
        _context = SynchronizationContext.Current;
        generateAudio.Init();
        loadSlide.Init(d => _context.Post(_ =>
        {
            // スライドの読み込みは非同期に行っているので，ここで同期処理に戻す
            // AudioClipとTexture2Dの作成はメインスレッドでしかできないので
            Debug.Log("Convert data.");
            var slide = ConvertRawData(d);
            Debug.Log($"Total {slide.Length} slides load completed.");
            presenter.StartPresentation(slide);
        }, null));
    }

    /// <summary>
    /// 画像のパスとテキストからなるスライド情報からTexture2DとAudioClipを取得する
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private SlideData[] ConvertRawData(IEnumerable<SlideDataRaw> data)
        => data.Select(item => new SlideData()
            {image = LoadImage(item.imageFilePath), clip = CreateAudio(item.note)}).ToArray();

    /// <summary>
    /// テキストからAudioClipを作成
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private AudioClip CreateAudio(string text)
    {
        // 長いテキストを分割
        var wavs = new List<WAV>();
        var sb = new StringBuilder();
        foreach (var cha in text)
        {
            sb.Append(cha);
            if ((cha != '．' && cha != '。' && cha != '\n') || sb.Length <= SEPARATE_THRESHOLD) continue;
            wavs.Add(new WAV(generateAudio.GenerateBinary(sb.ToString())));
            sb.Clear();
        }

        if (sb.Length > 1) wavs.Add(new WAV(generateAudio.GenerateBinary(sb.ToString())));

        // 分割された文の合成
        var result = WAV.Combine(wavs.ToArray());
        return result.CreateAudioClip("Audio");
    }

    /// <summary>
    /// 画像を読み込み
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static Texture2D LoadImage(string path)
    {
        Debug.Log($"[LoadImage] Load image from {path}");
        byte[] binary;
        try
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var length = (int) fs.Length;
                binary = new byte[length];
                fs.Read(binary, 0, length);
                fs.Close();
            }
        }
        catch(IOException exception)
        {
            Debug.Log(exception);
            return null;
        }
        
        var texture = new Texture2D(0, 0);
        texture.LoadImage(binary);
        return texture;
    }
}