using System.Collections.Generic;

namespace Statki.ViewModel
{
    public class MapViewModel
    {
        public IList<FieldViewModel> Fields { get; set; }
        public string NamePlayer { get; set; }
        public int IdPlayer { get; set; }
        public int IdOpponent { get; set; }
        public int IdSession { get; set; }
        public bool IsGo  { get; set; }
        public bool IsWinner { get; set; }
        public int ShotX { get; set; }
        public int ShotY { get; set; }
        public int LengthMap { get; set; }
        public int HighMap { get; set; }
    }
}
