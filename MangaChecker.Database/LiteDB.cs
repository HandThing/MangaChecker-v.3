﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using MangaChecker.Database.Enums;
using MangaChecker.Database.Tables;

namespace MangaChecker.Database {
    public static class LiteDB {
        private const string DatabaseVersion = "1.0.0.2";
        private static readonly string DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "mcv3.db");

        private static readonly LiteDatabase _db = new LiteDatabase(DatabasePath);

        private static readonly Dictionary<string, string> DefaultDatabaseSettings = new Dictionary<string, string> {
            {"Mangafox", "http://mangafox.me/"},
            {"Mangahere", "http://mangahere.co/"},
            {"Mangareader", "http://www.mangareader.net/"},
            {"Mangastream", "http://mangastream.com/"},
            {"Batoto", "http://bato.to/"},
            {"Webtoons", "http://www.webtoons.com/"},
            {"YoManga", "http://yomanga.co/"},
            {"Kissmanga", "http://kissmanga.com/"},
            {"GameOfScanlation", "https://gameofscanlation.moe/"},
            {"KireiCake", "http://kireicake.com/"},
            {"Jaiminisbox", "https://jaiminisbox.com/"},
            {"HeyManga", "https://www.heymanga.me/"},
            {"Tomochan", "http://read.tomochan.today/"},
            {"Crunchyroll", "http://www.crunchyroll.com/comics/manga"}
        };

        private static readonly Dictionary<string, string> DefaultVersions = new Dictionary<string, string> {
            {"db", "1.0.0.0"}
        };

        static LiteDB() {
        }

        public static event EventHandler<MangaEnum> MangaEvent; //switch to this maybe?
        public static event EventHandler<DatabaseEnum> DbEvent; //switch to this maybe?
        public static event EventHandler<SettingEnum> SettingEvent; //switch to this maybe?

        public static IOrderedEnumerable<Manga> GetHistory() {
            var query = _db.GetCollection<Manga>("History").FindAll();
            var ordered = query.OrderBy(m => m.Updated);
            MangaEvent?.Invoke(ordered, MangaEnum.GetHistory);
            return ordered;
        }

        public static IEnumerable<Manga> GetAllMangas() {
            var query = _db.GetCollection<Manga>("Manga").FindAll();
            MangaEvent?.Invoke(query, MangaEnum.Get);
            return query;
        }

        public static IOrderedEnumerable<Manga> GetAllNewMangas() {
            var query = _db.GetCollection<Manga>("Manga").Find(n => n.New);
            var ordered = query.OrderByDescending(m => m.Updated);
            MangaEvent?.Invoke(ordered, MangaEnum.New);
            return ordered;
        }

        public static IEnumerable<Manga> GetMangasFrom(string site) {
            var query = _db.GetCollection<Manga>("Manga").Find(s => s.Site == site);
            MangaEvent?.Invoke(query, MangaEnum.Get);
            return query;
        }

        public static void InsertManga(Manga manga) {
            var query = _db.GetCollection<Manga>("Manga");
            query.Insert(manga);
            MangaEvent?.Invoke(manga, MangaEnum.Insert);
        }

        public static void InsertHistory(Manga manga) {
            var query = _db.GetCollection<Manga>("History");
            query.Insert(manga);
            MangaEvent?.Invoke(manga, MangaEnum.InsertHistory);
        }

        public static void Update(Manga manga, bool history = false) {
            var query = _db.GetCollection<Manga>("Manga");
            query.Update(manga);
            if (history) return;
            MangaEvent?.Invoke(manga, MangaEnum.Update);
        }

        public static void UpdateTrans(List<Manga> manga, bool history = false) {
            using (var _db = new LiteDatabase(DatabasePath)) {
                var query = _db.GetCollection<Manga>("Manga");
                using (var trans1 = _db.BeginTrans()) {
                    query.Update(manga);
                    trans1.Commit();
                }
            }
            if (history) return;
            MangaEvent?.Invoke(manga, MangaEnum.Update);
        }

        public static void Delete(Manga manga) {
            var query = _db.GetCollection<Manga>("Manga");
            query.Delete(manga.MangaId);
            MangaEvent?.Invoke(manga, MangaEnum.Delete);
        }

        public static void DeleteHistory(Manga manga) {
            var query = _db.GetCollection<Manga>("History");
            query.Delete(manga.MangaId);
            MangaEvent?.Invoke(manga, MangaEnum.DeleteHistory);
        }

        public static List<Settings> GetAllSettings() {
            var query = _db.GetCollection<Settings>("Settings").FindAll().ToList();
            SettingEvent?.Invoke(query, SettingEnum.Get);
            return query;
        }

        public static Settings GetSettingsFor(string setting) {
            var query = _db.GetCollection<Settings>("Settings").FindOne(s => s.Setting == setting);
            SettingEvent?.Invoke(query, SettingEnum.Get);
            return query;
        }

