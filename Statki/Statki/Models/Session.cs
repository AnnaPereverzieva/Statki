namespace Statki.Models
{
    public class Session
    {
        public int Id { get; set; }
        public bool IsOpen { get; set; }

        public Session()
        {
            IsOpen = true;
        }
    }
}
