using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foxpro2Db
{
    abstract class ConvertBase :IConvertBase
    {
        public string FoxproConnStr
        {
            get;
            set;
        }

        public string DbConnStr
        {
            get;
            set;
        }

        public virtual void Convert()
        {
        }

        abstract public bool IsSetmeal(string prodNo);
    }
}
