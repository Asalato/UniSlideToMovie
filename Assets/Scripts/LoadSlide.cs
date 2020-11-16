using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Office.Core;
using UnityEngine;

/// <summary>
/// PowerPointプレゼンテーションファイルを読み込み，スライド画像を作成しノートを読み込む
/// </summary>
public class LoadSlide : MonoBehaviour
{
    [SerializeField] private string slidePath;
    [SerializeField] private string temporaryFilePath;

    private SlideDataRaw[] slides = null;

    /// <summary>
    /// スライドを読み込む
    /// </summary>
    /// <param name="action">読み込みが完了した際のアクション</param>
    public void Init(Action<SlideDataRaw[]> action)
    {
        if (string.IsNullOrEmpty(slidePath) || string.IsNullOrEmpty(temporaryFilePath))
        {
            Debug.LogError("[SlideLoader] Invalid file path.");
            return;
        }
        if (!File.Exists(slidePath) || !Directory.Exists(temporaryFilePath))
        {
            Debug.LogError("[SlideLoader] File not found.");
            return;
        }
        var extension = Path.GetExtension(slidePath);
        if (!(extension == ".pptx" || extension == ".ppt"))
        {
            Debug.LogError("[SlideLoader] File format must be \".ppt\" or \".pptx\"");
            return;
        }
        
        Debug.Log("[SlideLoader] Load Slide.");
        slides = null;
        Microsoft.Office.Interop.PowerPoint.Application app = null;
        Microsoft.Office.Interop.PowerPoint.Presentation ppt = null;

        Task.Run(() =>
        {
            try
            {
                app = new Microsoft.Office.Interop.PowerPoint.Application();

                // スライドを開く
                ppt = app.Presentations.Open(slidePath, MsoTriState.msoTrue, MsoTriState.msoFalse,
                    MsoTriState.msoFalse);

                var width = (int) ppt.PageSetup.SlideWidth;
                var height = (int) ppt.PageSetup.SlideHeight;

                var slideList = new List<SlideDataRaw>();

                for (var i = 1; i <= ppt.Slides.Count; i++)
                {
                    // 非表示スライドは無視
                    if (ppt.Slides[i].SlideShowTransition.Hidden == MsoTriState.msoTrue) continue;

                    // コメント
                    var note = ppt.Slides[i].NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text;
                    if (note == "") continue;

                    // JPEGとして保存
                    var file = temporaryFilePath + $"/slide{i:0000}.jpg";
                    ppt.Slides[i].Export(file, "jpg", width, height);

                    slideList.Add(new SlideDataRaw() {note = note, imageFilePath = file});
                }

                slides = slideList.ToArray();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                ppt?.Close();
                app?.Quit();
            }
        }).ContinueWith(_ =>
        {
            Debug.Log("[SlideLoader] Load completed.");
            action.Invoke(slides);
        });
    }
}
