﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaChecker.DataTypes.Interface;

namespace MangaChecker.Providers {
    public class Mangahere : ISite {
        public async Task CheckAll() {
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

        public Regex LinkRegex() {
            return new Regex("");
        }

        public bool ViewEnabled => false;
        public string LinktoSite => "http://mangahere.co/";
    }
}