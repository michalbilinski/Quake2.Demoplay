using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Quake2.Demoplay.App
{
    class Player
    {
        public string Name { get; private set; }
        public string Model { get; private set; }
        public string Skin { get; private set; }

        public Player(string nameModelSkin)
        {
            // nameModelSkin = neveride\male/grunt
            Match m = Regex.Match(nameModelSkin, @"^(.+)\\(.+)/(.+)$");

            if (m.Success)
            {
                Name = m.Groups[1].Value;
                Model = m.Groups[2].Value;
                Skin = m.Groups[3].Value;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator Player(string nameModelSkin)
        {
            return new Player(nameModelSkin);
        }
    }
}
