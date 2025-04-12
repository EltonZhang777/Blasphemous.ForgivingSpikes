using Blasphemous.CheatConsole;
using Gameplay.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blasphemous.ForgivingSpikes.Commands;

internal class SpikeCommand : ModCommand
{
    protected override string CommandName => "spike";

    protected override bool AllowUppercase => true;

    protected override Dictionary<string, Action<string[]>> AddSubCommands()
    {
        Dictionary<string, Action<string[]>> result = new()
        {
            { "help", SubCommand_Help }
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
    }

    private void SubCommand_Respawn(string[] parameters)
    {
        if (!ValidateParameterList(parameters, 0))
            return;

        UIController.instance.StartCoroutine(SpikeUtilities.TpoRespawnCoroutine());
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
