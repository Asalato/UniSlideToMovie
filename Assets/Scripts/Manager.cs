using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

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
            Debug.Log("Convert data.");
            var slide = ConvertRawData(d);
            Debug.Log($"Total {slide.Length} slides load completed.");
            presenter.StartPresentation(slide);
        }, null));
    }

    private SlideData[] ConvertRawData(IEnumerable<SlideDataRaw> data)
        => data.Select(item => new SlideData()
            {image = LoadImage(item.imageFilePath), clip = CreateAudio(item.note)}).ToArray();

    private AudioClip CreateAudio(string text)
    {
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

        var result = WAV.Combine(wavs.ToArray());
        return result.CreateAudioClip("Audio");
    }

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