using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


/*Here is the class that connects to the API, sends requests and receives responses. As this is used to make API calls,
 * many functions are asynchronous.*/


namespace NetModules.GPTBot
{
    internal class GPT_SourceCode
    {
        public GPT_SourceCode()
        {
            string API = "https://app.spike.digital/event/OpenAi.ChatGpt.Text?authorization=gZ2Ry1ije4F2MsuyyGVzydmwD49jV3W3aTmcc9jxSrbmN6830nUyEw3WQ76a94bP&temprature=0.1&request=";

        }

        /*To make this module have a function outside of the quiz bot, I have made it so that if a string is queried, if it begins with 'Q:', a 
         random general knowledge question will be asked, and if a string begins with A:, you can have the API return a binary 
        correct or incorrect statement. to do this, query with a string in the format... 
        
    --> "A: {your question here} : {your answer here}"
        
        And a string "1" will return if the answer is correct, "0" if incorrect.
         */

        public static async Task<string> query(string request)
        {
            string[] code = request.Split(":");
            if (code[0] == "Q")
                return await newQuestionAsync();
            else if (code[0] == "A")
                return await answerQuestion(code[1], code[2]);
            else
                return "Bad string request.";
        }

        /*This is essentially a wrapper for the GetNewQuestion function, and doesnt necessarily need to exist.*/
        private static async Task<string> newQuestionAsync()
        {
            return await GetNewQuestion();
        }

        /*This function takes 2 inputs, a question and an answer, and returns "0", "1" or "2" if an error occurs.*/
        private static async Task<string> answerQuestion(string prevQuestion, string usersAnswer)
        {
            return await answerQuestionAsync(prevQuestion,usersAnswer);
        }



        /*It's probably best this function is described line for line.*/
        static async Task<string> GetNewQuestion()
        {
            //First, a new httpClient is set up...
            using (HttpClient httpClient = new HttpClient())
            {
                //Then a string is created to query the API.
                string question = "give me one random general knowledge question. DO NOT REPLY with ANYTHING but the question, printed onto a single line.";
                question = addPlus(question);
                string apiUrl = "https://app.spike.digital/event/OpenAi.ChatGpt.Text?authorization=gZ2Ry1ije4F2MsuyyGVzydmwD49jV3W3aTmcc9jxSrbmN6830nUyEw3WQ76a94bP&temprature=0.1&request="; // Replace with the API endpoint URL
                apiUrl += question;

                //This is a try except to ensure that we connect to the API, and if not then we can output an error message.
                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {   
                        //This piece of code gets the request which is returned as an async string, and turns it into a JSON document so that
                        //the response string can be extracted without the rest of the API semantics.
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var jsonDocument = JsonDocument.Parse(apiResponse);

                        if (jsonDocument.RootElement.TryGetProperty("output", out var output))
                        {
                            if (output.TryGetProperty("response", out var innerResponse))
                            {
                                string responseText = innerResponse.GetString();
                                return responseText;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    return "err";
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return ex.Message;
                }
            }
        }

        //This does the exact same thing as the function above, but asks the API different things.
        static async Task<string> answerQuestionAsync(string question, string answer)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string answerprompt = "is the answer to the question: ";
                answerprompt += question;
                answerprompt += "the following...";
                answerprompt += answer;
                answerprompt += "answer this question with simply a 0 if the answer is not correct, and a 1 if it is. ONLY answer either 0 or 1.";
                answerprompt = addPlus(answerprompt);

                string apiUrl = "https://app.spike.digital/event/OpenAi.ChatGpt.Text?authorization=gZ2Ry1ije4F2MsuyyGVzydmwD49jV3W3aTmcc9jxSrbmN6830nUyEw3WQ76a94bP&temprature=0.1&request="; // Replace with the API endpoint URL
                apiUrl += answerprompt;

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var jsonDocument = JsonDocument.Parse(apiResponse);

                        // Access the nested "output" field
                        if (jsonDocument.RootElement.TryGetProperty("output", out var output))
                        {
                            // Access the "response" property within the "output" object
                            if (output.TryGetProperty("response", out var innerResponse))
                            {
                                // Get the value of the "response" property as a string
                                string responseText = innerResponse.GetString();
                                return responseText;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                    }
                    return "err";
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    return ex.Message;
                }
            }
        }

        //This puts plus characters in place of spaces so that API call strings can be made easier.
        static string addPlus(string question)
        {
            question.Replace(" ", "+");
            return question;
        }
    }
}
