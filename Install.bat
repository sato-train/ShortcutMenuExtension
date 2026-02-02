@echo off
echo ショートカットメニュー拡張のインストール
echo.
echo 管理者権限が必要です。
echo.
pause

set DLL_PATH=%~dp0bin\Release\ShortcutMenuExtension.dll
set REGASM=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe
set SERVER_MANAGER=%~dp0ServerRegistrationManager.exe

if not exist "%DLL_PATH%" (
    echo エラー: DLLファイルが見つかりません。
    echo パス: %DLL_PATH%
    echo.
    echo 先にVisual Studioでプロジェクトをビルドしてください。
    pause
    exit /b 1
)

echo DLLを登録しています...
"%REGASM%" /codebase "%DLL_PATH%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo インストールが完了しました！
    echo エクスプローラーを再起動してください。
    echo.
) else (
    echo.
    echo エラー: インストールに失敗しました。
    echo 管理者権限で実行していることを確認してください。
    echo.
)

pause
