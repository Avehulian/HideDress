using EFT.UI;
using HarmonyLib;
using HideDress.Models;

namespace HideDress.Patches
{
    /// <summary>
    ///     Remembers the PlayerBody of the character model that was loaded last (main menu / profile
    ///     screen) so <see cref="HideDressPlugin.Update" /> can hide its dress as well.
    /// </summary>
    /// <remarks>
    ///     <see cref="PlayerModelView.OnLoadingCompleted" /> is raised by PlayerModelLoader after
    ///     <c>CreatePlayerBody</c> has assigned <see cref="PlayerModelView.PlayerBody" />, so the
    ///     reference is always valid here. This replaces the 0.12.x HookRef on the async Show(Task).
    /// </remarks>
    [HarmonyPatch(typeof(PlayerModelView), nameof(PlayerModelView.OnLoadingCompleted))]
    internal static class PlayerModelViewOnLoadingCompletedPatch
    {
        private static void Postfix(PlayerModelView __instance)
        {
            var playerBody = __instance.PlayerBody;

            if (playerBody != null)
                HideDressModel.Instance.PlayerModelViewBody = playerBody;
        }
    }
}
