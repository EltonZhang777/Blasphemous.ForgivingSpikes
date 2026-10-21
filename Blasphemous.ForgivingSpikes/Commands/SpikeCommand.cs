using Blasphemous.CheatConsole;
using Blasphemous.ForgivingSpikes.Components;
using Gameplay.GameControllers.Entities;
using Gameplay.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Blasphemous.ForgivingSpikes.Commands;

/// <summary>
/// Command for viewing and editing the global spike configuration
/// </summary>
internal class SpikeCommand : ModCommand
{
    private const string ValidFields = "spikePenaltyType, spikeDamageAmount, spikeDamageIgnoreDefense, spikeDamageElement, spikeDamageType";

    private readonly ForgivingSpikes _mod;

    internal SpikeCommand(ForgivingSpikes mod)
    {
        _mod = mod;
    }

    protected override string CommandName => "spike";

    protected override bool AllowUppercase => true;

    protected override Dictionary<string, Action<string[]>> AddSubCommands()
    {
        Dictionary<string, Action<string[]>> result = new()
        {
            { "help", SubCommand_Help },
            { "current", SubCommand_Current },
            { "set", SubCommand_Set }
        };

#if DEBUG
        result.Add("respawn", SubCommand_Respawn);
#endif
        return result;
    }

    private void SubCommand_Help(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

#if DEBUG
        Write($"{CommandName} respawn : (debug use) show the spike respawn animation");
#endif
        Write($"{CommandName} current : show this mod's global spike configuration");
        Write($"{CommandName} set <field> <value> : update and save one global spike setting");
        Write($"Valid fields: {ValidFields}");
        Write("PercentageDamage treats spikeDamageAmount as a max-health multiplier (0.5 = 50%).");
    }

    private void SubCommand_Current(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        SpikePenaltyConfig config = SpikeUtilities.GlobalConfig;
        Write($"spikePenaltyType: {config.spikePenaltyType}");
        Write($"spikeDamageAmount: {config.spikeDamageAmount.ToString("R", CultureInfo.InvariantCulture)}");
        Write($"spikeDamageIgnoreDefense: {config.spikeDamageIgnoreDefense}");
        Write($"spikeDamageElement: {config.spikeDamageElement}");
        Write($"spikeDamageType: {config.spikeDamageType}");
    }

    private void SubCommand_Set(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 2))
            return;

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

    private void SubCommand_Respawn(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        UIController.instance.StartCoroutine(SpikeUtilities.TpoSpikeRespawnCoroutine());
    }

    private bool ValidateParameterList(string[] parameters, List<int> validParameterLengths)
    {
        if (!validParameterLengths.Contains(parameters.Length))
        {
            StringBuilder sb = new();
            sb.Append($"This command takes ");
            for (int i = 0; i < validParameterLengths.Count; i++)
            {
                sb.Append($"{i} ");
                if (i != validParameterLengths.Count - 1)
                    sb.Append("or ");
            }
            sb.Append($"parameters.  You passed {parameters.Length}");
            Write(sb.ToString());

            return false;
        }

        return true;
    }
}
