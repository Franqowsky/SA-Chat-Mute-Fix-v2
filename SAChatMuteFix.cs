using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Timers;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace SAChatMuteFix;

[MinimumApiVersion(80)]
public class SAChatMuteFix : BasePlugin
{
    public override string ModuleName        => "SA Chat Mute Fix";
    public override string ModuleVersion     => "2.0.5";
    public override string ModuleAuthor      => "Franqowsky";
    public override string ModuleDescription => "Blocks player chat from GAG/MUTE in SimpleAdmin (auto-DB)";

    private readonly Dictionary<ulong, DateTime> _penalized = new();
    private string _connStr = "";

    public override void Load(bool hotReload)
    {
        _connStr = ResolveConnectionString();
        if (string.IsNullOrEmpty(_connStr))
        {
            Logger.LogError("❌ SimpleAdmin DB connect failed. Plugin unload.");
            return;
        }

        RegisterEventHandler<EventPlayerChat>(OnChat);
        HookUserMessage(118, OnUserMsg, HookMode.Pre);

        RefreshPenalties();
        AddTimer(5f, RefreshPenalties, TimerFlags.REPEAT);

        Logger.LogInformation("🚀 SA Chat Mute Fix v2.0.5 loaded with conn: {0}", _connStr);
    }

    private string ResolveConnectionString()
    {
        try
        {
            var cfgPath = Path.Combine(
                Server.GameDirectory,
                "csgo", "addons", "counterstrikesharp",
                "configs", "plugins", "CS2-SimpleAdmin",
                "CS2-SimpleAdmin.json");

            if (File.Exists(cfgPath))
            {
                var json = File.ReadAllText(cfgPath);
                var root = JsonSerializer.Deserialize<SimpleAdminRoot>(json);
                var db   = root?.DatabaseConfig;
                if (db != null && db.DatabaseType.Equals("mysql", StringComparison.OrdinalIgnoreCase))
                {
                    return $"Server={db.DatabaseHost};Port={db.DatabasePort};" +
                           $"Database={db.DatabaseName};Uid={db.DatabaseUser};" +
                           $"Pwd={db.DatabasePassword};SslMode={db.DatabaseSSlMode};";
                }
            }
        }
        catch (Exception ex)
        {
            Logger.LogWarning("⚠️ Failed to load CS2-SimpleAdmin.json: {0}", ex.Message);
        }

        // fallback (upewnij się, że podmienisz na właściwe dane)
        return "Server=127.0.0.1;Database=simpleadmin;Uid=root;Pwd=TwojeHaslo;SslMode=none;";
    }

    private HookResult OnChat(EventPlayerChat ev, GameEventInfo info)
    {
        var ply = Utilities.GetPlayerFromUserid(ev.Userid);
        if (ply?.SteamID is ulong id && IsMuted(id))
        {
            Logger.LogInformation("🚫 BLOCKED chat from {0}", ply.PlayerName);
            return HookResult.Stop;
        }
        return HookResult.Continue;
    }

    private HookResult OnUserMsg(UserMessage um)
    {
        try
        {
            var idx = um.ReadInt("entityindex");
            var ply = Utilities.GetPlayerFromIndex(idx);
            if (ply?.SteamID is ulong id && IsMuted(id))
                return HookResult.Stop;
        }
        catch { }
        return HookResult.Continue;
    }

    private bool IsMuted(ulong id) =>
        _penalized.TryGetValue(id, out var until) && until > DateTime.UtcNow;

    private void RefreshPenalties()
    {
        Task.Run(async () =>
        {
            try
            {
                using var con = new MySqlConnection(_connStr);
                await con.OpenAsync();

                const string q = @"
                    SELECT player_steamid, ends
                      FROM sa_mutes
                     WHERE status='ACTIVE'
                       AND type IN('GAG','MUTE','SILENCE')
                       AND (ends IS NULL OR ends>NOW())";

                using var cmd = new MySqlCommand(q, con);
                using var rd  = await cmd.ExecuteReaderAsync();

                var tmp = new Dictionary<ulong, DateTime>();
                while (await rd.ReadAsync())
                {
                    if (!ulong.TryParse(rd.GetString("player_steamid"), out var sid))
                        continue;

                    var ord = rd.GetOrdinal("ends");
                    DateTime end = rd.IsDBNull(ord)
                        ? DateTime.MaxValue
                        : rd.GetDateTime(ord);

                    tmp[sid] = end;
                }

                _penalized.Clear();
                foreach (var kv in tmp)
                    _penalized[kv.Key] = kv.Value;
            }
            catch (Exception ex)
            {
                Logger.LogError("DB error: {0}", ex.Message);
            }
        });
    }
}

public class SimpleAdminRoot
{
    public DatabaseConfig DatabaseConfig { get; set; } = new();
}

public class DatabaseConfig
{
    public string DatabaseType     { get; set; } = "mysql";
    public string DatabaseHost     { get; set; } = "127.0.0.1";
    public int    DatabasePort     { get; set; } = 3306;
    public string DatabaseUser     { get; set; } = "root";
    public string DatabasePassword { get; set; } = "";
    public string DatabaseName     { get; set; } = "simpleadmin";
    public string DatabaseSSlMode  { get; set; } = "none";
}

// helper do bezpiecznego ładowania typów
internal static class ReflectionExt
{
    public static IEnumerable<Type> GetTypesSafe(this Assembly asm)
    {
        try { return asm.GetTypes(); }
        catch { return Array.Empty<Type>(); }
    }
}
