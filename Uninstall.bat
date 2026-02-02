@echo off
echo ショートカットメニュー拡張のアンインストール
echo.
echo 管理者権限が必要です。
echo.
pause

set DLL_PATH=%~dp0bin\Release\ShortcutMenuExtension.dll
set REGASM=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\RegAsm.exe

if not exist "%DLL_PATH%" (
    echo エラー: DLLファイルが見つかりません。
    echo パス: %DLL_PATH%
    pause
    exit /b 1
)

echo DLLの登録を解除しています...
"%REGASM%" /unregister "%DLL_PATH%"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo アンインストールが完了しました！
    echo エクスプローラーを再起動してください。
    echo.
) else (
    echo.
    echo エラー: アンインストールに失敗しました。
    echo.
)

pause
