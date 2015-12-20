using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Xml.Serialization;

namespace projectArduinoFirstTry.Sources
{
    #region Medicine
    public struct Medicine : IComparer<Medicine>
    {
        public string Name { get; set; }

        public DateTime Date { get; set; }
        public long Code { get; set; }
        public string DangersDesc { get; set; }
        public string UserDesc { get; set; }
        public string ImagePath { get; set; }

        public Medicine(string name, DateTime dateTime, int code) : this()
        {
            Name = name;
            Date = dateTime;
            Code = code;
        }

        public int Compare(Medicine x, Medicine y)
        {
            return x.Code.CompareTo(y.Code);
        }
    }
    #endregion

    #region MedicineList

    public class MedicineList
    {
        public MedicineList()
        {
            MedicineVal = new List<Medicine>();
        }

        public List<Medicine> MedicineVal { get; set; }
    }

    #endregion
}
