using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.Windows.Forms;

namespace ShortcutMenuExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    public class ShortcutContextMenu : SharpContextMenu
    {
        private const int MaxDepth = 3;
        private const int BaseCommandId = 1000;
        private Dictionary<int, string> commandToPathMap = new Dictionary<int, string>();
        private int currentCommandId = BaseCommandId;

        protected override bool CanShowMenu()
        {
            // 選択されたフォルダが存在するかチェック
            return SelectedItemPaths.Count() == 1 && Directory.Exists(SelectedItemPaths.First());
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            commandToPathMap.Clear();
            currentCommandId = BaseCommandId;

            string folderPath = SelectedItemPaths.First();

            try
            {
                // ショートカットファイルを検索
                var shortcuts = FindShortcuts(folderPath, 0);

                if (shortcuts.Count == 0)
                {
                    var noShortcutsItem = new ToolStripMenuItem("ショートカットが見つかりません");
                    noShortcutsItem.Enabled = false;
                    menu.Items.Add(noShortcutsItem);
                }
                else
                {
                    // メインメニュー項目
                    var mainItem = new ToolStripMenuItem("📂 ショートカット一覧")
                    {
                        Image = GetFolderIcon()
                    };

                    BuildMenuItems(mainItem.DropDownItems, shortcuts, folderPath);
                    menu.Items.Add(mainItem);
                }
            }
            catch (Exception ex)
            {
                var errorItem = new ToolStripMenuItem(string.Format("エラー: {0}", ex.Message));
                errorItem.Enabled = false;
                menu.Items.Add(errorItem);
            }

            return menu;
        }

        private List<ShortcutInfo> FindShortcuts(string directory, int depth)
        {
            var shortcuts = new List<ShortcutInfo>();

            if (depth >= MaxDepth || !Directory.Exists(directory))
                return shortcuts;

            try
            {
                // 現在のディレクトリのショートカットを追加
                var lnkFiles = Directory.GetFiles(directory, "*.lnk");
                foreach (var lnk in lnkFiles)
                {
                    shortcuts.Add(new ShortcutInfo
                    {
                        Path = lnk,
                        Name = Path.GetFileNameWithoutExtension(lnk),
                        Depth = depth,
                        ParentDirectory = directory
                    });
                }

                // サブディレクトリを検索
                var subdirectories = Directory.GetDirectories(directory);
                foreach (var subdir in subdirectories)
                {
                    var subdirShortcuts = FindShortcuts(subdir, depth + 1);
                    if (subdirShortcuts.Count > 0)
                    {
                        shortcuts.Add(new ShortcutInfo
                        {
                            Path = subdir,
                            Name = Path.GetFileName(subdir),
                            IsDirectory = true,
                            Depth = depth,
                            Children = subdirShortcuts
                        });
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // アクセス権限がない場合はスキップ
            }

            return shortcuts;
        }

        private void BuildMenuItems(ToolStripItemCollection menuItems, List<ShortcutInfo> shortcuts, string basePath)
        {
            foreach (var shortcut in shortcuts.OrderBy(s => s.IsDirectory ? 0 : 1).ThenBy(s => s.Name))
            {
                if (shortcut.IsDirectory)
                {
                    // フォルダの場合、サブメニューを作成
                    var folderItem = new ToolStripMenuItem(string.Format("📁 {0}", shortcut.Name));
                    BuildMenuItems(folderItem.DropDownItems, shortcut.Children, basePath);
                    menuItems.Add(folderItem);
                }
                else
                {
                    // ショートカットの場合
                    var shortcutItem = new ToolStripMenuItem(string.Format("🔗 {0}", shortcut.Name));
                    
                    int cmdId = currentCommandId++;
                    commandToPathMap[cmdId] = shortcut.Path;
                    shortcutItem.Tag = cmdId;
                    
                    shortcutItem.Click += (sender, e) =>
                    {
                        var item = sender as ToolStripMenuItem;
                        if (item != null && item.Tag is int)
                        {
                            var id = (int)item.Tag;
                            if (commandToPathMap.ContainsKey(id))
                                ExecuteShortcut(commandToPathMap[id]);
                        }
                    };

                    menuItems.Add(shortcutItem);
                }
            }
        }

        private void ExecuteShortcut(string shortcutPath)
        {
            try
            {
                System.Diagnostics.Process.Start(shortcutPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("ショートカットの実行に失敗しました:\n{0}", ex.Message), 
                    "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private const int MenuIconSize = 16;

        private System.Drawing.Image GetFolderIcon()
        {
            try
            {
                var icon = System.Drawing.SystemIcons.Application;
                var bmp = new System.Drawing.Bitmap(MenuIconSize, MenuIconSize);
                using (var g = System.Drawing.Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(icon.ToBitmap(), 0, 0, MenuIconSize, MenuIconSize);
                }
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        private class ShortcutInfo
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public bool IsDirectory { get; set; }
            public int Depth { get; set; }
            public string ParentDirectory { get; set; }
            public List<ShortcutInfo> Children { get; set; }

            public ShortcutInfo()
            {
                Children = new List<ShortcutInfo>();
            }
        }
    }
}
