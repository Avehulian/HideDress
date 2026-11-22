# HideDress — SPT 4.1.6 port notes

Ported from upstream `kmyuhkyuk/HideDress` 1.3.1 (SPT 3.x, EFTApi 1.2 / EFT 0.12.12) to
**SPT 4.1.6** (BepInEx 5.4.23, Mono client, EFT 0.16.9).

## Reference setup

| Purpose | Path (default) |
| --- | --- |
| Game assemblies | `G:\Games\SPTarkov-4.1.6\EscapeFromTarkov_Data\Managed` |
| BepInEx 5 core | `G:\Games\SPTarkov-4.1.6\BepInEx\core` |
| KmyTarkovApi 2.0 (4.1.6 fork) | `G:\Games\SPTarkov-4.1.6\BepInEx\plugins\kmyuhkyuk-KmyTarkovApi` |

Override with `-p:Spt416Dir=<install>`.

## Dependency / version changes

* `[BepInDependency("com.kmyuhkyuk.EFTApi", "1.2.1")]` → `[BepInDependency("com.dustinmoon78.kmytarkovapi", "2.0.0")]`
  (EFTApi was renamed to KmyTarkovApi; the installed 4.1.6 build carries the fork GUID).
* Plugin/assembly version 1.3.1 → **1.4.0**. Plugin GUID unchanged
  (`com.kmyuhkyuk.HideDress`), so existing `BepInEx/config/com.kmyuhkyuk.HideDress.cfg`
  values keep loading.
* Legacy `.csproj` (R: drive HintPaths, signed with a private `.pfx`) → SDK-style
  `netstandard2.1` project; references come from the live SPT 4.1.6 folder. The upstream
  `Build` helper project (7-Zip release script) was dropped — the csproj now stages
  `build/package` and zips `release/HideDress-<ver>-SPT4.1.6.zip` itself.

## API mapping (0.12.x → 4.1.6)

| Upstream | 4.1.6 |
| --- | --- |
| `EFTApi.EFTHelpers` (`using static`) | `KmyTarkovApi.EFTHelpers` — `_PlayerHelper`, `_GameWorldHelper` unchanged |
| `EFTReflection.RefHelper.FieldRef<PlayerBody, object>("SlotViews")` | `PlayerBody.SlotViews` — public `DictionaryListHydra<EquipmentSlot, SlotView>` |
| `FieldRef<object, IList>("list_0")` | iterated directly with `SlotViews.GetValuesEnumerator()` |
| `FieldRef<object, Dress[]>("Dresses")` | `PlayerBody.SlotView.Dresses` — public field |
| `FieldRef<Dress, Renderer[]>("Renderers")` | `Dress.Renderers` — public `[SerializeField]` field |
| `RefHelper.HookRef` on async `PlayerModelView.Show` | Harmony postfix on `PlayerModelView.OnLoadingCompleted` |
| `HideDress.Attributes.EFTConfigurationPluginAttributes` (local copy) | `KmyTarkovConfiguration.Attributes.EFTConfigurationPluginAttributes` |

`ReflectionModel` is gone: every member it reflected on is public in 4.1.6, so the plugin
now needs no `FieldRef` cache and no reflection at all.

`PlayerModelView.OnLoadingCompleted` is raised by `PlayerModelLoader.Load` right after
`CreatePlayerBody` has assigned `PlayerModelView.PlayerBody`, which is exactly the point
the old `await Task __result` postfix captured (main-menu/profile/clothing/matchmaker
previews all go through `PlayerModelView.Show`).

## Dress classification (4.1.6)

`Dress` subclasses in `Assembly-CSharp.dll`: `SkinDress`, `ArmBandView`, `JackOLantern`.

* **Dress** (plain `Dress` + `JackOLantern`): renderers are toggled via `Dress.Renderers`
  (keeps the item model/pooling alive, same as upstream).
* **SkinDress** (`SkinDress`, `ArmBandView`): whole GameObject toggled with `SetActive`.

`SlotView.Dresses` is re-read every frame, so changing clothing in the menus or in raid is
picked up without re-hooking.

## Build

```powershell
dotnet build HideDress.sln -c Release
```

Output:

* `HideDress/bin/Release/HideDress.dll`
* staged package `HideDress/build/package/BepInEx/plugins/kmyuhkyuk-HideDress/`
  (`HideDress.dll` + `localized/en.json`, `localized/zh.json`)
* release zip `HideDress/release/HideDress-1.4.0-SPT4.1.6.zip`
