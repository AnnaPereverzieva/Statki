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
                if (CheckActualModel(model.IdSession) == false)
                    return RedirectToAction("Login", "Authentication");

                var viewmodel = new List<MapViewModel>();
                bool isHit = false;
                var state =
                    db.Fields.FirstOrDefault(x => x.PlayerId == model.IdOpponent && x.X == model.ShotX && x.Y == model.ShotY);
                if (state.State == State.Statek)
                {
                    db.Fields
                    .Single(x => x.PlayerId == model.IdOpponent && x.X == model.ShotX && x.Y == model.ShotY)
                    .State = State.Zatopiony;
                    isHit = true;
                }
                if (state.State == State.Puste)
                {
                    db.Fields
                    .Single(x => x.PlayerId == model.IdOpponent && x.X == model.ShotX && x.Y == model.ShotY)
                    .State = State.Pudlo;
                }

                db.SaveChanges();

                var players = db.Players.Where(x => x.SessionId == model.IdSession && (x.Id == model.IdPlayer || x.Id == model.IdOpponent)).ToList();

                foreach (var player in players)
                {
                    var fields = db.Fields.Where(x => x.PlayerId == player.Id).ToList();
                    var fieldsVieModel = Mapper.Map<IList<Field>, IList<FieldViewModel>>(fields);
                    viewmodel.Add(new MapViewModel { IdSession = player.SessionId, LengthMap = _x, HighMap = _y, IdOpponent = players.First(x => x.Id != player.Id).Id, NamePlayer = player.Name, IdPlayer = player.Id, Fields = fieldsVieModel });
                }

                foreach (var map in viewmodel)
                {
                    if (map.Fields.FirstOrDefault(x => x.State == ViewModel.State.Statek) == null)
                    {
                        var idWinner = map.IdOpponent;
                        viewmodel.First(x => x.IdPlayer == idWinner).IsWinner = true;
                        db.Sessions.Single(x => x.Id == map.IdSession).IsOpen = false;
                        db.SaveChanges();
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

        private bool CheckActualModel(int id)
        {
            using (var db = new BattleShipContext())
            {
                var session = db.Sessions.FirstOrDefault(x => x.Id == id);
                if (session!=null && session.IsOpen)
                    return true;
            }
            return false;
        }


        public ActionResult ShowMap(int? sessionId)
        {
            if (sessionId.HasValue==false)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int actualSessionId = (int)sessionId;
            if (CheckActualSessionId((int)sessionId) == false)
                CreateSession(out actualSessionId, (int)sessionId);

            if (CreateMap(actualSessionId) == false)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var viewmodel = new List<MapViewModel>();
            using (var db = new BattleShipContext())
            {
                foreach (var player in Players)
                {
                    var fields = db.Fields.Where(x => x.PlayerId == player.Id).ToList();
                    var fieldsVieModel = Mapper.Map<IList<Field>, IList<FieldViewModel>>(fields);
                    viewmodel.Add(new MapViewModel { IdSession = player.SessionId, LengthMap = _x, HighMap = _y, NamePlayer = player.Name, IdPlayer = player.Id, Fields = fieldsVieModel });
                }
                viewmodel.First().IdOpponent = viewmodel.Last().IdPlayer;
                viewmodel.Last().IdOpponent = viewmodel.First().IdPlayer;
                viewmodel.First().IsGo = true;

                return View(viewmodel);
            }
        }

        private void CreateSession(out int actualSessionId, int oldSessionId)
        {
            using (var db = new BattleShipContext())
            {
                var obj = new Session();
                db.Sessions.Add(obj);
                db.SaveChanges();
                var players = db.Players.Where(x => x.SessionId == oldSessionId).ToList();
                foreach (var player in players)
                {
                    db.Players.Add(new Player { Name = player.Name, SessionId = obj.Id });
                }
                db.SaveChanges();
                actualSessionId = obj.Id;
            }
        }

        private bool CheckActualSessionId(int sessionId)
        {
            using (var db = new BattleShipContext())
            {
                var isActualSession = db.Sessions.FirstOrDefault(x => x.Id == sessionId);
                if (isActualSession != null && isActualSession.IsOpen == false)
                    return false;
            }
            return true;
        }


        private bool CreateMap(int sessionId)
        {
            using (var db = new BattleShipContext())
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                var players = db.Players.Where(x => x.SessionId == sessionId).ToList();
                Players = Mapper.Map<IList<Player>, IList<PlayerViewModel>>(players);

                if (Players.Any() == false)
                    return false;
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
                return true;
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