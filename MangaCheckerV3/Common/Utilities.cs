﻿using System;
using System.Diagnostics;
using System.Windows;
using MangaChecker.Database.Tables;
using MangaChecker.Utilities;
using MangaCheckerV3.ViewModels.Window_ViewModels;
using MangaCheckerV3.Views.Windows;

namespace MangaCheckerV3.Common {
    public class Utilities {
        public static void OpenViewer(Manga manga) {
            var provider = ProviderService.Providers.Find(p => p.DbName == manga.Site);
            if (!provider.ViewEnabled) {
                try {
                    Process.Start(manga.Link);
                }
                catch (Exception e) {
                    Log.Loggger.Error($"{manga.Link}\n{e}");
                }
                return;
            }
            var viewer = new ViewerWindow {
                DataContext = new ViewerWindowViewModel(manga, provider),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = Application.Current.MainWindow
            };
            viewer.Show();
        }
    }
}