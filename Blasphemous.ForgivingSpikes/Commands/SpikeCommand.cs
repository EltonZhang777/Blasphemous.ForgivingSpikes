using Blasphemous.CheatConsole;
using Blasphemous.ForgivingSpikes.Components;
using Blasphemous.NewbieEltonLibs.CheatConsole;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using System;
using System.Globalization;

namespace Blasphemous.ForgivingSpikes.Commands;

/// <summary>
/// Command for viewing and editing the global spike configuration
/// </summary>
internal class SpikeCommand : AutoModCommand
{
    private const string ValidFields = "spikePenaltyType, spikeDamageAmount, spikeDamageIgnoreDefense, spikeDamageElement, spikeDamageType";

    private readonly ForgivingSpikes _mod;

    internal SpikeCommand(ForgivingSpikes mod)
    {
        _mod = mod;
    }

    protected override string CommandName => "spike";

    [ModSubCommand("help", "show available spike commands", null, 0)]
    private void SubCommand_Help(string[] parameters)
    {
#if DEBUG
        Write($"{CommandName} respawn : (debug use) show the spike respawn animation");
#endif
        Write($"{CommandName} current : show this mod's global spike configuration");
        Write($"{CommandName} set <field> <value> : update and save one global spike setting");
        Write($"Valid fields: {ValidFields}");
        Write("PercentageDamage treats spikeDamageAmount as a max-health multiplier (0.5 = 50%).");
    }

    [ModSubCommand("current", "show this mod's global spike configuration", null, 0)]
    private void SubCommand_Current(string[] parameters)
    {
        SpikePenaltyConfig config = SpikeUtilities.GlobalConfig;
        Write($"spikePenaltyType: {config.spikePenaltyType}");
        Write($"spikeDamageAmount: {config.spikeDamageAmount.ToString("R", CultureInfo.InvariantCulture)}");
        Write($"spikeDamageIgnoreDefense: {config.spikeDamageIgnoreDefense}");
        Write($"spikeDamageElement: {config.spikeDamageElement}");
        Write($"spikeDamageType: {config.spikeDamageType}");
    }

    [ModSubCommand("set", "update and save one global spike setting", "set <field> <value>", 2)]
    private void SubCommand_Set(string[] parameters)
    {
        string field = parameters[0];
        string value = parameters[1];
        SpikePenaltyConfig updated = SpikeUtilities.GlobalConfig;
        string savedValue;

        switch (field)
        {
            case "spikePenaltyType":
                if (!TryParseEnum(value, out SpikePenaltyConfig.SpikePenaltyType penaltyType))
                {
                    Write($"Invalid value for {field}: '{value}'. Accepted values: {EnumValueNames<SpikePenaltyConfig.SpikePenaltyType>()}.");
                    return;
                }
                updated.spikePenaltyType = penaltyType;
                savedValue = penaltyType.ToString();
                break;
            case "spikeDamageAmount":
                if (!float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float damageAmount))
                {
                    Write($"Invalid value for {field}: '{value}'. Use '.' as the decimal separator; Single values include negative numbers, NaN, Infinity, and -Infinity.");
                    return;
                }
                updated.spikeDamageAmount = damageAmount;
                savedValue = damageAmount.ToString("R", CultureInfo.InvariantCulture);
                break;
            case "spikeDamageIgnoreDefense":
                if (!bool.TryParse(value, out bool ignoreDefense))
                {
                    Write($"Invalid value for {field}: '{value}'. Accepted values: true, false.");
                    return;
                }
                updated.spikeDamageIgnoreDefense = ignoreDefense;
                savedValue = ignoreDefense.ToString();
                break;
            case "spikeDamageElement":
                if (!TryParseEnum(value, out DamageArea.DamageElement damageElement))
                {
                    Write($"Invalid value for {field}: '{value}'. Accepted values: {EnumValueNames<DamageArea.DamageElement>()}.");
                    return;
                }
                updated.spikeDamageElement = damageElement;
                savedValue = damageElement.ToString();
                break;
            case "spikeDamageType":
                if (!TryParseEnum(value, out DamageArea.DamageType damageType))
                {
                    Write($"Invalid value for {field}: '{value}'. Accepted values: {EnumValueNames<DamageArea.DamageType>()}.");
                    return;
                }
                updated.spikeDamageType = damageType;
                savedValue = damageType.ToString();
                break;
            default:
                Write($"Invalid field '{field}'. Valid fields: {ValidFields}.");
                return;
        }

        SpikePenaltyConfig previous = _mod.config.globalSpikePenaltyConfig;
        _mod.config.globalSpikePenaltyConfig = updated;
        try
        {
            _mod.ConfigHandler.Save(_mod.config);
        }
        catch (Exception exception)
        {
            _mod.config.globalSpikePenaltyConfig = previous;
            Write($"Could not save {field}: {exception.Message}");
            return;
        }

        SpikeUtilities.SetGlobalConfig(updated);
        Write($"Saved {field} = {savedValue}.");
    }

    private static bool TryParseEnum<T>(string value, out T parsedValue) where T : struct
    {
        foreach (string name in Enum.GetNames(typeof(T)))
        {
            if (string.Equals(name, value, StringComparison.OrdinalIgnoreCase))
            {
                parsedValue = (T)Enum.Parse(typeof(T), name);
                return true;
            }
        }

        parsedValue = default;
        return false;
    }

    private static string EnumValueNames<T>() where T : struct
    {
        return string.Join(", ", Enum.GetNames(typeof(T)));
    }

#if DEBUG
    [ModSubCommand("respawn", "(debug use) show the spike respawn animation", null, 0)]
    private void SubCommand_Respawn(string[] parameters)
    {
        UIController.instance.StartCoroutine(SpikeUtilities.TpoSpikeRespawnCoroutine());
    }
#endif
}