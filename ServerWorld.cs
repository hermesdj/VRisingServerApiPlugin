#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectM;
using ProjectM.Network;
using ProjectM.UI;
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
    public static GameDataSystem GameDataSystem = Server.GetExistingSystem<GameDataSystem>();
    
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

    private static Player ConvertEntityToPlayer(Entity userEntity)
    {
        var user = EntityManager.GetComponentData<User>(userEntity);
        return new Player(user, 
            userEntity,
            EntityManager.GetComponentData<PlayerCharacter>(user.LocalCharacter._Entity),
            user.LocalCharacter._Entity);
    }

    public static IEnumerable<Player> GetAllPlayerCharacters()
    {
        return ListUtils.Convert(
            EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>())
                .ToEntityArray(Allocator.Temp)
            )
            .Where(userEntity => EntityManager.GetComponentData<User>(userEntity).LocalCharacter._Entity != Entity.Null)
            .Select(ConvertEntityToPlayer)
                .ToList();
    }

    public static Player? GetPlayer(int userIndex)
    {
        Entity? userEntity = ListUtils
            .Convert(EntityManager.CreateEntityQuery(ComponentType.ReadOnly<User>())
                .ToEntityArray(Allocator.Temp))
            .FirstOrDefault(userEntity => EntityManager.GetComponentData<User>(userEntity).Index == userIndex);

        return userEntity.HasValue ? ConvertEntityToPlayer(userEntity.Value) : null;
    }

    public static IEnumerable<ClanTeam> GetAllClans()
    {
        return ListUtils.Convert(
            EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ClanTeam>())
                .ToEntityArray(Allocator.Temp)
        )
            .Select(clanEntity => EntityManager.GetComponentData<ClanTeam>(clanEntity))
            .ToList();
    }

    public static IEnumerable<Entity> GetAllClanEntities()
    {
        return ListUtils
            .Convert(EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ClanTeam>())
                .ToEntityArray(Allocator.Temp))
            .ToList();
    }

    public readonly record struct Player(
        User User,
        Entity UserEntity,
        PlayerCharacter Character,
        Entity CharacterEntity
    );
}