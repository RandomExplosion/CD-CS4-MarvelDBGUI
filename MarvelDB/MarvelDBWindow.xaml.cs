using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using CSVUtility;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;

namespace MarvelDB
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MarvelDBWindow : Window
    {

        public HeroTable dbTable;

        public MarvelDBWindow()
        {
            InitializeComponent();

            //Check If Save File Exists and if not create it
            if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", "Database.csv")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB"));
                File.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", "Database.csv"));
            }

            dbTable = new HeroTable();
            dbTable.LoadFromCSV("Database.csv");

            //Load the database into a table
            HeroTableGrid.DataContext = dbTable.heroesByName;
            HeroTableGrid.CellEditEnding += HeroTableGrid_CellEditEnding;
        }

        //ValidateInput
        private void HeroTableGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //TODO: IMPLIMENT DATA VALIDATION
        }

    }

    //Race Enum
    [System.Serializable]
    public enum Race { Human = 1, Alien = 2, Animal = 3, Supernatural = 4 }

    public class SuperHero /*: INotifyPropertyChanged*/
    {
        public string heroName { get; set; } /*{ get { return heroName; } set { heroName = value; RaisePropertyChanged("heroName"); } }*/
        public string realName { get; set; }
        public double weight { get; set; }
        public double height { get; set; }
        public Race race { get; set; }
        public bool inMovie { get; set; }

        public SuperHero(string _heroName, string _realName, double _weight, double _height, Race _race, bool _inMovie)
        {
            heroName = _heroName;
            realName = _realName;
            weight = _weight;
            height = _height;
            race = _race;
            inMovie = _inMovie;
        }

        //PropertyChanged Event
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void RaisePropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}

        //Unused

        //public void ModifyData(string _heroName, string _realName, double _weight, double _height, Race _race, bool _inMovie)
        //{
        //    heroName = _heroName;
        //    realName = _realName;
        //    weight = _weight;
        //    height = _height;
        //    race = _race;
        //    inMovie = _inMovie;
        //}


    }

    [System.Serializable]
    public class HeroTable
    {
        public ObservableCollection<SuperHero> heroesByName { get; set; }

        //Load the data from the csv database
        public void LoadFromCSV(string fileName)
        {

            // Read sample data from CSV file
            using (CsvFileReader reader = new CsvFileReader(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MarvelDB", fileName)))
            {
                heroesByName = new ObservableCollection<SuperHero>();
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
                    heroesByName.Add(new SuperHero(tempList[0], tempList[1], Convert.ToDouble(tempList[2]), Convert.ToDouble(tempList[3]), tempRace, Convert.ToBoolean(tempList[5])));

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

                foreach (SuperHero heroValue in heroesByName)
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

    }
}
