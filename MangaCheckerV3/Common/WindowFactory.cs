﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MangaChecker.Data.Interfaces;
using MangaChecker.Data.Models;
using MangaChecker.Providers.Interfaces;
using MangaChecker.Utilities;
using MangaChecker.ViewModels.ViewModels.Window_ViewModels;
using MangaCheckerV3.Views.Windows;

namespace MangaCheckerV3.Common {
    public class WindowFactory : IWindowFactory {
        private readonly IProviderService _providerService;
        private readonly ILiteDb _liteDb;
        private readonly Logger _logger;
        public WindowFactory(Logger logger, IProviderService providerService, ILiteDb liteDb) {
            _logger = logger;
            _providerService = providerService;
            _liteDb = liteDb;
        }

        public void CreateViewerWindow(IManga manga) => _createViewer(manga);
        public void CreateViewerWindow(IManga manga, object provider) => _createViewer(manga, false, provider);
        public void CreateEditWindow(Manga manga) => new EditWindow {
            DataContext = new EditWindowViewModel(manga, _providerService, _liteDb),
        }.Show();

        private void _createViewer(IManga manga, bool saveEnabled = false, object provider = null) {
            var p = (IProvider) provider ?? _providerService.Providers.Find(x => x.DbName == manga.Site);
            if (!p.ViewEnabled) {
                try {
                    Process.Start(manga.Link);
                } catch (Exception e) {
                    _logger.Log.Error($"{manga.Link}\n{e}");
                }
                return;
            }
            var viewerWindow = new ViewerWindow {
                DataContext = new ViewerWindowViewModel(manga, p, saveEnabled),
            };
            viewerWindow.Show();
        }


    }
}
