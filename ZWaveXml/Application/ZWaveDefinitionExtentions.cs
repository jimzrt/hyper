using System.Collections.Generic;
using System.Linq;
using Utils;

namespace ZWave.Xml.Application
{
    public static class ZWaveDefinitionExtentions
    {
        public static CommandClass FindCommandClass(this ZWaveDefinition zWaveDefinition, string name, byte version)
        {
            return zWaveDefinition.CommandClasses.FirstOrDefault(var => var.Name == name && var.Version == version);
        }

        public static CommandClass FindCommandClass(this ZWaveDefinition zWaveDefinition, byte key, byte version)
        {
            return zWaveDefinition.CommandClasses.FirstOrDefault(var => var.KeyId == key && var.Version == version);
        }

        public static List<CommandClass> FindCommandClasses(this ZWaveDefinition zWaveDefinition, byte key)
        {
            return zWaveDefinition.CommandClasses.Where(var => var.KeyId == key).ToList();
        }

        public static Command FindCommand(this ZWaveDefinition zWaveDefinition, CommandClass commandClass, byte key)
        {
            Command ret = null;
            if (commandClass.Command != null)
                foreach (Command var in commandClass.Command)
                {
                    if (var.Bits > 0 && var.Bits < 8)
                    {
                        if (var.KeyId == (key & Tools.GetMaskFromBits(var.Bits, (byte)(8 - var.Bits))))
                        {
                            ret = var;
                            break;
                        }
                    }
                    else if (var.KeyId == key)
                    {
                        ret = var;
                        break;
                    }
                }
            return ret;
        }

        public static Command FindCommand(this ZWaveDefinition zWaveDefinition, CommandClass commandClass, string name)
        {
            return commandClass.Command.FirstOrDefault(var => var.Name == name);
        }

        public static Command FindCommand(this ZWaveDefinition zWaveDefinition, string commandClassName, byte commandClassVersion, string name)
        {
            CommandClass cmdClass = FindCommandClass(zWaveDefinition, commandClassName, commandClassVersion);
            return cmdClass.Command.FirstOrDefault(var => var.Name == name);
        }
    }
}
