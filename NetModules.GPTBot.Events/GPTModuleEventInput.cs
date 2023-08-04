using System;
using NetModules.Interfaces;

namespace NetModules.GPTBot.Events
{
    public struct GPTModuleEventInput : IEventInput
    {
        public string Request { get; set; }
    }
 
}