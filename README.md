# HideDress

Hide your own or other players' dress (clothing and skins) on the character model.

This is the **SPT 4.1.6** build (BepInEx 5.4.23 / Mono client, EFT 0.16.9), ported from
the original SPT 3.x mod by [kmyuhkyuk](https://github.com/kmyuhkyuk/HideDress).

## Requirements

* SPT 4.1.6
* [KmyTarkovApi 2.0](https://hub.sp-tarkov.com/files/file/1215-kmy-tarkov-api)
  (`BepInEx/plugins/kmyuhkyuk-KmyTarkovApi`, 4.1.6 build)

## Install

Copy the `BepInEx` folder from the release zip into the SPT folder.

## Settings

Open the KmyTarkovConfiguration window (default `F12`) and select **Hide Dress**:

| Setting | Meaning |
| --- | --- |
| Update Hide Player Dress | Hide your own dress (`Both` / `Dress` / `SkinDress` / `None`) |
| Update Hide Other Player Dress | Hide other players' dress |
| Keyboard Shortcut Settings | Toggle either setting at runtime |

## Build the source

```powershell
dotnet build HideDress.sln -c Release
```

The project references BepInEx, the game assemblies and KmyTarkovApi from
`G:\Games\SPTarkov-4.1.6` by default; override the install path with
`-p:Spt416Dir=<install>`. Release builds stage `HideDress/build/package` and
produce `HideDress/release/HideDress-<version>-SPT4.1.6.zip`.

See [PORT_NOTES.md](PORT_NOTES.md) for the full 0.12.x → 4.1.6 API mapping.

## Credits

* Original mod and code: **kmyuhkyuk** — <https://github.com/kmyuhkyuk/HideDress>
* SPT 4.1.6 port: **Avehulian**

## License

GNU General Public License v3.0 — see [LICENSE](LICENSE).

The upstream project is licensed under GPL-3.0; this port is a derivative work and keeps
the same license. The original copyright notice is preserved in `LICENSE`.
