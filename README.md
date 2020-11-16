# UniSlideToMovie
Unityを利用し，PowerPointのスライドデータを読み上げ音声付き動画ファイルに変換するサンプルです．

#### 注意事項

- 2020/11/16時点でβ版であり，現時点ではスライドの読み込みと音声合成が自動再生される機能が実装されています．
- Unity Editorで実行すると自動的に動画の再生が開始されるので，適宜[Unity Recorder](https://docs.unity3d.com/Packages/com.unity.recorder@2.0/manual/index.html)等を利用し録画してください．

## 実装予定

- UnityRecorderの自動起動・停止
- VRMモデルを利用し，アバターに喋らせる

## 利用ライブラリ

- [Microsoft.Office.Interop.PowerPoint](https://www.nuget.org/packages/Microsoft.Office.Interop.PowerPoint/)
- [ThammimTech.Microsoft.Vbe.Interop](https://www.nuget.org/packages/ThammimTech.Microsoft.Vbe.Interop/)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/)
- [Office](https://www.nuget.org/packages/Office/)
- [stdole](https://www.nuget.org/packages/stdole/)
