using System;
using System.Collections.Generic;
using System.Text;

namespace Quake2.Demoplay.App
{
    class AccuracyInfo : IComparable
    {
        public int Accuracy { get; private set; }
        public int Shots { get; private set; }
        public int Hits { get; private set; }
        public int Kills { get; private set; }
        public int Deaths { get; private set; }
        public int DamageGiven { get; private set; }
        public int DamageReceived { get; private set; }

        public AccuracyInfo()
        {
            Accuracy = 0;
            Shots = 0;
            Hits = 0;
            Kills = 0;
            Deaths = 0;
            DamageGiven = 0;
            DamageReceived = 0;
        }

        public AccuracyInfo(int accuracy, int shots, int hits, int kills, int deaths, int dmgGiven, int dmgReceived)
        {
            Accuracy = accuracy;
            Shots = shots;
            Hits = hits;
            Kills = kills;
            Deaths = deaths;
            DamageGiven = dmgGiven;
            DamageReceived = dmgReceived;
        }

        public override string ToString()
        {
            return Accuracy + "% (" + Hits + "/" + Shots + ")";
        }

        public static int IntFromWeaponString(string weapon)
        {
            switch (weapon)
            {
                case "Blaster":             return 0;
                case "Shotgun":             return 1;
                case "Super Shotgun":       return 2;
                case "Machinegun":          return 3;
                case "Chaingun":            return 4;
                case "Grenades":            return 5;
                case "Grenade Launcher":    return 6;
                case "Rocket Launcher":     return 7;
                case "HyperBlaster":        return 8;
                case "Railgun":             return 9;
                case "BFG10K":              return 10;
                
                default: throw new Exception("Unknown weapon '"+weapon+"' detected.");
            }
        }

        public int CompareTo(object obj)
        {
            return CompareTo((AccuracyInfo)obj);
        }

        public int CompareTo(AccuracyInfo accuracyInfo2)
        {
            return this.Accuracy.CompareTo(accuracyInfo2.Accuracy);
        }
    }

    public enum Weapon
    {
        Blaster = 0,
        Shotgun,
        SuperShotgun,
        Machinegun,
        Chaingun,
        Grenades,
        GrenadeLauncher,
        RocketLauncher,
        HyperBlaster,
        Railgun,
        BFG10K
    }
}
