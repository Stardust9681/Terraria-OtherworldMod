using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.Reflection;
using CombatPlus.Common;
using Terraria.Enums;

namespace CombatPlus
{
    public class CombatPlus : Mod
    {
        //Tuple as (Requirement{Mod,value,Reason},value)
        //Set as value
        //Get as Tuple (Requirement{Mod,value,Reason},value)
        //Worth making entire struct?
        #region Server-Side Config
        public ConfigData<bool> NPCGrief;
        public ConfigData<bool> DSTXOver;
        public ConfigData<bool> ItemBalance;
        #endregion
        #region Client-Side Config
        public ConfigData<bool> UseStyleAlts;
        #endregion
        public static CombatPlus Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            #region Detours
            Terraria.On_NPC.NewNPC += NewNPC;
            Terraria.On_WorldGen.TryGrowingAbigailsFlower += CorpseFlowerCheck;
            Terraria.On_Item.NewItem_IEntitySource_int_int_int_int_int_int_bool_int_bool_bool += NewItem1;
            Terraria.On_Item.NewItem_IEntitySource_Rectangle_int_int_bool_int_bool_bool += NewItem2;
            Terraria.On_Item.NewItem_IEntitySource_Vector2_int_int_bool_int_bool_bool += NewItem3;
            Terraria.On_Item.NewItem_IEntitySource_Vector2_int_int_int_int_bool_int_bool_bool += NewItem4;
            Terraria.On_Item.NewItem_IEntitySource_Vector2_Vector2_int_int_bool_int_bool_bool += NewItem5;
            #endregion
        }
        public override void Unload()
        {
            //Instance.Unload();
        }
        #region Anti-DST
        private static bool IsDSTItem(int type)
        {
            switch (type)
            {
                case ItemID.BatBat:
                case ItemID.HamBat:
                case ItemID.AbigailsFlower:
                case ItemID.LucyTheAxe:
                case ItemID.WilsonBeardMagnificent:
                case ItemID.WilsonBeardShort:
                case ItemID.WilsonBeardLong:
                case ItemID.DontStarveShaderItem:
                case 5107:
                case ItemID.WeatherPain:
                case ItemID.PewMaticHorn:
                case ItemID.TentacleSpike:
                case ItemID.HoundiusShootius:
                case ItemID.GarlandHat:
                case ItemID.DeerThing:
                case ItemID.DeerclopsMask:
                case ItemID.DeerclopsMasterTrophy:
                case ItemID.DeerclopsPetItem:
                case ItemID.DeerclopsTrophy:
                case ItemID.MusicBoxDeerclops:
                case ItemID.DeerclopsBossBag:
                case ItemID.BerniePetItem:
                case ItemID.Eyebrella:
                case ItemID.BoneHelm:
                    return true;
            }
            return false;
        }
        private int NewItem1(Terraria.On_Item.orig_NewItem_IEntitySource_int_int_int_int_int_int_bool_int_bool_bool orig, IEntitySource source, int X, int Y, int Width, int Height, int Type, int Stack, bool noBroadcast, int pfix, bool noGrabDelay, bool reverseLookup)
        {
            if (DSTXOver)
                return orig.Invoke(source, X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
            if (IsDSTItem(Type)) return -1;
            return orig.Invoke(source, X, Y, Width, Height, Type, Stack, noBroadcast, pfix, noGrabDelay, reverseLookup);
        }
        private int NewItem2(Terraria.On_Item.orig_NewItem_IEntitySource_Rectangle_int_int_bool_int_bool_bool orig, IEntitySource source, Rectangle rectangle, int Type, int Stack, bool noBroadcast, int prefixGiven, bool noGrabDelay, bool reverseLookup)
        {
            if (DSTXOver)
                return orig.Invoke(source, rectangle, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
            if (IsDSTItem(Type)) return -1;
            return orig.Invoke(source, rectangle, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
        }
        private int NewItem3(Terraria.On_Item.orig_NewItem_IEntitySource_Vector2_int_int_bool_int_bool_bool orig, IEntitySource source, Vector2 position, int Type, int Stack, bool noBroadcast, int prefixGiven, bool noGrabDelay, bool reverseLookup)
        {
            if (DSTXOver)
                return orig.Invoke(source, position, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
            if (IsDSTItem(Type)) return -1;
            return orig.Invoke(source, position, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
        }
        private int NewItem4(Terraria.On_Item.orig_NewItem_IEntitySource_Vector2_int_int_int_int_bool_int_bool_bool orig, IEntitySource source, Vector2 pos, int Width, int Height, int Type, int Stack, bool noBroadcast, int prefixGiven, bool noGrabDelay, bool reverseLookup)
        {
            if (DSTXOver)
                return orig.Invoke(source, pos, Width, Height, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
            if (IsDSTItem(Type)) return -1;
            return orig.Invoke(source, pos, Width, Height, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
        }
        private int NewItem5(Terraria.On_Item.orig_NewItem_IEntitySource_Vector2_Vector2_int_int_bool_int_bool_bool orig, IEntitySource source, Vector2 pos, Vector2 randomBox, int Type, int Stack, bool noBroadcast, int prefixGiven, bool noGrabDelay, bool reverseLookup)
        {
            if (DSTXOver)
                return orig.Invoke(source, pos, randomBox, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
            if (IsDSTItem(Type)) return -1;
            return orig.Invoke(source, pos, randomBox, Type, Stack, noBroadcast, prefixGiven, noGrabDelay, reverseLookup);
        }
        private bool CorpseFlowerCheck(Terraria.On_WorldGen.orig_TryGrowingAbigailsFlower orig, int i, int j)
        {
            if (DSTXOver)
                return orig.Invoke(i, j);
            return false;
        }
        private int NewNPC(Terraria.On_NPC.orig_NewNPC orig, IEntitySource source, int X, int Y, int Type, int Start, float ai0, float ai1, float ai2, float ai3, int Target)
        {
            if (DSTXOver)
                return orig.Invoke(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
            if (Type == NPCID.Deerclops)
                return -1;
            return orig.Invoke(source, X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
        }
        #endregion

        public override object Call(params object[] args)
        {
            //"ModifyAI", Action<NPC, int, int>
                //Delegate param pass for modifying AI
            if (args.Length == 3)
                if (args[0] is string && ((string)args[0]).ToLower().Equals("getconfig"))
                {
                    if (args[1] is string or null)
                        if (args[2] is Mod)
                            return GetConfig((string?)args[1], (Mod)args[2]);
                }
                else if (args[0] is string && ((string)args[0]).ToLower().Equals("requireconfig"))
                    if (args[1] is string)
                        if (args[2] is Mod)
                            return null; //return RequireConfig((string)args[1], (Mod)args[2]);

            return null;
        }
        public object GetConfig(string? name, Mod caller)
        {
            (string key, object val)[] KVPairs = new (string key, object val)[] {
                ("dst", Instance.DSTXOver.value),
                ("dontstarve", Instance.DSTXOver.value),
                ("dontstarvetogether", Instance.DSTXOver.value),
                ("dstogether", Instance.DSTXOver.value),

                ("npcgrief", Instance.NPCGrief.value),
                ("enemygrief", Instance.NPCGrief.value),

                ("ibalance", Instance.ItemBalance.value),
                ("itembalance", Instance.ItemBalance.value),
                ("balanceitem", Instance.ItemBalance.value),

                ("altusestyles", Instance.UseStyleAlts.value),
                ("usestylealts", Instance.UseStyleAlts.value),
            };
            if (name is null) //If Call wants to see what Configs there are, return array with config names
                return new string[]
                    {
                        "dst", "npcgrief", "itembalance", "altusestyles"
                    };
            name = name!.ToLower();
            for (int i = 0; i < KVPairs.Length; i++)
            {
                if (KVPairs[i].key.Equals(name))
                    return KVPairs[i].key;
            }
            Logging.PublicLogger.Warn($"[CombatPlus] Could not find config \"{name}\" from {caller.Name}'s request.");
            return null;

            /*
            name = name.ToLower();
            if (AllConfig.ContainsKey(name))
                return AllConfig[name];
            string[] keys = new string[AllConfig.Keys.Count];
            AllConfig.Keys.CopyTo(keys, 0);
            string maybeKey = "";
            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i].ToLower();
                if (key.Equals(name))
                {
                    Terraria.ModLoader.Logging.PublicLogger.Debug($"[CombatPlus]: Incorrect config key capitalisation, using fallback, '{key}.'");
                    return AllConfig[keys[i]];
                }
                if (key.Contains(name))
                    maybeKey = key;
            }
            if (!maybeKey.Equals(""))
            {
                Terraria.ModLoader.Logging.PublicLogger.Warn($"[CombatPlus] Warning from <{caller.Name}>: Could not find config key, name, or index, '{name},' using closest match.");
                return AllConfig[maybeKey];
            }
            Terraria.ModLoader.Logging.PublicLogger.Error($"[CombatPlus] Warning from <{caller.Name}>: Could not find config key, name, or index, '{name}.' No match found.");
            return null;
            */
        }
    }

    [Label("Combat+ Mod Config (Server)")]
    public class CombatServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        //True : Allows applicable NPCs to affect environment
        [DefaultValue(false)]
        [Label("Toggle Enemy Grief")]
        public bool enemyGrief;

        //False : Prevent DST garbage
        [DefaultValue(false)]
        [Label("Toggle DST Crossover")]
        public bool dst;

        //True : Enable damage, usetime, stack, rarity, sell price, liquid, etc balances.
        [DefaultValue(true)]
        [Label("Toggle Item Balances")]
        public bool itemBalance;

        public override void OnChanged()
        {
            if (CombatPlus.Instance != null)
            {
                CombatPlus.Instance.NPCGrief.value = enemyGrief;
                CombatPlus.Instance.DSTXOver.value = dst;
                CombatPlus.Instance.ItemBalance.value = itemBalance;
            }
        }
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            if (whoAmI != Main.myPlayer)
            {
                message = "Please contact the server host to change these settings.";
                System.Console.WriteLine($"{Main.player[whoAmI].name} tried to access server config.");
                return false;
            }
            return true;
        }
        private static CombatServerConfig AsThis(ModConfig config) => (CombatServerConfig)config;
    }

    [Label("Combat+ Mod Config (Client)")]
    public class CombatClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;
        [DefaultValue(true)]
        [Label("Toggle Alternative Usestyles")]
        public bool altUsestyles;
        public override void OnChanged()
        {
            if(CombatPlus.Instance!=null)
                CombatPlus.Instance.UseStyleAlts.value = altUsestyles;
        }
        public override bool NeedsReload(ModConfig pendingConfig)
        {
            return CombatPlus.Instance.UseStyleAlts.value != AsThis(pendingConfig).altUsestyles;
        }
        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
        {
            return true;
        }
        private static CombatClientConfig AsThis(ModConfig config) => (CombatClientConfig)config;
    }
}