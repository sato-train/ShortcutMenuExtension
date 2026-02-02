# ショートカットメニュー拡張

Windowsのコンテキストメニュー（右クリックメニュー）に、フォルダ内のショートカット一覧を表示する拡張機能です。

## 機能

- フォルダを右クリックすると、そのフォルダ以下にあるショートカット（.lnk）ファイルの一覧が表示されます
- サブフォルダ内のショートカットも階層的に表示されます（最大3階層まで）
- メニューからショートカットを直接実行できます

## 必要な環境

- Windows 10/11（64bit）
- .NET Framework 4.7.2 以降
- Visual Studio 2017 以降（ビルドする場合）

## ビルド方法

### 1. NuGetパッケージの復元

Visual Studioでプロジェクトを開き、NuGetパッケージマネージャーで以下のパッケージをインストールします：

```
Install-Package SharpShell
```

### 2. 署名キーの生成

プロジェクトフォルダで以下のコマンドを実行：

```powershell
sn -k Key.snk
```

または、Visual Studioの「プロジェクトのプロパティ」→「署名」から新しいキーを作成できます。

### 3. ビルド

Visual Studioで「Release」構成を選択してビルドします。

### リポジトリ（GitHub）からクローンしてビルドする場合

このリポジトリには以下のファイルが含まれていません（`.gitignore` で除外しています）。

- **`packages/`** … NuGet パッケージ  
  → ビルド前に NuGet で復元してください。  
  - Visual Studio: ソリューションを開くと自動復元される場合があります。  
  - コマンドライン: `nuget restore ShortcutMenuExtension.csproj -PackagesDirectory packages`（[NuGet CLI](https://www.nuget.org/downloads) が必要）

- **`Key.snk`** … アセンブリ署名用のキーペア（秘密鍵を含むためリポジトリに含めていません）  
  → 署名付きでビルドする場合のみ、各自で生成してください。  
  - コマンド: `sn -k Key.snk`（Strong Name ツールは Visual Studio または Windows SDK に含まれます）  
  - プロジェクトの署名設定がコメントアウトされている場合は、`.csproj` 内の署名用 `PropertyGroup` と `Key.snk` の参照を有効にしてください。

署名なしのままでもビルド・動作は可能です。その場合は「2. 署名キーの生成」と `.csproj` の署名設定の変更は不要です。

## インストール方法

### 方法1: バッチファイルを使用（推奨）

1. プロジェクトをビルドします
2. `Install.bat` を**管理者権限で実行**します
3. エクスプローラーを再起動します（タスクマネージャーから）

### 方法2: 手動インストール

管理者権限のコマンドプロンプトで以下を実行：

```cmd
cd /d "プロジェクトのパス"
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe /codebase bin\Release\ShortcutMenuExtension.dll
```

## アンインストール方法

`Uninstall.bat` を**管理者権限で実行**します。

## 使用方法

1. エクスプローラーでフォルダを右クリック
2. 「📂 ショートカット一覧」メニュー項目が表示されます
3. サブメニューからショートカットを選択して実行できます

## 注意事項

- ショートカットが1つも見つからない場合は「ショートカットが見つかりません」と表示されます
- アクセス権限のないフォルダはスキップされます
- 階層の深さは3階層までに制限されています
- エクスプローラーの再起動が必要な場合があります

## トラブルシューティング

### メニューが表示されない場合

1. エクスプローラーを再起動
2. Windowsを再起動
3. 管理者権限でインストールしたか確認
4. イベントビューアーでエラーログを確認

### DLLの登録状態を確認

```cmd
%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe /regfile bin\Release\ShortcutMenuExtension.dll
```

## ライセンス

このプロジェクトはフリーソフトウェアです。自由に使用・改変できます。

## 使用ライブラリ

- [SharpShell](https://github.com/dwmkerr/sharpshell) - Windows Shell Extensions を .NET で開発するためのライブラリ
