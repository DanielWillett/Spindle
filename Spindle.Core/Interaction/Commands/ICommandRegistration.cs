using System;
using System.Collections.Generic;
using System.Text;

namespace Spindle.Interaction.Commands;
public interface ICommandRegistration// : ISubCommandContainer
{
    string Name { get; }
    IEnumerable<string> Aliases { get; }

    int Priority { get; }
}