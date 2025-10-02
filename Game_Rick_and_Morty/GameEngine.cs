using Game_Rick_and_Morty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace RickAndMortyGame.GameCore
{
    public class GameEngine
    {
        private readonly GameSettings _settings;
        private readonly GameStatistics _stats;
        private readonly IMorty? _morty;
        private readonly Sha3BasedRandomGenerator _randomnessProtocol;

        private Sha3BasedRandomGenerator? _lastPortalGunProtocolResult; 

        public GameEngine(GameSettings settings)
        {
            _settings = settings;
            _stats = new GameStatistics();

            _randomnessProtocol = new Sha3BasedRandomGenerator();
            MortyPluginLoader.TryLoadMortyAssemblies();
            _morty = CreateMortyInstance(settings.MortyType);
            if (_morty == null)
            {
                Console.WriteLine($"Error: Could not load Morty implementation for type '{settings.MortyType}'. Exiting.");
                Environment.Exit(1);
            }
        }

        private IMorty? CreateMortyInstance(string mortyTypeName)
        {
            var relevantAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

            var executingAssembly = Assembly.GetExecutingAssembly();
            if (!relevantAssemblies.Contains(executingAssembly))
            {
                relevantAssemblies.Add(executingAssembly);
            }

            foreach (var assembly in relevantAssemblies)
            {
                Type? mortyType = assembly.GetTypes()
                                         .FirstOrDefault(t => typeof(IMorty).IsAssignableFrom(t) &&
                                                              t.Name.Equals(mortyTypeName, StringComparison.OrdinalIgnoreCase) &&
                                                              !t.IsInterface && !t.IsAbstract);
                if (mortyType != null)
                {
                    try
                    {
                        return (IMorty)Activator.CreateInstance(mortyType)!;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Failed to create instance of Morty '{mortyTypeName}': {ex.Message}");
                        return null;
                    }
                }
            }

            Console.WriteLine($"[ERROR] Morty implementation '{mortyTypeName}' not found in any loaded assembly.");
            return null;
        }

        
        public void StartGame()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Welcome to Rick and Morty's Portal Gun Game!");
            Console.WriteLine($"You are playing with {_settings.NumberOfBoxes} boxes and Morty's type is '{_morty!.Name}'.");
            Console.WriteLine("------------------------------------------");

            int round = 1;
            while (true)
            {
                Console.WriteLine($"\n--- Round {round} ---");
                PlayRound();

                Console.Write("\nPlay again? (y to continue, any other key to exit): ");
                var key = Console.ReadKey(true).KeyChar;
                Console.WriteLine();
                if (key != 'y' && key != 'Y')
                {
                    break;
                }
                round++;
            }

            _stats.DisplayStatistics(_settings.NumberOfBoxes, _morty);
        }

       
        private void PlayRound()
        {
            int numberOfBoxes = _settings.NumberOfBoxes;

            Console.WriteLine($"Morty: Oh geez, Rick, I'm gonna hide your portal gun in one of the {numberOfBoxes} boxes, okay?");

            _lastPortalGunProtocolResult = _randomnessProtocol.GenerateFairNumberInteractive(numberOfBoxes, () =>
            {
                return GetRickChoice($"Morty: Rick, enter your number [0,{numberOfBoxes}) so you don’t whine later that I cheated, alright? ", 0, numberOfBoxes - 1);
            });
            int portalGunBox = _lastPortalGunProtocolResult.FinalValue;

            Console.WriteLine("Morty: Okay, okay, I hid the gun.");
            int rickInitialChoice = GetRickChoice($"Morty: What's your guess [0,{numberOfBoxes})? ", 0, numberOfBoxes - 1);
            Console.WriteLine($"Rick: {rickInitialChoice}");

            Console.WriteLine("Morty: Let's, uh, generate another value now, I mean, to select a box to keep in the game.");
            var keepBoxesProtocol = _randomnessProtocol.GenerateFairNumberInteractive(numberOfBoxes - 1, () =>
            {
                return GetRickChoice($"Morty: Rick, enter your number [0,{numberOfBoxes - 1}), and, uh, don’t say I didn’t play fair, okay? ", 0, numberOfBoxes - 2);
            });

            List<int> allBoxes = Enumerable.Range(0, numberOfBoxes).ToList();
            List<int> candidateBoxes = allBoxes.Where(b => b != rickInitialChoice).ToList();
            int extraBox = candidateBoxes[keepBoxesProtocol.FinalValue];
            List<int> remainingClosedBoxes = new List<int> { rickInitialChoice, extraBox };

            Console.WriteLine($"Morty: I'm keeping the box you chose, I mean {rickInitialChoice}, and the box {extraBox}.");
            Console.WriteLine($"Morty: You can switch your box (enter {extraBox}), or, you know, stick with it (enter {rickInitialChoice}).");

            int rickFinalChoice = GetRickChoice("Rick: ", 0, numberOfBoxes - 1);
            Console.WriteLine($"Rick chose box {rickFinalChoice}");

            Console.WriteLine($"Morty: You portal gun is in the box {portalGunBox}.");
            if (rickFinalChoice == portalGunBox)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Morty: Aw jeez, Rick, you actually won this time!");
                Console.ResetColor();
                _stats.RecordGameResult(true);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Morty: Aww man, you lost, Rick. Now we gotta go on one of *my* adventures!");
                Console.ResetColor();
                _stats.RecordGameResult(false);
            }

            if (_lastPortalGunProtocolResult != null)
            {
                Console.WriteLine($"Morty: Aww man, my 1st random value is {_lastPortalGunProtocolResult.MortySecretValue}.");
                Console.WriteLine($"Morty: KEY1={BitConverter.ToString(_lastPortalGunProtocolResult.SecretKeyUsed).Replace("-", "")}");
                Console.WriteLine($"Morty: So the 1st fair number is ({_lastPortalGunProtocolResult.RickInputValue} + {_lastPortalGunProtocolResult.MortySecretValue}) % {numberOfBoxes} = {_lastPortalGunProtocolResult.FinalValue}.");
            }

            if (keepBoxesProtocol != null)
            {
                Console.WriteLine($"Morty: Aww man, my 2nd random value is {keepBoxesProtocol.MortySecretValue}.");
                Console.WriteLine($"Morty: KEY2={BitConverter.ToString(keepBoxesProtocol.SecretKeyUsed).Replace("-", "")}");
                Console.WriteLine($"Morty: Uh, okay, the 2nd fair number is ({keepBoxesProtocol.RickInputValue} + {keepBoxesProtocol.MortySecretValue}) % {numberOfBoxes - 1} = {keepBoxesProtocol.FinalValue}.");
            }
        }


        private int GetRickChoice(string prompt, int min, int max)
        {
            int choice = -1;
            bool isValid = false;
            while (!isValid)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();
                if (int.TryParse(input, out choice) && choice >= min && choice <= max)
                {
                    isValid = true;
                }
                else
                {
                    Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
                }
            }
            return choice;
        }

       
    }
}