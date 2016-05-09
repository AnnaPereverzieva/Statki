namespace Statki.ViewModel
{
    public enum State
    {
        Puste,
        Statek,
        Pudlo,
        Zatopiony
    }
   public class FieldViewModel
    {
        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public State State { get; set; }
        public int PlayerId { get; set; }
    }
}
