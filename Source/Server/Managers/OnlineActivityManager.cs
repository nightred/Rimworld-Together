﻿using Shared;
using static Shared.CommonEnumerators;

namespace GameServer
{
    public static class OnlineActivityManager
    {
        public static void ParseOnlineActivityPacket(ServerClient client, Packet packet)
        {
            OnlineActivityData visitData = Serializer.ConvertBytesToObject<OnlineActivityData>(packet.contents);

            switch (visitData.activityStepMode)
            {
                case OnlineActivityStepMode.Request:
                    SendVisitRequest(client, visitData);
                    break;

                case OnlineActivityStepMode.Accept:
                    AcceptVisitRequest(client, visitData);
                    break;

                case OnlineActivityStepMode.Reject:
                    RejectVisitRequest(client, visitData);
                    break;

                case OnlineActivityStepMode.Action:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Create:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Destroy:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Damage:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Hediff:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.TimeSpeed:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.GameCondition:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Weather:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Kill:
                    SendVisitActions(client, visitData);
                    break;

                case OnlineActivityStepMode.Stop:
                    SendVisitStop(client);
                    break;
            }
        }

        private static void SendVisitRequest(ServerClient client, OnlineActivityData data)
        {
            SettlementFile settlementFile = SettlementManager.GetSettlementFileFromTile(data.targetTile);
            if (settlementFile == null) ResponseShortcutManager.SendIllegalPacket(client, $"Player {client.userFile.Username} tried to visit a settlement at tile {data.targetTile}, but no settlement could be found");
            else
            {
                ServerClient toGet = UserManager.GetConnectedClientFromUsername(settlementFile.owner);
                if (toGet == null)
                {
                    data.activityStepMode = OnlineActivityStepMode.Unavailable;
                    Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                    client.listener.EnqueuePacket(packet);
                }

                else
                {
                    if (toGet.InVisitWith != null)
                    {
                        data.activityStepMode = OnlineActivityStepMode.Unavailable;
                        Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                        client.listener.EnqueuePacket(packet);
                    }

                    else
                    {
                        data.otherPlayerName = client.userFile.Username;
                        Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                        toGet.listener.EnqueuePacket(packet);
                    }
                }
            }
        }

        private static void AcceptVisitRequest(ServerClient client, OnlineActivityData data)
        {
            SettlementFile settlementFile = SettlementManager.GetSettlementFileFromTile(data.fromTile);
            if (settlementFile == null) return;
            else
            {
                ServerClient toGet = UserManager.GetConnectedClientFromUsername(settlementFile.owner);
                if (toGet == null) return;
                else
                {
                    client.InVisitWith = toGet;
                    toGet.InVisitWith = client;

                    Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                    toGet.listener.EnqueuePacket(packet);
                }
            }
        }

        private static void RejectVisitRequest(ServerClient client, OnlineActivityData data)
        {
            SettlementFile settlementFile = SettlementManager.GetSettlementFileFromTile(data.fromTile);
            if (settlementFile == null) return;
            else
            {
                ServerClient toGet = UserManager.GetConnectedClientFromUsername(settlementFile.owner);
                if (toGet == null) return;
                else
                {
                    Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                    toGet.listener.EnqueuePacket(packet);
                }
            }
        }

        private static void SendVisitActions(ServerClient client, OnlineActivityData data)
        {
            if (client.InVisitWith == null)
            {
                data.activityStepMode = OnlineActivityStepMode.Stop;
                Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                client.listener.EnqueuePacket(packet);
            }

            else
            {
                Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), data);
                client.InVisitWith.listener.EnqueuePacket(packet);
            }
        }

        public static void SendVisitStop(ServerClient client)
        {
            OnlineActivityData visitData = new OnlineActivityData();
            visitData.activityStepMode = OnlineActivityStepMode.Stop;

            Packet packet = Packet.CreatePacketFromObject(nameof(PacketHandler.OnlineActivityPacket), visitData);
            client.listener.EnqueuePacket(packet);

            ServerClient otherPlayer = client.InVisitWith;
            if (otherPlayer != null)
            {
                otherPlayer.listener.EnqueuePacket(packet);
                otherPlayer.InVisitWith = null;
                client.InVisitWith = null;
            }
        }
    }
}
