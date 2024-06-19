﻿using System;
using System.Collections.Generic;

namespace Shared
{
    [Serializable]

    public class MapData
    {
        //Misc

        public string mapOwner;
        public int mapTile;
        public int[] mapSize;

        //Mods

        public List<string> mapMods = new List<string>();

        //Tiles

        public List<string> tileDefNames = new List<string>();
        public List<string> roofDefNames = new List<string>();
        public List<bool> tilePollutions = new List<bool>();

        //Things

        public List<ItemData> factionThings = new List<ItemData>();
        public List<ItemData> nonFactionThings = new List<ItemData>();

        //Humans

        public List<HumanData> factionHumans = new List<HumanData>();
        public List<HumanData> nonFactionHumans = new List<HumanData>();

        //Animals

        public List<AnimalData> factionAnimals = new List<AnimalData>();
        public List<AnimalData> nonFactionAnimals = new List<AnimalData>();
    }
}
