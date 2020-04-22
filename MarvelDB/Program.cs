using System;
using System.IO;
using System.Collections.Generic;
using ConsoleTables;
using CSVUtility;

namespace CSharp_MinorBoss_ConsoleApp
{

    public enum Race { Human, Alien, Animal, Supernatural }

    class Program
    {
        /// <summary>
        /// Source: https://www.codeproject.com/Articles/415732/Reading-and-Writing-CSV-Files-in-Csharp
        /// </summary>
        
        public static HeroTable dbTable = new HeroTable();

        static void WriteTest()
        {

        }

        static void ReadTest()
        {

        }

        //static void Main(string[] args)
        //{
        //    //Check If File Exists and if not create it
        //    if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", "Database.csv")))
        //    {
        //        Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB"));
        //        File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", "Database.csv"));
        //    }

        //    //Instantiate 
        //    dbTable = new HeroTable();
        //    dbTable.LoadFromCSV("Database.csv");

        //    while (true)
        //    {
        //        #region Scope:RootMenu
        //        int input = 0;  //Reused throughout entire program
        //        string[] options = { "View Marvel DB", "Add Superhero", "Edit Superhero", "Remove Superhero", "Exit System" };  //Reused throughout the program

        //        input = QueryUserAction("Welcome To MarvelDB Superhero Database Management!\nWhat would you like to do?", options);
        //        switch (input)
        //        {
        //            case 1:
        //                //Console.WriteLine("Not Yet Implimented");
        //                dbTable.PrintTable();
        //                WaitForEnterPress();
        //                break;

        //            case 2: //Add Hero
        //                dbTable.AddSuperHero();
        //                WaitForEnterPress();
        //                break;

        //            case 3:
        //                dbTable.EditSuperHero();
        //                WaitForEnterPress();
        //                break;

        //            case 4:
        //                dbTable.RemoveHero();
        //                WaitForEnterPress();
        //                break;

        //            case 5:
        //                return;

        //                //No need for default state as validation is handled in QueryUserAction()

        //        }
        //        #endregion
        //    }
        //}

        static int QueryUserAction(string promptText, string[] optionsList)
        {
            bool GotValidInput = false;
            string inputStr = "";
            int inputInt = 0;

            while (!GotValidInput)
            {
                Console.Clear();                                                //Flush the console
                Console.WriteLine(promptText);                                  //Print Prompt  
                for (int i = 0; i < optionsList.Length; i++)                    //Print Options
                {
                    Console.WriteLine($"{i + 1}. {optionsList[i]}");
                }

                try                                                             //Gather Input and Verify that it is in range
                {
                    inputStr = Console.ReadLine();                          //Collect input
                    inputInt = Convert.ToInt32(inputStr);                   //Convert to int
                    if (inputInt <= optionsList.Length && inputInt > 0)     //Return Valid
                    {
                        GotValidInput = true;
                    }
                    else
                    {
                        throw new IndexOutOfRangeException($"Number must be between 1 and {optionsList.Length} (Indexing from 1 for idiot proofing)");
                    }
                }
                catch (Exception)
                {
                    continue;                                               //Allow the user to try again
                }

            }

            return inputInt;

        }

        public static void WaitForEnterPress()
        {
            Console.WriteLine("Press ENTER to continue...");
            bool hasPressedEnter = false;
            while (!hasPressedEnter)
            {
                ConsoleKeyInfo keyPress = Console.ReadKey(); //Get the next keypress

                if (keyPress.Key == ConsoleKey.Enter)
                {
                    hasPressedEnter = true;
                }
            }
        }
    }

    public class SuperHero
    {
        public string heroName = "";
        public string realName = "";
        public double weight = 0d;
        public double height = 0d;
        public Race race = Race.Human;
        public bool inMovie = false;

        public SuperHero(string _heroName, string _realName, double _weight, double _height, Race _race, bool _inMovie)
        {
            heroName = _heroName;
            realName = _realName;
            weight = _weight;
            height = _height;
            race = _race;
            inMovie = _inMovie;
        }

        public void ModifyData(string _heroName, string _realName, double _weight, double _height, Race _race, bool _inMovie)
        {
            heroName = _heroName;
            realName = _realName;
            weight = _weight;
            height = _height;
            race = _race;
            inMovie = _inMovie;
        }


    }
    public class HeroTable
    {
        public readonly SortedDictionary<string, SuperHero> heroesByName = new SortedDictionary<string, SuperHero>();

