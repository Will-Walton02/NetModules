using System;
using NetModules.Interfaces;

namespace NetModules.GPTBot.Events
{
    public struct GPTModuleEventOutput : IEventOutput
    {
        public string Response { get; set; }
    }
}