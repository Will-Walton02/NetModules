
using System;
using System.Reflection;
using System.Threading.Tasks;
using NetModules.GPTBot.Events;
using NetModules.Interfaces;

namespace NetModules.testGPTProgram
{
    class GPTProgram
    {
        static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            //AppDomain.CurrentDomain.TypeResolve += CurrentDomain_TypeResolve;
            Launch(args);
        }
        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }

        private static System.Reflection.Assembly CurrentDomain_TypeResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }
        static async void Launch(string[] args)
        {

            // Create a new module host
            ModuleHost host = new BasicModuleHost();

            // Invoking ModuleHost.Modules.GetModuleNames method tells us what modules have been imported. We
            // are calling this method for information only. Modules have not been loaded yet.
            var names = host.Modules.GetModuleNames();

            // Now we load modules. Modules must be loaded otherwise they can not handle events.
            host.Modules.LoadModules();

            // Importing module happens by default when the default ModuleHost is initialized but you can call
            // ImportModules any time and any newly added modules will be loaded.
            host.Modules.ImportModules();

            var modulesList = host.Modules.GetModulesByType<IModule>();

            var GPTBotModule = host.Modules.GetModulesByType<GPTBot.GPTBotModule>()[0];
            if (!string.IsNullOrWhiteSpace(GPTBotModule.ModuleAttributes.Name))
            {
                Console.WriteLine(GPTBotModule.ModuleAttributes.Name);
            }

            // Writing console lines here has nothing to do with the functionality of NetModules
            Console.WriteLine("...");
            Console.WriteLine("...");

            Console.WriteLine("Welcome to the Quiz, with your host, ChatGPT 3.5!!!");

            Console.WriteLine("...");
            Console.WriteLine("...");

            var testSolidEvent = host.Events.GetSolidEventFromName("NetModules.GPTBot.GPTModuleEvent");

            if (testSolidEvent == null)
            {
                throw new Exception("We have no GPT event loaded.");
            }

            int score = 0;
            int questions = 0;
            while (true)
            {
                string question = askQuestion(host);
                Console.WriteLine(question);
                var request = Console.ReadLine();
                if(answerQuestion(host, question, request) == 0)
                {
                    System.Console.WriteLine("Sorry! You are not Correct.");
                }
                else if(answerQuestion(host, question, request) == 1)
                {
                    System.Console.WriteLine("Well done! You are Correct.");
                    score++;
                }
                questions++;
                Console.WriteLine($"You have answered {score} questions correctly out of {questions}.");
                
                Console.WriteLine("...");
                Console.WriteLine("...");
            }
        }
        static string askQuestion(ModuleHost host)
        {
            var e = new GPTModuleEvent();

            e.Input = new GPTModuleEventInput()
            {
                Request = "Q:"
            };

            if (host.CanHandle(e))
            {
                host.Handle(e);
            }
            else
            {
                throw new Exception("Unable to handle the event.");
            }

            var input = e.GetEventInput();
            var output = e.GetEventOutput();
            e.SetEventOutput(output);

            if (e.Handled)
            {
                return e.Output.Response;
            }
            return "Sorry, i encountered an error and I cannot ask any questions.";
        }

        static int answerQuestion(ModuleHost host,string question, string answer)
        {
            var e = new GPTModuleEvent();

            e.Input = new GPTModuleEventInput()
            {
                Request = "A:" + question + ":" + answer
            };

            if (host.CanHandle(e))
            {
                host.Handle(e);
            }
            else
            {
                throw new Exception("Unable to handle the event.");
            }

            var input = e.GetEventInput();
            var output = e.GetEventOutput();
            e.SetEventOutput(output);


            if (e.Handled)
            {
                int i;
                bool success = int.TryParse(e.Output.Response, out i);
                if (success) { return i; }
            }
            return 2;
        }
    }
}