        public void PrintTable()
        {
            Console.Clear();
            Console.WriteLine("MarvelDB Superhero Database:\n");
            ConsoleTable consoleTable = new ConsoleTable("Alias/Code Name", "Real Name", "Weight", "Height", "Race", "In Movie");
            foreach (SuperHero hero in heroesByName.Values)
            {
                consoleTable.AddRow(hero.heroName, hero.realName, hero.weight.ToString(), hero.height.ToString(), hero.race.ToString(), hero.inMovie.ToString());
            }
            consoleTable.Write();
            
        }

        //Load the data from the csv database
        public void LoadFromCSV(string fileName)
        {

            // Read sample data from CSV file
            using (CsvFileReader reader = new CsvFileReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", fileName)))
            {
                heroesByName.Clear();
                List<string> tempList = new List<string>();
                CsvRow row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    tempList.Clear();

                    //Load row data into list
                    foreach (string value in row)
                    {
                        tempList.Add(value);
                    }

                    Race tempRace;
                    tempRace = Enum.Parse<Race>(tempList[4]);

                    //Add the row's hero to th
                    heroesByName.Add(tempList[0], new SuperHero(tempList[0], tempList[1], Convert.ToDouble(tempList[2]), Convert.ToDouble(tempList[3]), tempRace, Convert.ToBoolean(tempList[5])));

                    Console.WriteLine();
                }
            } 
            
        }

