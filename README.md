# Blasphemous: Forgiving Spikes

A Blasphemous 1 mod that makes spike, fall, and other trap penalty more forgiving. Highly customizable with other mods or by config.

---

## Features

[<img src="docs/showcase_1.gif"/>]()
[<img src="docs/showcase_2.gif"/>]()

- Make Penitent respawn after falling into spikes trap or abyss trap, instead of being instakilled. 
- Configurable functionalities, including
  - spike damage (fixed damage / percentage damage)
  - spike damage element & damage type
  - spike ignore defense or not
  - whether to revert to vanilla instakill mechanics
- Supports configurating both through `.cfg` file and public API access through `SpikeUtilities` static class

## Usage

Spike settings can be configured via either command or config file:

commands: 
- `spike current`: show global spike configuration.
- `spike set <field> <value>`: update and save a property of spike configuration.
  - e.g. `spike set spikeDamageAmount 30`

The `spike set` command accepts the below-listed `SpikePenaltyConfig` fields and values, which are also configurable in the `.cfg` file:

- `spikePenaltyType`: `Instakill`, `FixedDamage`, or `PercentageDamage`.
- `spikeDamageAmount`: a `Single` value using `.` as the decimal separator, such as `1.5`, `-1`, or `1e-3`; `NaN`, `Infinity`, and `-Infinity` are also accepted.
  - With `FixedDamage`, this is the damage amount.
  - With `PercentageDamage`, it is a multiplier of Penitent's maximum health (`0.5` means 50%).
- `spikeDamageIgnoreDefense`: `true` or `false`.
- `spikeDamageElement`: `Normal`, `Fire`, `Toxic`, `Lightning`, `Magic`, or `Contact`.
- `spikeDamageType`: `Normal`, `Heavy`, `Critical`, `Simple`, `Stunt`, or `OptionalStunt`.

When JSON omits fields from `globalSpikePenaltyConfig`, defaults are `Instakill`, `0`, `true`, `Normal`, and `Heavy`, respectively. If the entire `globalSpikePenaltyConfig` field is omitted, the config defaults to vanilla instakill behavior.


## Installation

This mod is available for download through the [Blasphemous Mod Installer](https://github.com/BrandenEK/Blasphemous.Modding.Installer)

Required dependencies (dependencies are auto-downloaded if installed through Mod Installer):
- [Modding API](https://github.com/BrandenEK/Blasphemous.ModdingAPI)
- [Cheat Console](https://github.com/brandenEK/Blasphemous.CheatConsole)
