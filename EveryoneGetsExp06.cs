﻿using MelonLoader;
using HarmonyLib;
using Il2Cpp;
using everyone_gets_exp_06;

[assembly: MelonInfo(typeof(EveryoneGetsExp06), "Everyone Gets Experience 25% (0.6 ver.)", "1.0.0", "Matthiew Purple")]
[assembly: MelonGame("アトラス", "smt3hd")]

namespace everyone_gets_exp_06;
public class EveryoneGetsExp06 : MelonMod
{
    private static uint[] unitExpList;

    [HarmonyPatch(typeof(nbResultProcess), nameof(nbResultProcess.nbResultLoad))]
    private class Patch
    {
        public static void Prefix()
        {
            // Registers everyone's exp after exp attribution at the end of a battle
            int teamSize = dds3GlobalWork.DDS3_GBWK.unitwork.Length;
            unitExpList = new uint[teamSize];
            for (int i = 0; i < teamSize; i++) unitExpList[i] = dds3GlobalWork.DDS3_GBWK.unitwork[i].exp;
        }
    }

    [HarmonyPatch(typeof(nbResultProcess), nameof(nbResultProcess.nbResultShutdown))]
    private class Patch2
    {
        public static void Postfix()
        {
            // Give passive demons either 25% or 100% exp if they have Watchful
            for (int i = 0; i < dds3GlobalWork.DDS3_GBWK.unitwork.Length; i++)
            {
                if (dds3GlobalWork.DDS3_GBWK.unitwork[i].hp > 0)
                {
                    // If a demon didn't get any exp yet (i.e. if it's a passive demon without Watchful)
                    if (unitExpList[i] == dds3GlobalWork.DDS3_GBWK.unitwork[i].exp)
                    {
                        datCalc.datAddExp(dds3GlobalWork.DDS3_GBWK.unitwork[i], (int)(nbResultProcess.AllExp * 0.25)); // Get 25% exp
                    }
                    // If they didn't get 100% exp and have Watchful
                    else if (unitExpList[i] + nbResultProcess.AllExp != dds3GlobalWork.DDS3_GBWK.unitwork[i].exp && dds3GlobalWork.DDS3_GBWK.unitwork[i].skill.Contains(354))
                    {
                        datCalc.datAddExp(dds3GlobalWork.DDS3_GBWK.unitwork[i], (int)Math.Ceiling(nbResultProcess.AllExp * 0.5)); // Get 50% exp (+ vanilla 50% exp)
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(datSkillHelp_msg), nameof(datSkillHelp_msg.Get))]
    private class Patch3
    {
        public static void Postfix(ref int id, ref string __result)
        {
            if (id == 354) __result = "Earn 100% EXP when \nnot participating in battle."; // Overwrites Watchful's skill description
        }
    }
}
