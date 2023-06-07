#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace VRisingServerApiPlugin;

public static class ServerWorld
{
    private static readonly Lazy<World> _server = new(() =>
        GetWorld("Server") ??
        throw new Exception("There is no Server world (yet). This should not be installed on a client."));

    private static readonly World Server = _server.Value;

    public static EntityManager EntityManager = Server.EntityManager;
    public static bool IsServer => Application.productName == "VRisingServer";

    private static World? GetWorld(string name)
    {
        foreach (var world in World.s_AllWorlds)
        {
            if (world.Name == name)
            {
                return world;
            }
        }

        return null;
    }

    public static List<Player> GetAllPlayerCharacters()
    {
        return ListUtils.Convert(
            EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>())
                .ToEntityArray(Allocator.Temp)
            )
            .Where(userEntity => EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity != Entity.Null)
            .Select(userEntity =>
            {
                var user = EntityManager.GetComponentData<User>(userEntity);
                return new Player(user, 
                    userEntity,
                    EntityManager.GetComponentData<PlayerCharacter>(user.LocalCharacter._Entity),
                    user.LocalCharacter._Entity);
            })
                .ToList();
    }

    public static List<ClanTeam> GetAllClans()
    {
        return ListUtils.Convert(
            EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ClanTeam>())
                .ToEntityArray(Allocator.Temp)
        )
            .Select(clanEntity => EntityManager.GetComponentData<ClanTeam>(clanEntity))
            .ToList();
    }

    public readonly record struct Player(
        User User,
        Entity UserEntity,
        PlayerCharacter Character,
        Entity CharacterEntity
    );
}