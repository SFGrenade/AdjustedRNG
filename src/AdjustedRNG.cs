using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UDebug = UnityEngine.Debug;

namespace AdjustedRNG;

[UsedImplicitly]
public class AdjustedRNG : SFCore.Generics.GlobalSettingsMod<RngSettings>
{
    internal static AdjustedRNG Instance;

    public override string GetVersion() => "1.0";

    private static Type urt = typeof(UnityEngine.Random);
    private static Type cdrt = typeof(ContextDependendRandom);
    private static Dictionary<string, MethodInfo> cdr = new();

    private static List<ILHook> hooks = new();

    public AdjustedRNG() : base("Adjusted RNG")
    {
        Instance = this;

        InitCallBacks();
        cdr["Range_float"] = cdrt.GetMethod("Range", new Type[] { typeof(float), typeof(float), typeof(string) });
        cdr["Range_int"] = cdrt.GetMethod("Range", new Type[] { typeof(int), typeof(int), typeof(string) });
        cdr["RandomRangeInt"] = cdrt.GetMethod("RandomRangeInt", new Type[] { typeof(int), typeof(int), typeof(string) });
        cdr["RandomRange_int"] = cdrt.GetMethod("RandomRange", new Type[] { typeof(int), typeof(int), typeof(string) });
        cdr["RandomRange_float"] = cdrt.GetMethod("RandomRange", new Type[] { typeof(float), typeof(float), typeof(string) });

        hooks.Add(new ILHook(typeof(Breakable).GetMethod("Break", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Breakable.Break.Angle");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Breakable.Break.Speed");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(Breakable.FlingObject).GetMethod("Fling", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Breakable.FlingObject.Fling.Amount");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
            }));

        hooks.Add(new ILHook(typeof(BreakableObject.FlingObject).GetMethod("Fling", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "BreakableObject.FlingObject.Fling.Amount");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
            }));

        hooks.Add(new ILHook(typeof(CrystalPieceSize).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "CrystalPieceSize.OnEnable.SizeSmall");
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "CrystalPieceSize.OnEnable.SizeBig");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
            }));

        hooks.Add(new ILHook(typeof(EnemyBullet).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "EnemyBullet.scale");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(EnemyDeathEffects).GetMethod("EmitEssence", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "EnemyDeathEffects.EmitEssenceMissChance");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
            }));

        hooks.Add(new ILHook(typeof(EnemySpawner).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "EnemySpawner.SpawnMissChance");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        /*hooks.Add(new ILHook(typeof(FakeBat).GetMethod("SendOutRoutine", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Flying.Direction");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Flying.xAcceleration");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Flying.yAcceleration");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Flying.FlipAcceleration");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Flying.DirectionChangeTimer");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/

        hooks.Add(new ILHook(typeof(FakeBat).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Size");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(GameManager).GetMethod("TimePasses", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "GameManager.TimePasses.BrettaPosition");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        /*hooks.Add(new ILHook(typeof(GeoControl).GetMethod("Getter", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "GeoControl.PickupWaitTime");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/

        /*hooks.Add(new ILHook(typeof(GrimmballControl).GetMethod("DoFire", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "GrimmballControl.FireballHeight");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/

        hooks.Add(new ILHook(typeof(HealthCocoon).GetMethod("FlingObjects", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "HealthCocoon.Amount");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
            }));

        hooks.Add(new ILHook(typeof(HeroController).GetMethod("TakeDamage", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                for (int c = 0; c < 7; c++)
                {
                    cursor.GotoNext
                    (
                        i => i.MatchCall(urt, "Range")
                    );
                    cursor.Remove();
                    cursor.Emit(OpCodes.Ldstr, $"HeroController.Carefree.{c}.hitChance");
                    cursor.Emit(OpCodes.Call, cdr["Range_int"]);
                }
            }));

        hooks.Add(new ILHook(typeof(KnightHatchling).GetMethod("DoBuzz", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.waitTime.outsideOfMinusYRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.waitTime.outsideOfPlusYRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.waitTime.outsideOfMinusXRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.waitTime.outsideOfPlusXRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.accelerationY.outsideOfMinusRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.accelerationY.outsideOfPlusRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.accelerationY.insideOfRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.accelerationX.outsideOfMinusRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.accelerationX.outsideOfPlusRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.accelerationX.insideOfRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "KnightHatchling.DoBuzz.waitTime.insideOfRoamingRange");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        /*hooks.Add(new ILHook(typeof(MegaJellyZap).GetMethod("MultiZapSequence", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "MegaJellyZap.ZapWaitTime");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/

        hooks.Add(new ILHook(typeof(PaintBullet).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "PaintBullet.Size");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(ScuttlerControl).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "ScuttlerControl.Size");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "ScuttlerControl.MaxSpeed");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(SetWalkerFacing).GetMethod("Apply", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SetWalkerFacing.RandomStartDirection");
                cursor.Emit(OpCodes.Call, cdr["Range_int"]);
            }));

        hooks.Add(new ILHook(typeof(SoulOrb).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SoulOrb.ScaleModifier");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        /*hooks.Add(new ILHook(typeof(SpellFluke).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellFluke.xVelocity");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellFluke.yVelocityMin");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellFluke.yVelocityMax");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/

        hooks.Add(new ILHook(typeof(SpellFluke).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellFluke.LifeTime");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellFluke.SizeShaman");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellFluke.SizeNormal");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(SpellGetOrb).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.StartX");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.StartY");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.StartZ");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.IdleTime");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.Size");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(SpellGetOrb).GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.Speed");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "SpellGetOrb.Direction");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(Walker).GetMethod("BeginStopped", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Walker.TimeBeforeWalking");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(Walker).GetMethod("BeginWalking", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Walker.TimeBeforeStopping");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));

        hooks.Add(new ILHook(typeof(Walker).GetMethod("EndStopping", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Walker.StartToDirection");
                cursor.Emit(OpCodes.Call, cdr["Range_Int"]);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Walker.Turn");
                cursor.Emit(OpCodes.Call, cdr["Range_Int"]);
            }));

        hooks.Add(new ILHook(typeof(Walker).GetMethod("StartMoving", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "Walker.DoTurnBeforeMoving");
                cursor.Emit(OpCodes.Call, cdr["Range_Int"]);
            }));

        hooks.Add(new ILHook(typeof(WeaverlingEnemyList).GetMethod("GetTarget", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "WeaverlingEnemyList.RandomEnemyIndex");
                cursor.Emit(OpCodes.Call, cdr["Range_Int"]);
            }));
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        Log("Initializing");

        Instance = this;

        foreach ((string context, ContextValue value) in GlobalSettings.Contexts)
        {
            ContextDependendRandom.AddContext(context, value);
        }

        foreach (ILHook ilHook in hooks)
        {
            ilHook.Apply();
        }

        Log("Initialized");
    }

    private void InitCallBacks()
    {
    }
}