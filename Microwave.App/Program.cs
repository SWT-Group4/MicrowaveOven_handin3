using System;
using Microwave.Classes.Boundary;
using Microwave.Classes.Controllers;

namespace Microwave.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Button startCancelButton = new Button();
            Button powerButton = new Button();
            Button timeButton = new Button();

            Door door = new Door();

            Output output = new Output();

            Display display = new Display(output);

            PowerTube powerTube = new PowerTube(output);

            Light light = new Light(output);

            Microwave.Classes.Boundary.Timer timer = new Timer();

            CookController cooker = new CookController(timer, display, powerTube);

            UserInterface ui = new UserInterface(powerButton, timeButton, startCancelButton, door, display, light, cooker);

            // Finish the double association
            cooker.UI = ui;

            // Simulate a simple sequence

            bool finish = false;

            System.Console.WriteLine("Indtast:\n" +
                                     "(E) Exit\n" +
                                     "(O) Open Door\n" +
                                     "(C) Close Door\n" +
                                     "(P) Power Button, several click increases power\n" +
                                     "(T) Time Button\n" +
                                     "(S) Start / Cancel button\n");

            do
            {
                string input = null;

                // Gate the blocking ReadLine function
                if (Console.KeyAvailable)
                {
                    input = Console.ReadLine();
                }
                if (string.IsNullOrEmpty(input)) continue;

                input = input.ToUpper();

                switch (input[0])
                {
                    case 'E':
                        finish = true;
                        break;

                    case 'O':
                        door.Open();
                        break;

                    case 'C':
                        door.Close();
                        break;

                    case 'P':
                        powerButton.Press();
                        break;

                    case 'T':
                        timeButton.Press();
                        break;

                    case 'S':
                        startCancelButton.Press();
                        break;

                    default:
                        break;
                }

                System.Console.WriteLine("Indtast:\n" +
                                         "(E) Exit\n" +
                                         "(O) Open Door\n" +
                                         "(C) Close Door\n" +
                                         "(P) Power Button, several click increases power\n" +
                                         "(T) Time Button\n" +
                                         "(S) Start / Cancel button\n");

            } while (!finish);


        }
    }
}
