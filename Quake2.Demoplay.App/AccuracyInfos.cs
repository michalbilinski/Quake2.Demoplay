using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Quake2.Demoplay.App
{
    class AccuracyInfos
    {
        AccuracyInfo[] _accuracyInfoArray = new AccuracyInfo[11];

        public AccuracyInfo this[int weaponIndex]
        {
            get
            {
                if (_accuracyInfoArray[weaponIndex] == null)
                    _accuracyInfoArray[weaponIndex] = new AccuracyInfo();

                return _accuracyInfoArray[weaponIndex];
            }
        }

        public AccuracyInfos(string tdmAccuracyTable, string mod)
        {
            /*-----------------+------+-------+------+-------+--------+-------+-------
            Blaster          |   0% |     0 |    0 |     0 |      0 |     0 |   135
            Super Shotgun    |   0% |     0 |    0 |     0 |      0 |     0 |   186
            Chaingun         |  15% |  2274 |  345 |    17 |      1 |  2070 |  1584
            Rocket Launcher  |  38% |    13 |    5 |     2 |      0 |   431 |   253

            Railgun          |  43% |   151 |   66 |    62 |      0 |  6600 |     0
            Damage:       9101/2158 
            Team Damage:   539/291  
            */

            /*            ---------------+----------------------------------------
                     Blaster |    -     -      -     -     -    0    0
                     Shotgun |    -     -      -     -     -    2    0
               Super Shotgun |  70%     2      0   348    66    2    2
                  Machinegun |   0%     0      0     0   144    3    3
                    Chaingun |  14%    10      0  2094   240    5    4
                    Grenades |   0%     0      -     0     -    0    0
            Grenade Launcher |  20%     0      -   107     -    4    4
             Rocket Launcher |  22%     7      1  1039   941    1   13
                HyperBlaster |   0%     0      0     0    60    0    1
                     Railgun |  43%     2      2   600   900    0    4
                C
            */
            string regex = String.Empty;
            switch(mod)
            {
                case "tdm":
                    regex = @"\n+([\w\s]+?)[\s\|]+(\d+)%[\s\|]+(\d+)[\s\|]+(\d+)[\s\|]+(\d+)[\s\|]+(\d+)[\s\|]+(\d+)[\s\|]+(\d+)";                    
                    break;
                case "opentdm":
                    regex = @"\n+\s*([\w\s]+?)[\s\|]+([\d\-]+)%{0,1}[\s\|]+([\d\-]+)[\s\|]+([\d\-]+)[\s\|]+([\d\-]+)[\s\|]+([\d\-]+)[\s\|]+([\d\-]+)[\s\|]+([\d\-]+)";
                    break;
                default:
                    throw new NotSupportedException("No support for accuracy information for mod "+mod);
            }

            MatchCollection matches = Regex.Matches(tdmAccuracyTable, regex);
            //TODO make it work!
            foreach (Match m in matches)
            {
                string weapon = m.Groups[1].Value;

                int accuracy = 0;
                int shots = 0;
                int hits = 0;
                int kills = 0;
                int deaths = 0;
                int dmgGiven = 0;
                int dmgReceived = 0;

                if (m.Groups[2].Value != "-")
                    accuracy = Convert.ToInt32(m.Groups[2].Value);
                if (m.Groups[3].Value != "-")
                    shots = Convert.ToInt32(m.Groups[3].Value);
                if (m.Groups[4].Value != "-")
                    hits = Convert.ToInt32(m.Groups[4].Value);
                if (m.Groups[5].Value != "-")
                    kills = Convert.ToInt32(m.Groups[5].Value);
                if (m.Groups[6].Value != "-")
                    deaths = Convert.ToInt32(m.Groups[6].Value);
                if (m.Groups[5].Value != "-")
                    dmgGiven = Convert.ToInt32(m.Groups[5].Value);
                if (m.Groups[6].Value != "-")
                    dmgReceived = Convert.ToInt32(m.Groups[6].Value);

                _accuracyInfoArray[AccuracyInfo.IntFromWeaponString(weapon)] = new AccuracyInfo(accuracy, shots, hits, kills, deaths, dmgGiven, dmgReceived);
            }
        }
    }

}