        public static void SaveSettings(List<Settings> settings) {
            var query = _db.GetCollection<Settings>("Settings");
            using (var trans1 = _db.BeginTrans()) {
                query.Update(settings);
                trans1.Commit();
            }
            SettingEvent?.Invoke(settings, SettingEnum.Update);
        }

        public static int GetRefreshTime() {
            var query = _db.GetCollection<Settings>("Settings").FindOne(s => s.Setting == "Refresh Time");
            SettingEvent?.Invoke(query, SettingEnum.Refresh);
            return query.Active;
        }

        public static bool GetOpenLinks() {
            var query = _db.GetCollection<Settings>("Settings").FindOne(s => s.Setting == "Open Links");
            SettingEvent?.Invoke(query, SettingEnum.Refresh);
            return query.OpenLinks;
        }

        private static void UpdateDatabase(Versions dbv) {
            var set = _db.GetCollection<Settings>("Settings");
            _db.GetCollection<Manga>("History");
            _db.GetCollection<Manga>("Manga");
            var ver = _db.GetCollection<Versions>("Versions");

            var setting = set.FindAll().Select(s => s.Setting).ToArray();
            var versions = ver.FindAll().Select(v => v.Name).ToArray();

            foreach (var defaultSetting in DefaultDatabaseSettings)
                if (!setting.Contains(defaultSetting.Key))
                    set.Insert(new Settings {
                        Setting = defaultSetting.Key,
                        Link = defaultSetting.Value,
                        Active = 0,
                        Created = DateTime.Now
                        //OpenLinks = true
                    });
            if (!setting.Contains("Refresh Time"))
                set.Insert(new Settings {
                    Setting = "Refresh Time",
                    Link = "/",
                    Active = 300,
                    Created = DateTime.Now
                });
            if (!setting.Contains("Open Links"))
                set.Insert(new Settings {
                    Setting = "Open Links",
                    Link = "/",
                    Active = 0,
                    Created = DateTime.Now
                });
            if (!setting.Contains("Batoto Rss"))
                set.Insert(new Settings {
                    Setting = "Batoto Rss",
                    Link = "/",
                    Active = 0,
                    Created = DateTime.Now
                });
            foreach (var defaultver in DefaultVersions)
                if (!versions.Contains(defaultver.Key))
                    ver.Insert(new Versions {
                        Name = defaultver.Key,
                        Version = defaultver.Key
                    });
            ver.Update(dbv);
            DbEvent?.Invoke(dbv, DatabaseEnum.Update);
        }

        public static void CreateDatabase() {
            var set = _db.GetCollection<Settings>("Settings");
            _db.GetCollection<Manga>("Manga");
            _db.GetCollection<Manga>("History");
            var ver = _db.GetCollection<Versions>("Versions");

            ver.Insert(new Versions {
                Name = "db",
                Version = DatabaseVersion
            });

            foreach (var sites in DefaultDatabaseSettings)
                set.Insert(new Settings {
                    Setting = sites.Key,
                    Link = sites.Value,
                    Active = 0,
                    Created = DateTime.Now
                });
            if (set.FindOne(s => s.Setting == "Refresh Time") == null)
                set.Insert(new Settings {
                    Setting = "Refresh Time",
                    Link = "/",
                    Active = 300,
                    Created = DateTime.Now
                });
            if (set.FindOne(s => s.Setting == "Open Links") == null)
                set.Insert(new Settings {
                    Setting = "Open Links",
                    Link = "/",
                    Active = 300,
                    Created = DateTime.Now
                });
            if (set.FindOne(s => s.Setting == "Batoto Rss") == null)
                set.Insert(new Settings {
                    Setting = "Batoto Rss",
                    Link = "/",
                    Active = 0,
                    Created = DateTime.Now
                });
            DbEvent?.Invoke(null, DatabaseEnum.Create);
        }

        public static string CheckDbVersion() {
            var dbv = _db.GetCollection<Versions>("Versions").FindOne(v => v.Name == "db");
            if (dbv.Version == DatabaseVersion) return null;
            dbv.Version = DatabaseVersion;
            UpdateDatabase(dbv);
            return $"Updated Database to {DatabaseVersion}";
        }
    }
}

//http://stackoverflow.com/questions/1249517/super-simple-example-of-c-sharp-observer-observable-with-delegates

//class Observable {
//    public event EventHandler SomethingHappened;
//    public static event EventHandler SomethingHappened; //switch to this maybe?

//    public void DoSomething() {
//        EventHandler handler = SomethingHappened;
//        if (handler != null) {
//            handler(this, EventArgs.Empty);
//        }
//    }
//}

//class Observer {
//    public void HandleEvent(object sender, EventArgs args) {
//        Console.WriteLine("Something happened to " + sender);
//    }
//}

//class Test {
//    static void Main() {
//        Observable observable = new Observable();
//        Observer observer = new Observer();
//        observable.SomethingHappened += observer.HandleEvent;

//        observable.DoSomething();
//    }
//}