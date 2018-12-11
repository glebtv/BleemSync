﻿using BleemSync.Data;
using BleemSync.Data.Models;
using BleemSync.Services;
using BleemSync.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BleemSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var gameService = new GameService();

            var gameIds = Filesystem.GetGameIds();

            using (var db = new DatabaseContext())
            {
                foreach (var existingGame in db.Games)
                {
                    db.Remove(existingGame);
                }

                foreach (var existingDisc in db.Discs)
                {
                    db.Remove(existingDisc);
                }

                db.SaveChanges();

                var infos = gameIds.Select(id => gameService.GetGameInfo(id));

                foreach (var info in infos) {
                    var game = new Game()
                    {
                        Id = info.Id,
                        Title = info.Title,
                        Publisher = info.Publisher,
                        Year = info.Year,
                        Players = info.Players
                    };

                    game.Discs = info.DiscIds.Select((discId, index) => new Disc() { GameId = info.Id, DiscNumber = index + 1, DiscBasename = discId}).ToList();

                    db.Add(game);
                }

                db.SaveChanges();
            }
        }
    }
}
