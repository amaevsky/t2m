using System;
using Lingua.ZoomIntegration;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ZoomClientOptions("YIgVDoPDRH2o_cixTMXEaQ", "VonaMFLd6leQJ4ZfdTUjtqtAT6CKx97k", "https://localhost:4000/redirect");

            var auth = new AuthService(options);
            var response = auth.RequestAccessToken("x6xKvWFWmB_yPw3BSlmT_WhzfCMPknMqA").Result;

            Console.ReadKey();

        }
    }
}
