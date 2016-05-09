using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Statki.Models;
using Statki.ViewModel;
using State = Statki.Models.State;

namespace Statki.Controllers
{
    public class GameController : Controller
    {
        private int _x = 6;
        private int _y = 6;
        private IList<PlayerViewModel> Players { get; set; }

        [HttpPost]
        public ActionResult ShowMap(MapViewModel model)
        {
            using (var db = new BattleShipContext())
            {
                var viewmodel = new List<MapViewModel>();
                bool isHit = false;
                var state =
                    db.Fields.FirstOrDefault(x => x.PlayerId == model.IdOpponent && x.X == model.ShotX && x.Y == model.ShotY);
                if (state.State == State.Statek)
                {
                    db.Fields
                    .Single(x => x.PlayerId == model.IdOpponent && x.X == model.ShotX && x.Y == model.ShotY)
                    .State=State.Zatopiony;
                    isHit = true;
                }
                if (state.State == State.Puste)
                {
                    db.Fields
                    .Single(x => x.PlayerId == model.IdOpponent && x.X == model.ShotX && x.Y == model.ShotY)
                    .State = State.Pudlo;
                }

                db.SaveChanges();

                var players =
                    db.Players.ToList();

                foreach (var player in players)
                {
                    var fields = db.Fields.Where(x => x.PlayerId == player.Id).ToList();
                    var fieldsVieModel = Mapper.Map<IList<Field>, IList<FieldViewModel>>(fields);
                    viewmodel.Add(new MapViewModel { LengthMap = _x, HighMap = _y, IdOpponent = players.First(x => x.Id != player.Id).Id, NamePlayer = player.Name, IdPlayer = player.Id, Fields = fieldsVieModel });
                }

                foreach (var map in viewmodel)
                {
                    if (map.Fields.FirstOrDefault(x=>x.State==ViewModel.State.Statek) == null)
                    {
                        var idWinner = map.IdOpponent;
                        viewmodel.First(x => x.IdPlayer == idWinner).IsWinner = true;
                        break;
                    }
                }
            

                if (isHit)
                    viewmodel.First(x => x.IdPlayer == model.IdPlayer).IsGo = true;
                else
                {
                    viewmodel.First(x => x.IdPlayer == model.IdOpponent).IsGo = true;
                }
                ModelState.Clear();
              
                return View(viewmodel);
            }
        }


        public ActionResult ShowMap()
        {          
            CreateMap();
            var viewmodel = new List<MapViewModel>();
            using (var db = new BattleShipContext())
            {
                foreach (var player in Players)
                {
                    var fields = db.Fields.Where(x => x.PlayerId == player.Id).ToList();
                    var fieldsVieModel = Mapper.Map<IList<Field>, IList<FieldViewModel>>(fields);
                    viewmodel.Add(new MapViewModel { LengthMap = _x, HighMap = _y, NamePlayer = player.Name, IdPlayer = player.Id, Fields = fieldsVieModel });
                }
                viewmodel.First().IdOpponent = viewmodel.Last().IdPlayer;
                viewmodel.Last().IdOpponent = viewmodel.First().IdPlayer;
                viewmodel.First().IsGo = true;

                return View(viewmodel);
            }
        }


        private void CreateMap()
        {
            using (var db = new BattleShipContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Fields]");
                var rnd = new Random(DateTime.Now.Millisecond);
                var count = db.Players.Count();
                var players = db.Players.OrderBy(x => x.Id).Skip(count - 2).ToList();
                Players = Mapper.Map<IList<Player>, IList<PlayerViewModel>>(players);
                for (int i = 0; i < 2; i++)
                {
                    var list = new List<Field>();

                    for (int j = 0; j < _x; j++)
                    {
                        for (int k = 0; k < _y; k++)
                        {
                            list.Add(new Field { X = j, Y = k, State = State.Puste, PlayerId = Players[i].Id });
                        }
                    }

                    int stateCount = 0;
                    while (stateCount < 6)
                    {
                        if (CheckNeighbours(rnd.Next(0, 6), rnd.Next(0, 6), list))
                            stateCount++;
                    }
                    db.Fields.AddRange(list);
                    db.SaveChanges();
                }
            }
        }

        private bool CheckNeighbours(int x, int y, List<Field> list)
        {
            if (list.FirstOrDefault(k => k.X == x && k.Y == y && k.State == State.Statek) != null)
                return false;
            var startX = x - 1 >= 0 ? x - 1 : x;
            var endX = x + 1 < _x ? x + 1 : x;
            var startY = y - 1 >= 0 ? y - 1 : y;
            var endY = y + 1 < _y ? y + 1 : y;

            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j <= endY; j++)
                {
                    if (list.FirstOrDefault(k => k.X == i && k.Y == j && k.State == State.Statek) != null)
                        return false;
                }
            }
            list.First(k => k.X == x && k.Y == y).State = State.Statek;
            return true;
        }
    }
}