using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace projectArduinoFirstTry.Sources
{
    #region Medicine
    internal struct Medicine
    {
        public string Name;
        public DateTime Date;
        public long Code;
        public string DangersDesc;
        public FlowDocument UserDesc;

        public Medicine(string name, DateTime dateTime, int code) : this()
        {
            Name = name;
            Date = dateTime;
            Code = code;
        }
    }
    #endregion
}
