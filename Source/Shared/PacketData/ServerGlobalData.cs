﻿using System;

namespace Shared
{
    [Serializable]
    public class ServerGlobalData
    {
        public bool isClientAdmin;
        public bool AllowCustomScenarios;
        public bool isClientFactionMember;

        public SiteValuesFile siteValues;
        public EventValuesFile eventValues;
        public ActionValuesFile actionValues;
        public RoadValuesFile roadValues;
        public DifficultyValuesFile difficultyValues;

        public PlanetNPCSettlement[] npcSettlements;
        public OnlineSettlementFile[] playerSettlements;
        public OnlineSiteFile[] playerSites;
        public CaravanDetails[] playerCaravans;
        public RoadDetails[] roads;
        public PollutionDetails[] pollutedTiles;
    }
}