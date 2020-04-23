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
        //Singleton
        public static MarvelDBWindow Window;
        public HeroTable dbTable;
        public static bool initDone = false;


        public MarvelDBWindow()
        {
            InitializeComponent();

            //Singleton Static Reference
            Window = this;

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

            AddHeroButton.Click += new RoutedEventHandler(dbTable.AddHero);
            RemoveHeroButton.Click += new RoutedEventHandler(dbTable.RemoveSelectedHero);

            //Start tracking changes to the table
            initDone = true;
        }
    }

    //Race Enum
    [System.Serializable]
    public enum Race { Human = 1, Alien = 2, Animal = 3, Supernatural = 4 }
    
    public class SuperHero : INotifyPropertyChanged, IEditableObject
    {
        private string _heroName;   //Private Internally Used Field
        public string HeroName      //Publicly Accessible Field
        {
            get
            {
                return _heroName;
            }

            set
            {
                if (_heroName == value) return;
                _heroName = value;
                OnPropertyChanged("HeroName");
            }
        }

        private string _realName;   //Private Internally Used Field
        public string RealName      //Publicly Accessible Field
        {
            get
            {
                return _realName;
            }

            set
            {
                if (_realName == value) return;
                _realName = value;
                OnPropertyChanged("RealName");
            }
        }

        private double _weight;     //Private Internally Used Field
        public double Weight        //Publicly Accessible Field
        {
            get
            {
                return _weight;
            }

            set
            {
                if (_weight == value) return;
                _weight = value;
                OnPropertyChanged("Weight");
            }
        }


        private double _height;     //Private Internally Used Field
        public double Height        //Publicly Accessible Field
        {
            get
            {
                return _height;
            }

            set
            {
                if (_height == value) return;
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        private Race _race;         //Private Internally Used Field

        public string RaceStr
        {
            get { return _race.ToString(); }
        }

        //public int RaceId
        //{
        //    get
        //    {
        //        return (int)_race;
        //    }

        //    set
        //    {
        //        if (value > 4) { RaceId = 4; return; }
        //        Race = (Race)value;
        //    }
        //}

        public IEnumerable<Race> RaceTypeValues
        {
            get
            {
                return Enum.GetValues(typeof(Race))
                    .Cast<Race>();
            }
        }

        public Race Race            //Publicly Accessible Field
        {
            get
            {
                return _race;
            }

            set
            {
                if (_race == value) return;
                _race = value;
                OnPropertyChanged("Race");
            }
        }

        private bool _inMovie;         //Private Internally Used Field
        public bool InMovie         //Publicly Accessible Field
        {
            get
            {
                return _inMovie;
            }

            set
            {
                if (_inMovie == value) return;
                _inMovie = value;
                OnPropertyChanged("InMovie");
            }
        }

        public SuperHero(string heroName, string realName, double weight, double height, Race race, bool inMovie)
        {
            HeroName = heroName;
            RealName = realName;
            Weight = weight;
            Height = height;
            Race = race;
            InMovie = inMovie;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
                if (MarvelDBWindow.initDone == true)
                {
                    MarvelDBWindow.Window.dbTable.SaveToCSV("Database.csv"); 
                }
            }
        }

        #endregion

        #region IEditableObject

        private SuperHero _backupCopy;
        private bool _inEdit;

        public void BeginEdit()
        {
            if (_inEdit) return;
            _inEdit = true;
            _backupCopy = this.MemberwiseClone() as SuperHero;
        }

        public void CancelEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            this.HeroName = _backupCopy.HeroName;
            this.RealName = _backupCopy.RealName;
            this.Weight = _backupCopy.Weight;
            this.Height = _backupCopy.Height;
            this.Race = _backupCopy.Race;
            this.InMovie = _backupCopy.InMovie;
        }

        public void EndEdit()
        {
            if (!_inEdit) return;
            _inEdit = false;
            _backupCopy = null;
        }

        #endregion
    }

    [System.Serializable]
    public class HeroTable
    {
        public ObservableCollection<SuperHero> heroesByName { get; set; }

        //Add a new hero entry
        public void AddHero(object sender, RoutedEventArgs e)
        {
            SuperHero newHero = new SuperHero("New Hero", "New Hero", 0, 0, Race.Human, false);
            heroesByName.Add(newHero);
        }

        //Remove Hero Entry
        public void RemoveSelectedHero(object sender, RoutedEventArgs e)
        {
            int index = MarvelDBWindow.Window.HeroTableGrid.SelectedIndex;
            if (index != -1)
            {
                heroesByName.Remove(heroesByName[index]); 
            }

        }

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

                    Race tempRace = Enum.Parse<Race>(tempList[4]);

                    //Add the row's hero to the csv file
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

                    row.Add(heroValue.HeroName);
                    row.Add(heroValue.RealName);
                    row.Add(heroValue.Weight.ToString());
                    row.Add(heroValue.Height.ToString());
                    row.Add(heroValue.Race.ToString());
                    row.Add(heroValue.InMovie.ToString());


                    writer.WriteRow(row);
                }

            }
        }

    }
}
