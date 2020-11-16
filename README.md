# UniSlideToMovie
Unityを利用し，PowerPointのスライドデータを読み上げ音声付き動画ファイルに変換するサンプルです．
実装の詳細は[記事](https://qiita.com/Asalato/items/01321b3a45f0cbbc0ac3)を確認ください．

#### 注意事項

- 2020/11/16時点でβ版であり，現時点ではスライドの読み込みと音声合成が自動再生される機能が実装されています．
- Unity Editorで実行すると自動的に動画の再生が開始されるので，適宜[Unity Recorder](https://docs.unity3d.com/Packages/com.unity.recorder@2.0/manual/index.html)等を利用し録画してください．

## 使い方
- [記事](https://qiita.com/Asalato/items/01321b3a45f0cbbc0ac3)を参考にWatson Text To SpeechのAPIキー及びリクエストURLを取得
- HierarchyビューのManagerコンポーネントを参照し，`GenerateAudio`の`Api Key`および`Url`にWatsonで取得した値を設定
- `LoadSlide`の`Slide Path`にスライドの絶対パス（拡張子付き）を，`Temporary File Path`に一時保存ファイル（詳細は記事参照）のパスを設定
- Playボタンを押し，スライドと音声合成のリクエストの後自動でナレーションの再生とスライド送りが行われることを確認する

## 実装予定

- UnityRecorderの自動起動・停止
- VRMモデルを利用し，アバターに喋らせる

## 利用ライブラリ

- [Microsoft.Office.Interop.PowerPoint](https://www.nuget.org/packages/Microsoft.Office.Interop.PowerPoint/)
- [ThammimTech.Microsoft.Vbe.Interop](https://www.nuget.org/packages/ThammimTech.Microsoft.Vbe.Interop/)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
- [Office](https://www.nuget.org/packages/Office/)
- [stdole](https://www.nuget.org/packages/stdole/)

- [Watson Text To Speech](https://www.ibm.com/jp-ja/cloud/watson-text-to-speech)
