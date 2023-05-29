﻿using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static OtherworldMod.Core.Util.Utils;
using static OtherworldMod.Common.ChangeNPC.Utilities.NPCMethods;
using static OtherworldMod.Common.ChangeNPC.Utilities.OtherworldNPCSets;

namespace OtherworldMod.Common.ChangeNPC.AI
{
    /// <summary>
    /// <see cref="NPCAIStyleID.DemonEye"/>
    /// </summary>
    internal class AIStyle_002 : AIStyleType
    {
        protected override int[] ApplicableNPCs => new int[] { NPCID.DemonEye2, NPCID.PurpleEye2, NPCID.GreenEye2, NPCID.DialatedEye2, NPCID.SleepyEye2,
                NPCID.CataractEye2, NPCID.DemonEye, NPCID.TheHungryII, NPCID.WanderingEye, NPCID.PigronCorruption, NPCID.PigronHallow,
                NPCID.PigronCrimson, NPCID.CataractEye, NPCID.SleepyEye, NPCID.DialatedEye, NPCID.GreenEye, NPCID.PurpleEye,
                NPCID.DemonEyeOwl, NPCID.DemonEyeSpaceship };
        public override void Load()
        {
            AddAI(EyeAttack1, EyeAttack2, EyeAttack3, EyeWet, EyeDaytime);
        }
        static string? EyeDaytime(NPC npc, int timer)
        {
            npc.GetGlobalNPC<OtherworldNPC>().allowContactDmg = false;
            //Find target
            bool foundTarget = FindTarget(npc, out Vector2 targetPos);
            //Find target direction
            Vector2 targetDir = npc.DirectionTo(targetPos);
            //Change velocity to move away from target (account for confusion)
            npc.velocity -= targetDir * (npc.confused ? -.14f : .14f);
            //Never move on from this AI (I'm not all too concerned about it having weird behaviour when it cycles to nighttime again)
            return null;
        }
        static string? EyeWet(NPC npc, int timer)
        {
            if (Main.dayTime)
            {
                return nameof(EyeDaytime);
            }
            //If NPC is no longer wet change to different phase
            if (!(npc.wet || npc.honeyWet || npc.lavaWet || npc.shimmerWet))
            {
                return nameof(EyeAttack1);
            }
            npc.GetGlobalNPC<OtherworldNPC>().allowContactDmg = false;
            npc.velocity.Y -= .048f;
            npc.velocity.X *= .98f;
            return null;
        }
        static string? EyeAttack1(NPC npc, int timer)
        {
            if (Main.dayTime)
            {
                return nameof(EyeDaytime);
            }
            if (npc.wet || npc.honeyWet || npc.lavaWet || npc.shimmerWet)
            {
                return nameof(EyeWet);
            }
            bool foundTarget = FindTarget(npc, out Vector2 targetPos);
            float dist = AppxDistanceTo(npc, targetPos);
            //If target is far enough, move to charge attack
            if (dist > 800)
            {
                return nameof(EyeAttack2);
            }
            int targetDir = targetPos.X < npc.position.X ? -1 : 1 * (npc.confused ? -1 : 1);
            npc.velocity.X += targetDir * .07f;
            int moveDir = npc.velocity.X < 0 ? -1 : 1;
            if (targetDir == moveDir)
            {
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, ((npc.Center.Y - targetPos.Y) * (npc.confused ? -1 : 1)) * .008f, .05f);
            }
            else
            {
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, ((targetPos.Y - npc.Center.Y) * (npc.confused ? -1 : 1)) * .012f, .05f);
            }
            npc.GetGlobalNPC<OtherworldNPC>().allowContactDmg = targetDir == moveDir && (MathF.Abs(npc.velocity.X) + MathF.Abs(npc.velocity.Y)) > 6.5f;
            if (npc.collideX)
            {
                npc.velocity.X = -npc.oldVelocity.X;
            }
            if (npc.collideY)
            {
                if (MathF.Abs(npc.oldVelocity.Y) > 4)
                    npc.velocity.Y = -npc.oldVelocity.Y;
                else
                {
                    npc.velocity.Y = npc.oldVelocity.Y < 0 ? -4.5f : 4.5f;
                }
            }
            return null;
        }
        static string? EyeAttack2(NPC npc, int timer)
        {
            if (Main.dayTime)
            {
                return nameof(EyeDaytime);
            }
            if (npc.wet || npc.honeyWet || npc.lavaWet || npc.shimmerWet)
            {
                return nameof(EyeWet);
            }
            bool foundTarget = FindTarget(npc, out Vector2 targetPos);
            OtherworldNPC gNPC = npc.GetGlobalNPC<OtherworldNPC>();
            //Disable contact damage on ground
            gNPC.allowContactDmg = false;
            //Check if NPC can shoot projectiles
            bool canShoot = gNPC.shootProj != null && gNPC.shootProj.Length > 0 && gNPC.shootProj[0] != 0;
            npc.velocity += npc.DirectionTo(targetPos) * .5f;
            npc.velocity *= .9f;
            if (!canShoot || timer > 90)
            {
                return nameof(EyeAttack3);
            }
            else if (timer != 0 && timer % 30 == 0)
            {
                Vector2 vel = npc.DirectionTo(targetPos).RotatedBy(MathHelper.ToRadians(((timer / 30) - 2) * 10)) * (npc.confused ? -5.4f : 5.4f);
                Projectile proj = Projectile.NewProjectileDirect(npc.GetSource_FromAI(), npc.Center, vel, Main.rand.Next(gNPC.shootProj), npc.damage / 2, 0f, Main.myPlayer);
                proj.friendly = npc.friendly;
                proj.hostile = !npc.friendly;
                npc.position -= vel;
            }
            return null;
        }
        static string? EyeAttack3(NPC npc, int timer)
        {
            if (Main.dayTime)
            {
                return nameof(EyeDaytime);
            }
            if (npc.wet || npc.honeyWet || npc.lavaWet || npc.shimmerWet)
            {
                return nameof(EyeWet);
            }
            npc.GetGlobalNPC<OtherworldNPC>().allowContactDmg = false;
            bool foundTarget = FindTarget(npc, out Vector2 targetPos);
            if (npc.collideX)
            {
                npc.velocity.X = -npc.oldVelocity.X;
            }
            if (npc.collideY)
            {
                if (MathF.Abs(npc.oldVelocity.Y) > 4)
                    npc.velocity.Y = -npc.oldVelocity.Y;
                else
                {
                    npc.velocity.Y = npc.oldVelocity.Y < 0 ? -4.5f : 4.5f;
                }
            }
            if (timer > 180)
            {
                if (timer == 181)
                {
                    npc.velocity *= 4f;
                }
                npc.GetGlobalNPC<OtherworldNPC>().allowContactDmg = true;
                if (npc.velocity.LengthSquared() < 49f)
                {
                    npc.velocity += npc.DirectionTo(targetPos).RotatedByRandom(.262f) * (npc.confused ? -.35f : .35f);
                }
                else
                {
                    npc.velocity *= .986f;
                }
                float dist = AppxDistanceTo(npc, targetPos);
                if (dist < timer * 3f && timer > 270)
                {
                    return nameof(EyeAttack1);
                }
                return null;
            }
            else
            {
                npc.velocity += npc.DirectionTo(targetPos) * (npc.confused ? -1 : 1);
                npc.velocity *= .15f;
                npc.rotation = npc.velocity.ToRotation();
            }
            return null;
        }
    }
}