        //Save the data to the csv database
        public void SaveToCSV(string fileName)
        {
            // Write sample data to CSV file
            using (CsvFileWriter writer = new CsvFileWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", fileName)))   //CHECK IF THIS IS MEANT TO BE THE FULL PATH!!!!
            {

                foreach (SuperHero heroValue in heroesByName.Values)
                {
                    CsvRow row = new CsvRow();

                    row.Add(heroValue.heroName);
                    row.Add(heroValue.realName);
                    row.Add(heroValue.weight.ToString());
                    row.Add(heroValue.weight.ToString());
                    row.Add(heroValue.race.ToString());
                    row.Add(heroValue.inMovie.ToString());


                    writer.WriteRow(row);
                }
                
            }
        }

        public void AddSuperHero()
        {
            Console.Clear();
            bool validStats = false;
            string _heroName = "";
            string _realName = "";
            double _weight = 0d;
            double _height = 0d;
            Race _race = Race.Human;
            bool _inMovie = false;

            while (!validStats)
            {
                Console.Clear();

                try
                {
                    //Hero Name
                    Console.WriteLine("Enter Superhero's Hero Alias");
                    _heroName = Console.ReadLine();
                    if (heroesByName.ContainsKey(_heroName))
                    {
                        Console.WriteLine("A hero with that name already exists.\nTry editing it instead.");
                        Program.WaitForEnterPress();

                        continue; //Invalid
                    }

                    //Real Name
                    Console.WriteLine("Enter Superhero's Real Name");
                    _realName = Console.ReadLine();

                    //Weight
                    Console.WriteLine("Enter Superhero's Weight In KG (Numeric, Rounded to two decimal places)");
                    _weight = Convert.ToDouble(Console.ReadLine());

                    //Height
                    Console.WriteLine("Enter Superhero's Height in M (Numeric, Rounded to two decimal places)");
                    _height = Convert.ToDouble(Console.ReadLine());

                    //Race
                    Console.WriteLine("Enter Superhero's Race (not case sensitive)\nValid Races: Human, Alien, Animal, Supernatural");
                    if (!Enum.TryParse<Race>(Console.ReadLine(), true, out _race))    //Attempt to parse Race and go back to the start if invalid
                    {
                        continue;
                    }

                    //In Movie?
                    Console.WriteLine("Has the SuperHero been in a movie yet? Y/N");
                    string YNSame = Console.ReadLine();

                    if (YNSame.ToUpper() == "Y")
                    {
                        _inMovie = true;
                    }
                    else if (YNSame.ToUpper() == "N")
                    {
                        _inMovie = false;
                    }

                    #region Samedetection
                    if (_realName.ToUpper() == "SAME")
                    {
                        _realName = Program.dbTable.heroesByName[_heroName].realName;
                    }
                    else if (_realName.ToUpper() == "SAME")
                    {
                        _realName = Program.dbTable.heroesByName[_heroName].realName;
                    }
                    #endregion

                    heroesByName.Add(_heroName, new SuperHero(_heroName, _realName, _weight, _height, _race, _inMovie));
                    SaveToCSV("Database.csv");
                    validStats = true;
                    Console.WriteLine();
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void EditSuperHero()
        {
            Console.Clear();
            bool validStats = false;
            string _heroName = "";
            string _realName = "";
            string _weight = "";
            string _height = "";
            Race _race = Race.Human;
            string _raceStr = "";
            bool _inMovie = false;

            while (!validStats)
            {
                Console.Clear();

                try
                {
                    //Hero Name
                    Console.WriteLine("Enter Hero Alias of the Superhero you want to edit.");
                    _heroName = Console.ReadLine();
                    if (!heroesByName.ContainsKey(_heroName))
                    {
                        Console.WriteLine("The database does not contain a hero with that name.\nTry Creating it instead");
                        Program.WaitForEnterPress();

                        return; //Invalid
                    }

                    //Usage note
                    Console.WriteLine("\nNOTE: type \'same\' (not case sensitive) to keep the value the same\n\n");

                    //Real Name
                    Console.WriteLine("Enter Superhero's Real Name");
                    _realName = Console.ReadLine();

                    //Weight
                    Console.WriteLine("Enter Superhero's Weight In KG (Numeric, Rounded to two decimal places)");
                    _weight = Console.ReadLine();
                    //Validate
                    //Validate if not same
                    if (_weight.ToUpper() != "SAME")
                    {
                        Convert.ToInt32(_weight);
                    }

                    //Height
                    Console.WriteLine("Enter Superhero's Height in M (Numeric, Rounded to two decimal places)");
                    _height = Console.ReadLine();
                    //Validate if not same
                    if (_height.ToUpper() != "SAME")
                    {
                        Convert.ToInt32(_height); 
                    }

                    //Race
                    Console.WriteLine("Enter Superhero's Race (not case sensitive)\nValid Races: Human, Alien, Animal, Supernatural");
                    _raceStr = Console.ReadLine();
                    if (!Enum.TryParse<Race>(_raceStr, true, out _race) && _raceStr.ToUpper() != "SAME")    //Attempt to parse Race and go back to the start if invalid
                    {
                        continue;
                    }

                    //In Movie?
                    Console.WriteLine("Has the SuperHero been in a movie yet? Y/N");
                    string YNSame = Console.ReadLine();

                    if (YNSame.ToUpper() == "Y")
                    {
                        _inMovie = true;
                    }
                    else if (YNSame.ToUpper() == "N")
                    {
                        _inMovie = false;
                    }

                    SuperHero existingData = Program.dbTable.heroesByName[_heroName];

                    #region Samedetection
                    if (_realName.ToUpper() == "SAME")
                    {
                        _realName = existingData.realName;
                    }
                    if (_weight.ToUpper() == "SAME")
                    {
                        _weight = existingData.weight.ToString();
                    }
                    if (_height.ToUpper() == "SAME")
                    {
                        _height = existingData.height.ToString();
                    }
                    if (_raceStr.ToUpper() == "SAME")
                    {
                        _race = existingData.race;
                    }
                    if(YNSame.ToUpper() == "SAME")
                    {
                        _inMovie = existingData.inMovie;
                    }
                    #endregion

                    double _doubleweight = Convert.ToDouble(_weight);
                    double _doubleheight = Convert.ToDouble(_height);

                    //Remove Key For this hero (so it can be added with new values)
                    heroesByName.Remove(_heroName);
                    heroesByName.Add(_heroName, new SuperHero(_heroName, _realName, _doubleheight, _doubleweight, _race, _inMovie));
                    SaveToCSV("Database.csv");
                    validStats = true;
                    Console.WriteLine();
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void RemoveHero()
        {
            //Query The name of the hero to remove
            Console.WriteLine("Enter name of hero to remove");
            string _heroName = Console.ReadLine();

            //Check that the hero is in the database
            if (!heroesByName.ContainsKey(_heroName))
            {
                Console.WriteLine("Could not remove hero: No hero with that name exists in database.");
                Program.WaitForEnterPress();
                return;
            }

            //Prompt for Confirmation
            Console.Clear();
            Console.WriteLine($"Are you sure you want to delete this hero?\n\'{_heroName}\' will be lost forever! (A long time)\nWrite the name of this hero below to confirm.");
            string ConfirmationStr = Console.ReadLine();

            if (ConfirmationStr == _heroName)
            {
                //Remove them
                heroesByName.Remove(_heroName);
                Console.Clear(); 
            }
        }
    }
}
