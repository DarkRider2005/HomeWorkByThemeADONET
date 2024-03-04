using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfApp1
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string _connectionString;
        private string _taskBar;

        private ModelCommand _connectServerCommand;
        private ModelCommand _disconnectServerCommand;
        private ObservableCollection<ViewModelTableFromServer> _dataServer;

        private SqlConnection _connectionSQLServer;
        private SqlCommand _sQLServerCommand;
        private SqlDataReader _sQLDataReader;

        private ICommand _displayTitlesAndColorsCommand;
        private ICommand _displayMinMaxAvgCalorieCommand;
        private ICommand _displayNumberVegetablesAndFruitsCommand;
        private ICommand _displayNumberVegetablesAndFruitsSelectedColorCommand;
        private ICommand _displayNumberVegetablesAndFruitsAllColorCommand;
        private ICommand _displayVegetablesAndFruitsWithCalorieBelowSpecifiedCommand;
        private ICommand _displayVegetablesAndFruitsWithCalorieInSpecifiedRangeCommand;
        private ICommand _displayNumberVegetableAndFruitsRedOrYellowColorCommand;

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isConnected;

        public ViewModel()
        {
            _dataServer = new ObservableCollection<ViewModelTableFromServer>();
            _connectionString = "Data Source=DARKPROGRAMMER\\MSSQLSERVER01;Initial Catalog = VegetableAndFruits;Integrated Security=True;Connect Timeout=5;";

            _connectServerCommand = new ViewModelCommand(() => _connectionString != String.Empty && !_isConnected, () =>
            {
                try
                {
                    _connectionSQLServer = new SqlConnection(_connectionString);
                    _connectionSQLServer.Open();
                    IsConnected = true;

                    _sQLServerCommand = new SqlCommand("SELECT * FROM VegetableFruits", _connectionSQLServer);
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    DataServer.Add(new ViewModelTableFromServer(_sQLDataReader.GetName(0), _sQLDataReader.GetName(1), _sQLDataReader.GetName(2), _sQLDataReader.GetName(3), _sQLDataReader.GetName(4)));
                    while (_sQLDataReader.Read())
                        DataServer.Add(new ViewModelTableFromServer(_sQLDataReader[0].ToString(), _sQLDataReader[1].ToString(), _sQLDataReader[2].ToString(), _sQLDataReader[3].ToString(), _sQLDataReader[4].ToString()));
                    _sQLDataReader.Close();

                    MessageBox.Show("Подключение успешно!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _disconnectServerCommand = new ViewModelCommand(() => _isConnected, () =>
            {
                _connectionSQLServer.Close();
                _dataServer.Clear();
                IsConnected = false;
                TaskBar = String.Empty;
                MessageBox.Show("Отключение успешно!");
            });

            _displayTitlesAndColorsCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    _sQLServerCommand.CommandText = "SELECT Title\r\nFROM VegetableFruits";
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    TaskBar = "Названия: ";
                    while (_sQLDataReader.Read())
                        TaskBar += _sQLDataReader["Title"].ToString() + ", ";

                    _sQLDataReader.Close();
                    string cmd = "SELECT Distinct Color FROM VegetableFruits";
                    _sQLServerCommand.CommandText = cmd;
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    TaskBar += "\nЦвета: ";
                    while (_sQLDataReader.Read())
                        TaskBar += _sQLDataReader["Color"].ToString() + ", ";
                    _sQLDataReader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayMinMaxAvgCalorieCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    TaskBar = String.Empty;
                    string[] titleCommands = { "Минимальная: ", "Максимальная: ", "Средняя: " };
                    string[] commands = {
                            "SELECT Min(Calorie)\r\nFROM VegetableFruits",
                            "SELECT Max(Calorie)\r\nFROM VegetableFruits",
                            "SELECT AVG(Calorie)\r\nFROM VegetableFruits" };

                    for (int i = 0; i < commands.Length; i++)
                    {
                        _sQLServerCommand.CommandText = commands[i];
                        TaskBar += titleCommands[i] + _sQLServerCommand.ExecuteScalar() + "; ";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayNumberVegetablesAndFruitsCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    TaskBar = String.Empty;
                    string[] titles = { "Кол-во овощей: ", "Кол-во фруктов: " };
                    string[] commands = { "SELECT COUNT(Type)\r\nfrom VegetableFruits\r\nWHERE (Type = 'Овощ')", "SELECT COUNT(Type)\r\nfrom VegetableFruits\r\nWHERE (Type = 'Фрукт')" };
                    for (int i = 0; i < commands.Length; i++)
                    {
                        _sQLServerCommand.CommandText = commands[i];
                        TaskBar += titles[i] + _sQLServerCommand.ExecuteScalar() + "; ";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayNumberVegetablesAndFruitsSelectedColorCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    string findColor = "Красный";
                    _sQLServerCommand.CommandText = $"SELECT COUNT(Title)\r\nfrom VegetableFruits\r\nWHERE (Color = '{findColor}')";
                    TaskBar = $"Количество овощей и фруктов с цветом \"{findColor}\" = " + _sQLServerCommand.ExecuteScalar() + "\nЧтобы произвести поиск по другим цветам - " +
                    "измените переменную findColor в ViewModel() у команды _displayNumberVegetablesAndFruitsSelectedColorCommand. Не стал заморачиваться тут.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayNumberVegetablesAndFruitsAllColorCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    _sQLServerCommand.CommandText = "SELECT Color, \r\n SUM(CASE WHEN Type = 'Фрукт' OR Type = 'Овощ' THEN 1 ELSE 0 END)\r\nFROM VegetableFruits\r\nGROUP BY Color;";
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    TaskBar = String.Empty;

                    while (_sQLDataReader.Read())
                    {
                        TaskBar += "Кол-во фруктов и овощей с цветом ";
                        for (int i = 0; i < _sQLDataReader.FieldCount; i++)
                        {
                            if (i == 0)
                                TaskBar += "\"";

                            TaskBar += _sQLDataReader[i];
                            if (i == 0)
                            {
                                TaskBar += "\"";
                                TaskBar += " = ";
                            }
                        }
                        TaskBar += "\n";
                    }
                    _sQLDataReader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayVegetablesAndFruitsWithCalorieBelowSpecifiedCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    int maxCalorie = 45;
                    _sQLServerCommand.CommandText = $"Select ID,Title,Calorie\r\nfrom VegetableFruits\r\nWHERE (Calorie < {maxCalorie})";
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    TaskBar = String.Empty;

                    while( _sQLDataReader.Read())
                    {
                        for (int i = 0; i < _sQLDataReader.FieldCount; i++)
                        {
                            TaskBar += _sQLDataReader[i] + " ";
                            if (i == 0)
                                TaskBar += ") ";
                        }
                        TaskBar += "\n";
                    }
                    TaskBar += "Чтобы указать максимальное кол-во калорий - зайдите в ViewModel() и у команды _displayVegetablesAndFruitsWithCalorieBelowSpecifiedCommand измените maxCalorie.";
                    _sQLDataReader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayVegetablesAndFruitsWithCalorieInSpecifiedRangeCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    int min = 10, max = 50;
                    _sQLServerCommand.CommandText = $"SELECT ID,Title, Calorie\r\nFROM VegetableFruits\r\nWHERE Type IN ('Фрукт', 'Овощ') \r\nAND Calorie BETWEEN {min} AND {max};";
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    TaskBar = String.Empty;

                    while (_sQLDataReader.Read())
                    {
                        for (int i = 0; i < _sQLDataReader.FieldCount; i++)
                        {
                            TaskBar += _sQLDataReader[i] + " ";
                            if (i == 0)
                                TaskBar += ") ";
                        }
                        TaskBar += "\n";
                    }
                    TaskBar += "Чтобы изменить диапазон - зайдите в ViewModel() и команды _displayVegetablesAndFruitsWithCalorieInSpecifiedRangeCommand измените min max.Тут тоже не стал заморачиваться";
                    _sQLDataReader.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
            _displayNumberVegetableAndFruitsRedOrYellowColorCommand = new ViewModelCommand(() => true, () =>
            {
                try
                {
                    _sQLServerCommand.CommandText = "Select ID,Title\r\nfrom VegetableFruits\r\nWHERE (Color = 'Красный' OR Color = 'Желтый')";
                    _sQLDataReader = _sQLServerCommand.ExecuteReader();

                    TaskBar = String.Empty;

                    while (_sQLDataReader.Read())
                    {
                        for (int i = 0; i < _sQLDataReader.FieldCount; i++)
                        {
                            TaskBar += _sQLDataReader[i];
                            if (i == 0)
                                TaskBar += ") ";
                        }
                        TaskBar += "\n";
                    }

                    _sQLDataReader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                _isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
                _connectServerCommand.RaiseCanExecute();
                _disconnectServerCommand.RaiseCanExecute();
            }
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                OnPropertyChanged(nameof(ConnectionString));
                _connectServerCommand.RaiseCanExecute();
            }
        }
        public string TaskBar
        {
            get => _taskBar;
            set
            {
                _taskBar = value;
                OnPropertyChanged(nameof(TaskBar));
            }
        }

        public ICommand ConnectServerCommand => _connectServerCommand;
        public ICommand DisconnectServerCommand => _disconnectServerCommand;
        public ICommand DisplayTitlesAndColorsVegetablesFruitsCommand => _displayTitlesAndColorsCommand;
        public ICommand DisplayMinMaxAvgCalorieVegetablesFruitsCommand => _displayMinMaxAvgCalorieCommand;
        public ICommand DisplayNumberVegetablesAndFruitsCommand => _displayNumberVegetablesAndFruitsCommand;
        public ICommand DisplayNumberVegetablesAndFruitsSelectedColorCommand => _displayNumberVegetablesAndFruitsSelectedColorCommand;
        public ICommand DisplayNumberVegetablesAndFruitsAllColorCommand => _displayNumberVegetablesAndFruitsAllColorCommand;
        public ICommand DisplayVegetablesAndFruitsWithCalorieBelowSpecifiedCommand => _displayVegetablesAndFruitsWithCalorieBelowSpecifiedCommand;
        public ICommand DisplayVegetablesAndFruitsWithCalorieInSpecifiedRangeCommand => _displayVegetablesAndFruitsWithCalorieInSpecifiedRangeCommand;
        public ICommand DisplayNumberVegetableAndFruitsRedOrYellowColorCommand => _displayNumberVegetableAndFruitsRedOrYellowColorCommand;

        public ObservableCollection<ViewModelTableFromServer> DataServer => _dataServer;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}