﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaChecker.Data.Interfaces;
using MangaChecker.Providers.Interfaces;

namespace MangaChecker.Providers.Sites {
    public class Mangahere : IProvider {
        public async Task CheckAll(Action<IManga> status) {
            throw new NotImplementedException();
        }

        public async Task<Tuple<List<object>, int>> GetImagesTaskAsync(string url) {
            throw new NotImplementedException();
        }

        public async Task<object> CheckOne(object manga) {
            throw new NotImplementedException();
        }

        public async Task<object> FindMangaInfoOnSite(string url) {
            throw new NotImplementedException();
        }

        public string DbName => "Mangahere";

        public bool LinkIsMatch(string link) {
            var regex = new Regex("");
            return regex.IsMatch(link);
        }

        public bool ViewEnabled => false;
        public string LinktoSite => "http://mangahere.co/";
    }
}