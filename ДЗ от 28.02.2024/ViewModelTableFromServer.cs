using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class ViewModelTableFromServer
    {
        private string _id;
        private string _title;
        private string _type;
        private string _color;
        private string _calorie;

        public string Id => _id;
        public string Title => _title;
        public string Type => _type;
        public string Color => _color;
        public string Calorie => _calorie;

        public ViewModelTableFromServer(string id, string title, string type, string color, string calorie)
        {
            _id = id;
            _title = title;
            _type = type;
            _color = color;
            _calorie = calorie;
        }
    }
}
