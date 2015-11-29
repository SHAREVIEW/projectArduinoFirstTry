using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projectArduinoFirstTry.Sources
{
    #region Medicine
    internal struct Medicine
    {
        public string Name;
        public DateTime Date;
        public long Code;

        public Medicine(string name, DateTime dateTime, int code) : this()
        {
            Name = name;
            Date = dateTime;
            Code = code;
        }
    }
    #endregion
}
