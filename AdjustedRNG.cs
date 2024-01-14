using Modding;
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

        /*hooks.Add(new ILHook(typeof(FakeBat).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Start");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/
        
        /*hooks.Add(new ILHook(typeof(FakeBat).GetMethod("Start", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic),
            il =>
            {
                ILCursor cursor = new ILCursor(il).Goto(0);
                cursor.GotoNext
                (
                    i => i.MatchCall(urt, "Range")
                );
                cursor.Remove();
                cursor.Emit(OpCodes.Ldstr, "FakeBat.Start");
                cursor.Emit(OpCodes.Call, cdr["Range_float"]);
            }));*/
    }

    public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
    {
        Log("Initializing");

        Instance = this;

        foreach ((string context, ContextValue value) in GlobalSettings.Contexts)
        {
            ContextDependendRandom.AddContext(context, value);
        }

        Log("Initialized");
    }

    private void InitCallBacks()
    {
    }
}