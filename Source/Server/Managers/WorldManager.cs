﻿using Shared;
using static Shared.CommonEnumerators;

namespace GameServer
{
    public static class WorldManager
    {
        private static string worldFileName = "WorldValues.json";

        private static string worldFilePath = Path.Combine(Master.corePath, worldFileName);

        public static void ParseWorldPacket(ServerClient client, Packet packet)
        {
            WorldData worldData = Serializer.ConvertBytesToObject<WorldData>(packet.contents);

            switch (worldData.worldStepMode)
            {
                case WorldStepMode.Required:
                    Master.worldValues = worldData.worldValuesFile;
                    Master.SaveValueFile(ServerFileMode.World);
                    break;

                case WorldStepMode.Existing:
                    //Do nothing
                    break;
            }
        }

        public static bool CheckIfWorldExists() { return File.Exists(worldFilePath); }

        public static void RequireWorldFile(ServerClient client)
        {
            WorldData worldData = new WorldData();
            worldData.worldStepMode = WorldStepMode.Required;

            Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.WorldPacket), worldData);
            client.listener.EnqueuePacket(packet);
        }

        public static void SendWorldFile(ServerClient client)
        {
            WorldData worldData = new WorldData();
            worldData.worldStepMode = WorldStepMode.Existing;
            worldData.worldValuesFile = Master.worldValues;

            Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.WorldPacket), worldData);
            client.listener.EnqueuePacket(packet);
        }
    }
}
