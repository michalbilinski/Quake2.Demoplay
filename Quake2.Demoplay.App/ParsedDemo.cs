using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Quake2.Demoplay.App
{
    class ParsedDemo
    {
        [DisplayName("Date")]
        public DateTime Date { get { return _fi.LastWriteTime; } }
        [DisplayName("File name")]
        public string FileName { get { return _fi.Name; } }
        [DisplayName("Game mod")]
        public string Mod { get; private set; }
        [DisplayName("Map name")]
        public string MapName { get; private set; }
        [DisplayName("Full map name")]
        public string FullMapName { get; private set; }
        [DisplayName("Players")]
        public string Players
        {
            get
            {
                if (_players == null)
                {
                    StringBuilder playersSb = new StringBuilder();
                    for (int i=0; i<_playersList.Count; ++i)
                    {
                        if (i > 0)
                            playersSb.Append(", ");

                        playersSb.Append(_playersList[i]);                        
                    }

                    _players = playersSb.ToString();
                }

                return _players;
            }
        }
        /*[DisplayName("Acc Blaster")]
        public AccuracyInfo AccBlaster
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.Blaster];
            }
        }

        [DisplayName("Acc SG")]
        public AccuracyInfo AccShotgun
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.Shotgun];
            }
        }
        [DisplayName("Acc SSG")]
        public AccuracyInfo AccSuperShotgun
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.SuperShotgun];
            }
        }
        [DisplayName("Acc MG")]
        public AccuracyInfo AccMachinegun
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.Machinegun];
            }
        }
        [DisplayName("Acc CG")]
        public AccuracyInfo AccChaingun
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.Chaingun];
            }
        }
        [DisplayName("Acc Grenades")]
        public AccuracyInfo AccGrenades
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.Grenades];
            }
        }
        [DisplayName("Acc GL")]
        public AccuracyInfo AccGrenadeLauncher
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.GrenadeLauncher];
            }
        }
        [DisplayName("Acc RL")]
        public AccuracyInfo AccRocketLauncher
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.RocketLauncher];
            }
        }
        [DisplayName("Acc HB")]
        public AccuracyInfo AccHyperBlaster
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.HyperBlaster];
            }
        }
        [DisplayName("Acc RL")]
        public AccuracyInfo AccRailgun
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.Railgun];
            }
        }
        [DisplayName("Acc BFG10K")]
        public AccuracyInfo AccBFG10K
        {
            get
            {
                if (_accuracyInfos == null)
                    return null;
                return _accuracyInfos[(int)Weapon.BFG10K];
            }
        }*/
        
        private string _players = null;
        private List<Player> _playersList = null;
        private FileInfo _fi = null;
        private FileStream _fs = null;
        private AccuracyInfos _accuracyInfos = null;

        public ParsedDemo(FileInfo fi)
        {
            _playersList = new List<Player>();
            _fi = fi;
            _fs = File.Open(fi.FullName, FileMode.Open, FileAccess.Read);
            ParseAll();
        }

        void ParseAll()
        {
            ParseMod();
            ParseFullMapName();
            ParseMapName();
            ParsePlayers();
            //ParseAccuracy();
            _fs.Close();
        }

        void ParseMod()
        {
            int currentByte;
            StringBuilder modBuilder = new StringBuilder();

            _fs.Seek(14, SeekOrigin.Begin);

            while ((currentByte = _fs.ReadByte()) != 0)
            {
                modBuilder.Append((char)currentByte);
            }

            this.Mod = modBuilder.ToString();
        }

        void ParseFullMapName()
        {
            int currentByte;
            StringBuilder mapBuilder = new StringBuilder();

            _fs.Seek(2, SeekOrigin.Current);

            while ((currentByte = _fs.ReadByte()) != 0)
            {
                mapBuilder.Append((char)currentByte);
            }

            this.FullMapName = mapBuilder.ToString();
        }

        void ParseMapName()
        {
            while (_fs.CanRead)
            {
                if (_fs.ReadByte() == 0x00 && _fs.ReadByte() == 0x0d && _fs.ReadByte() == 0x21 &&
                    _fs.ReadByte() == 0x00 && _fs.ReadByte() == 0x6d && _fs.ReadByte() == 0x61 &&
                    _fs.ReadByte() == 0x70 && _fs.ReadByte() == 0x73 && _fs.ReadByte() == 0x2f)
                    {
                        //we have a map
                        int currentByte;
                        StringBuilder mapBuilder = new StringBuilder();

                        while ((currentByte = _fs.ReadByte()) != 0)
                        {
                            mapBuilder.Append((char)currentByte);
                        }

                        this.MapName = mapBuilder.ToString().Substring(0, mapBuilder.Length - 4);

                        return;
                    }
            }
        }

        public string GetFullPath()
        {
            return _fi.FullName;
        }

        void ParsePlayers()
        {
            // First players is 00 0d 20 05 = 0 13 32 5
            bool playerFound = false;

            while (_fs.CanRead)
            {
                if (_fs.ReadByte() == 13 && _fs.ReadByte() != 0 && _fs.ReadByte() == 5)
                {
                    // we have a player
                    playerFound = true;

                    int currentByte;
                    StringBuilder playerBuilder = new StringBuilder();

                    while ((currentByte = _fs.ReadByte()) != 0)
                    {
                        playerBuilder.Append((char)currentByte);
                    }

                    _playersList.Add(playerBuilder.ToString());
                }
                else if (playerFound) // we finish; all players have been added
                    return;
            }
        }

        void ParseAccuracy()
        {
            switch (this.Mod)
            {
                case "tdm":
                    ParseTdmAccuracy();
                    break;
                case "opentdm":
                    ParseOpenTdmAccuracy();
                    break;
            }
        }

        void ParseTdmAccuracy()
        {
            int start = 0;
            int previousByte = 0x00;
            int currentByte;
            StringBuilder accBuilder = new StringBuilder();

            while (_fs.CanRead && _fs.Position < _fs.Length - 9)
            {
                if (_fs.ReadByte() == 0x20 && _fs.ReadByte() == 0x7c && _fs.ReadByte() == 0x20 &&
                    _fs.ReadByte() == 0x64 && _fs.ReadByte() == 0x6d && _fs.ReadByte() == 0x67 &&
                    _fs.ReadByte() == 0x20 && _fs.ReadByte() == 0x72 && _fs.ReadByte() == 0x0a)
                {
                    // ' | dmg r.' found
                    // we have accuracy info!
                    // possibly unsafe for big demos
                    start = (int)_fs.Position;
                    break;
                }
            }

            if (start > 0)
            {
                while (_fs.CanRead && !((currentByte = _fs.ReadByte()) == 0x14 && previousByte == 0x00))
                {
                    if (currentByte != 0x00 && currentByte != 0x02)
                        accBuilder.Append((char)currentByte);
                    previousByte = currentByte;
                }

                if (previousByte == 0x00)
                {
                    //We have a full string.
                    _accuracyInfos = new AccuracyInfos(accBuilder.ToString(), this.Mod);
                    return;
                }
            }
        }

        void ParseOpenTdmAccuracy()
        {
            int start = 0;
            int previousByte = 0x00;
            int currentByte;
            StringBuilder accBuilder = new StringBuilder();

            while (_fs.CanRead && _fs.Position < _fs.Length - 3)
            {
                if (_fs.ReadByte() == 0x0a && _fs.ReadByte() == 0x2d && _fs.ReadByte() == 0x2d)
                {
                    // '.--' found
                    // we have accuracy info!
                    // possibly unsafe for big demos
                    start = (int)_fs.Position;
                    break;
                }
            }

            if (start > 0)
            {
                // if Co (from Combat Armor found, we stop)
                while (_fs.CanRead && !((currentByte = _fs.ReadByte()) == 0x6f && previousByte == 0x43))
                {
                    accBuilder.Append((char)currentByte);
                    previousByte = currentByte;
                }

                if (previousByte == 0x43)
                {
                    //We have a full string.
                    _accuracyInfos = new AccuracyInfos(accBuilder.ToString(), this.Mod);

                    // Let's move the pointer back so we are in the beginning of word Combat
                    // and we can start parsing for item stats if we want to
                    _fs.Seek(-2, SeekOrigin.Current);
                    return;
                }
            }
        }
    }
}
