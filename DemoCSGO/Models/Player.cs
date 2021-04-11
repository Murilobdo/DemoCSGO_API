namespace DemoCSGO.Models
{
    public class Player
    {
        public Player(string name, int killed, int death)
        {
            Name = name;
            Killed = killed;
            Death = death;
        }

        public string Name { get; set; }
        public int Killed { get; set; }
        public int Death { get; set; }
    }
}