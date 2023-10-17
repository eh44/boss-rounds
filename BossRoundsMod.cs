using System;
using BossRounds;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.UI.Modded;
using Il2Cpp;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Data.Gameplay;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.ServerEvents;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.Main.DifficultySelect;
using Il2CppAssets.Scripts.Unity.UI_New.Main.ModeSelect;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Threading.Tasks;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(BossRoundsMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BossRounds;

public class BossRoundsMod : BloonsTD6Mod
{
    
    public static float BossHealth = 1;
    public static float BossSpeed = 1;
    public const string EventId = nameof(BossRoundsMod);
    public const string BossTypeKey = "BossRounds-BossType";
    public const string IsEliteKey = "BossRounds-IsElite";

    // Not really much point in making these settings lol, people would just abuse it even more
    public const int BaseMonkeyMoneyBonus = 300;
    public const float MapModeMonkeyMoneyMult = .5f;
    public const float EliteMonkeyMoneyMult = 2;
    private static readonly ModSettingHotkey BossHealthCustom = new(KeyCode.F7)
    {
        displayName = "Custom Boss Health HotKey"
    };
    private static readonly ModSettingHotkey BossSpeedCustom = new(KeyCode.F8)
    {
        displayName = "Custom Boss Speed HotKey"
    };
    public BossRoundSet? SelectedSet { get; private set; }

    public override void OnNewGameModel(GameModel gameModel)
    {
       
        if (InGameData.CurrentGame.gameEventId == EventId)
        {   
            gameModel.endRound = 140;
        }
    }

    public override void OnUpdate()
    {
        if (BossHealthCustom.JustPressed())
        {
            PopupScreen.instance.ShowSetValuePopup("Custom Boss Health",
                "Sets the Boss Health to the specified percentage of health",
                new Action<int>(i =>
                {
                    if (i <= 1)
                    {
                        i = 1;
                    }

                    BossHealth = ((float)i)/100;
                }), (int) BossHealth);
        }
        if (BossSpeedCustom.JustPressed())
        {
            PopupScreen.instance.ShowSetValuePopup("Custom Boss Speed",
                "Sets the Boss speed to the specified percentage of speed",
                new Action<int>(i =>
                {
                    if (i <= 1)
                    {
                        i = 1;
                    }

                    BossSpeed = i / 100;
                }), (int) BossSpeed);
        }

        if (MenuManager.instance == null) return;

        var updateScreens = false;
        if (BossRoundSet.Cache.TryGetValue(RoundSetChanger.RoundSetOverride ?? "", out var roundSet))
        {
            if (roundSet != SelectedSet)
            {
                SelectedSet = roundSet;
                updateScreens = true;
            }
        }
        else if (SelectedSet != null)
        {
            SelectedSet = null;
            updateScreens = true;
        }

        // Refresh the labels on the screens if the boss mode changed
        if (updateScreens)
        {
            var menu = MenuManager.instance.GetCurrentMenu();
            if (menu.Is<DifficultySelectScreen>(out var difficultyScreen))
            {
                foreach (var item in difficultyScreen.transform.GetComponentsInChildren<DifficultySelectMmItems>())
                {
                    item.Initialise();
                }
            }
            else if (menu.Is<ModeScreen>(out var modeScreen))
            {
                foreach (var label in modeScreen.transform.GetComponentsInChildren<ModeSelectMonkeyMoneyLabel>())
                {
                    label.Initialise();
                }
            }
        }
    }
}
