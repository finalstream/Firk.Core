# Firk.Core

[![Build status](https://ci.appveyor.com/api/projects/status/2vwcpt684byufr1e?svg=true)](https://ci.appveyor.com/project/finalstream/firk-core)　[![NuGet](https://img.shields.io/nuget/v/Firk.Core.svg?style=plastic)](https://www.nuget.org/packages/Firk.Core/)　[![GitHub license](https://img.shields.io/github/license/finalstream/Firk.Core.svg)]()

Windowsアプリ開発をサポートするコアフレームワークです。  
フレームワークとしての使用方法等は[ExplorerWindowCleaner](https://github.com/finalstream/ExplorerWindowCleaner)を確認していただければと思います。


##主な機能

###Core
* AppClient(アプリケーションのクライント)
 * 設定ファイル(json)をサポート
 * スレッドセーフ処理実行機構(アクションを処理の単位として特定のスレッドで実行)
 * データベーススキーマアップグレード用アクション
* BackgroundWorker(バックグラウンド処理を実装を支援)
