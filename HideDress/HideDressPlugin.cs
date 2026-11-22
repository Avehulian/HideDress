using System;
using EFT;
using EFT.Visual;
using HarmonyLib;
using HideDress.Models;
using KmyTarkovConfiguration.Attributes;
using BepInEx;
using static KmyTarkovApi.EFTHelpers;

namespace HideDress
{
    [BepInPlugin("com.kmyuhkyuk.HideDress", "HideDress", "1.4.0")]
    [BepInDependency("com.dustinmoon78.kmytarkovapi", "2.0.0")]
    [EFTConfigurationPluginAttributes("https://hub.sp-tarkov.com/files/file/654-hide-dress")]
    public partial class HideDressPlugin : BaseUnityPlugin
    {
        internal static readonly BepInEx.Logging.ManualLogSource Log =
            BepInEx.Logging.Logger.CreateLogSource("HideDress");

        private void Awake()
        {
            SettingsModel.Create(Config);

            new Harmony("com.kmyuhkyuk.HideDress").PatchAll(typeof(HideDressPlugin).Assembly);

            Log.LogInfo("HideDress 1.4.0 loaded (SPT 4.1.6)");
        }

        private void Update()
        {
            var settingsModel = SettingsModel.Instance;
            if (settingsModel == null)
                return;

            var hideDressModel = HideDressModel.Instance;

            if (settingsModel.KeyUpdatePlayerHideDressShortcut.Value.IsDown())
            {
                settingsModel.KeyUpdatePlayerHideDress.Value = !settingsModel.KeyUpdatePlayerHideDress.Value;
            }

            if (settingsModel.KeyUpdateOtherPlayerHideDressShortcut.Value.IsDown())
            {
                settingsModel.KeyUpdateOtherPlayerHideDress.Value = !settingsModel.KeyUpdateOtherPlayerHideDress.Value;
            }

            if (settingsModel.KeyUpdatePlayerHideDress.Value && hideDressModel.PlayerModelViewBody != null)
            {
                EnabledPartDress(hideDressModel.PlayerModelViewBody, settingsModel.KeyPlayerHideDressPart.Value);
            }

            var player = _PlayerHelper.Player;

            if (settingsModel.KeyUpdatePlayerHideDress.Value && player != null)
            {
                EnabledPartDress(player.PlayerBody, settingsModel.KeyPlayerHideDressPart.Value);
            }

            if (!settingsModel.KeyUpdateOtherPlayerHideDress.Value)
                return;

            var otherPlayers = _GameWorldHelper.AllOtherPlayer;

            if (otherPlayers == null)
                return;

            foreach (var otherPlayer in otherPlayers)
            {
                if (otherPlayer == null)
                    continue;

                EnabledPartDress(otherPlayer.PlayerBody, settingsModel.KeyOtherPlayerHideDressPart.Value);
            }
        }

        private static void EnabledPartDress(PlayerBody playerBody, HideDressModel.DressPart dressPart)
        {
            if (playerBody == null)
                return;

            var slotViews = playerBody.SlotViews;

            if (slotViews == null)
                return;

            foreach (var slotView in slotViews.GetValuesEnumerator())
            {
                if (slotView == null)
                    continue;

                var dresses = slotView.Dresses;

                if (dresses == null)
                    continue;

                switch (dressPart)
                {
                    case HideDressModel.DressPart.Both:
                        EnabledDress(dresses, false);
                        EnabledSkinDress(dresses, false);
                        break;
                    case HideDressModel.DressPart.Dress:
                        EnabledDress(dresses, false);
                        EnabledSkinDress(dresses, true);
                        break;
                    case HideDressModel.DressPart.SkinDress:
                        EnabledDress(dresses, true);
                        EnabledSkinDress(dresses, false);
                        break;
                    case HideDressModel.DressPart.None:
                        EnabledDress(dresses, true);
                        EnabledSkinDress(dresses, true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dressPart), dressPart, null);
                }
            }
        }

        private static void EnabledDress(Dress[] dresses, bool enabled)
        {
            foreach (var dress in dresses)
            {
                if (dress == null)
                    continue;

                var dressType = dress.GetType();

                // Plain clothing meshes (JackOLantern is a cosmetic prop that also renders through Dress).
                if (dressType != typeof(Dress) && dressType != typeof(JackOLantern))
                    continue;

                var renderers = dress.Renderers;

                if (renderers == null)
                    continue;

                foreach (var renderer in renderers)
                {
                    if (renderer != null)
                        renderer.enabled = enabled;
                }
            }
        }

        private static void EnabledSkinDress(Dress[] dresses, bool enabled)
        {
            foreach (var dress in dresses)
            {
                if (dress == null)
                    continue;

                var dressType = dress.GetType();

                if (dressType != typeof(SkinDress) && dressType != typeof(ArmBandView))
                    continue;

                dress.gameObject.SetActive(enabled);
            }
        }
    }
}
