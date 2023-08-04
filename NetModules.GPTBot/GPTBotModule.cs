using System;
using NetModules.Interfaces;
using NetModules.GPTBot.Events;
using System.Threading.Tasks;


/*This is  essenially a re-write of the chatModule example in another part of the code.
 Any parts or functions that are different shall be explained in comments around the code.*/


namespace NetModules.GPTBot
{
    [Serializable]
    [Module(LoadFirst = true, Description = "This is a basic example chat module. All Types implementing Module or IModule should be decorated with a ModuleAttribute.",
        AdditionalInformation = new string[] { "AdditionalInformation could be used to hold further information such as usage instruction or documentation reference." }
        )]
    public class GPTBotModule : Module, IEventPostHandler<GPTModuleEvent>, IEventPreHandler<GPTModuleEvent>
    {
        public override bool CanHandle(IEvent e)
        {
            if (e is GPTModuleEvent)
            {
                return true;
            }
            return false;
        }

        /*This override calls another function I have created called asyncHandle, and pauses until the asyncHandle has been awaited and returns the
         value that the API gives returns. otherwise, the code would execute and the output of the modue would be returned as null, as
        the handler does not give hanve for the API to create a response in time before it continues executing.*/
        public override async void Handle(IEvent e)
        {
            asyncHandle(e).GetAwaiter().GetResult();
        
            return;
        }
        public void OnBeforeHandle(IEvent e)
        {
            Console.WriteLine("...");
        }
        public void OnHandled(IEvent e)
        {
            Console.WriteLine("...");
        }

        /*This is the asyncHandle that I described above. it is essentially a carbon copy of the code within the chatbot modules overridden 
         *'handle' function, but is asynchronous and returns a task type instead of void. This was made to avoid creating an async task return void,
         as this is highly disfavourable when programming in c#.*/
        public async Task asyncHandle(IEvent e)
        {
            if (e is GPTModuleEvent @event)
            {
                @event.Output = new GPTModuleEventOutput()
                {
                    Response = await GPT_SourceCode.query(@event.Input.Request)
                };
                @event.Handled = true;
                return;
            }

        }
    }

}