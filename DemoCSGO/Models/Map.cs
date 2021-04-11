using System.Numerics;

namespace DemoCSGO.Models
{
    public class Map
    {
        public string name { get; set; }

        public Vector2 PZero { get; set; }

        public float Scale { get; set; }    

        public Map(string name, Vector2 PZero, float Scale)
        {
            this.name = name;
            this.PZero = PZero;
            this.Scale = Scale;
        }
    }
}