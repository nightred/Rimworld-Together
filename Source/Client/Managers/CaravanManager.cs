﻿using GameClient;
using RimWorld;
using RimWorld.Planet;
using Shared;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Shared.CommonEnumerators;

namespace GameClient
{
    public static class CaravanManager
    {
        //Variables

        public static WorldObjectDef onlineCaravanDef;
        public static List<CaravanDetails> activeCaravans = new List<CaravanDetails>();
        public static Dictionary<Caravan, int> activePlayerCaravans = new Dictionary<Caravan, int>();

        public static void ParsePacket(Packet packet)
        {
            CaravanData data = Serializer.ConvertBytesToObject<CaravanData>(packet.contents);

            switch (data.stepMode)
            {
                case CaravanStepMode.Add:
                    AddCaravan(data.details);
                    break;

                case CaravanStepMode.Remove:
                    RemoveCaravan(data.details);
                    break;

                case CaravanStepMode.Move:
                    MoveCaravan(data.details);
                    break;
            }
        }

        public static void AddCaravans(CaravanDetails[] details)
        {
            if (details == null) return;

            foreach (CaravanDetails caravan in details)
            {
                AddCaravan(caravan);
            }
        }

        private static void AddCaravan(CaravanDetails details)
        {
            activeCaravans.Add(details);

            if (details.owner == ClientValues.username)
            {
                Caravan toAdd = Find.WorldObjects.Caravans.FirstOrDefault(fetch => fetch.Faction == Faction.OfPlayer && 
                    !activePlayerCaravans.ContainsKey(fetch));

                if (toAdd == null) return;
                else activePlayerCaravans.Add(toAdd, details.ID);
            }

            else
            {
                OnlineCaravan onlineCaravan = (OnlineCaravan)WorldObjectMaker.MakeWorldObject(onlineCaravanDef);
                onlineCaravan.Tile = details.tile;
                onlineCaravan.SetFaction(FactionValues.neutralPlayer);
                Find.World.worldObjects.Add(onlineCaravan);
            }
        }

        private static void RemoveCaravan(CaravanDetails details)
        {
            CaravanDetails toRemove = CaravanManagerHelper.GetCaravanDetailsFromID(details.ID);
            if (toRemove == null) return;
            else
            {
                activeCaravans.Remove(toRemove);

                if (details.owner == ClientValues.username)
                {
                    foreach (KeyValuePair<Caravan, int> pair in activePlayerCaravans.ToArray())
                    {
                        if (pair.Value == details.ID)
                        {
                            activePlayerCaravans.Remove(pair.Key);
                            break;
                        }
                    }
                }

                else
                {
                    WorldObject worldObject = Find.World.worldObjects.AllWorldObjects.First(fetch => fetch.Tile == details.tile 
                        && fetch.def == onlineCaravanDef);

                    Find.World.worldObjects.Remove(worldObject);
                }
            }
        }

        private static void MoveCaravan(CaravanDetails details)
        {
            CaravanDetails toMove = CaravanManagerHelper.GetCaravanDetailsFromID(details.ID);
            if (toMove == null) return;
            else
            {
                if (details.owner == ClientValues.username) return;
                else
                {
                    RemoveCaravan(toMove);
                    AddCaravan(details);
                }
            }
        }

        public static void RequestCaravanAdd(Caravan caravan)
        {
            CaravanData data = new CaravanData();
            data.stepMode = CaravanStepMode.Add;
            data.details = new CaravanDetails();
            data.details.tile = caravan.Tile;
            data.details.owner = ClientValues.username;

            Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.CaravanPacket), data);
            Network.listener.EnqueuePacket(packet);
        }

        public static void RequestCaravanRemove(Caravan caravan)
        {
            activePlayerCaravans.TryGetValue(caravan, out int caravanID);

            CaravanDetails details = CaravanManagerHelper.GetCaravanDetailsFromID(caravanID);
            if (details == null) return;
            else
            {
                CaravanData data = new CaravanData();
                data.stepMode = CaravanStepMode.Remove;
                data.details = details;

                Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.CaravanPacket), data);
                Network.listener.EnqueuePacket(packet);
            }
        }

        public static void RequestCaravanMove(Caravan caravan)
        {
            activePlayerCaravans.TryGetValue(caravan, out int caravanID);

            CaravanDetails details = CaravanManagerHelper.GetCaravanDetailsFromID(caravanID);
            if (details == null) return;
            else
            {
                CaravanData data = new CaravanData();
                data.stepMode = CaravanStepMode.Move;
                data.details = details;

                Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.CaravanPacket), data);
                Network.listener.EnqueuePacket(packet);
            }
        }

        public static void ClearAllCaravans()
        {            
            activeCaravans.Clear();
            activePlayerCaravans.Clear();

            foreach (WorldObject worldObject in Find.World.worldObjects.AllWorldObjects.ToArray())
            {
                if (worldObject.def == onlineCaravanDef)
                {
                    Find.World.worldObjects.Remove(worldObject);
                }
            }
        }

        public static void ModifyDetailsTile(Caravan caravan, int updatedTile)
        {
            activePlayerCaravans.TryGetValue(caravan, out int caravanID);

            foreach (CaravanDetails details in activeCaravans)
            {
                if (details.ID == caravanID)
                {
                    details.tile = updatedTile;
                    break;
                }
            }
        }
    }
}

public static class CaravanManagerHelper
{
    //Variables

    public static CaravanDetails[] tempCaravanDetails;

    public static void SetValues(ServerGlobalData serverGlobalData)
    {
        tempCaravanDetails = serverGlobalData.playerCaravans;
    }

    public static CaravanDetails GetCaravanDetailsFromID(int id)
    {
        return CaravanManager.activeCaravans.FirstOrDefault(fetch => fetch.ID == id);
    }

    public static void SetCaravanDefs()
    {
        CaravanManager.onlineCaravanDef = DefDatabase<WorldObjectDef>.AllDefs.First(fetch => fetch.defName == "RTCaravan");
    }
}
