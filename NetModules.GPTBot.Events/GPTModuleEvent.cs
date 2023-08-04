using System;
using System.Collections.Generic;
using NetModules.Interfaces;

namespace NetModules.GPTBot.Events
{
    public class GPTModuleEvent : IEvent<GPTModuleEventInput, GPTModuleEventOutput>
    {
        public EventName Name { get; } = "NetModules.GPTBot.GPTModuleEvent";
        public Dictionary<string, object> Meta { get; set; }
        public bool Handled { get; set; }
        public GPTModuleEventInput Input { get; set; }
        public GPTModuleEventOutput Output { get; set; }
        public IEventInput GetEventInput()
        {
            return Input;
        }
        public IEventOutput GetEventOutput()
        {
            return Output;
        }
        public void SetEventOutput(IEventOutput output)
        {
            if (output is GPTModuleEventOutput o)
            {
                Output = o;
            }
        }

    }
}