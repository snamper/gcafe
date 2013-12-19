using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foxpro2Db
{
    class Program
    {
        static void Main(string[] args)
        {
            IConvertBase converter = new Foxpro2SQL();

            converter.FoxproConnStr = @"Provider=VFPOLEDB.1;Data Source=D:\gwr\DATA\lygOrder.dbc";
            //converter.DbConnStr = @"Data Source=192.168.1.200;Initial Catalog=gcafe;Persist Security Info=True;User ID=sa;Password=85bc3e30";
            converter.DbConnStr = @"Data Source=192.168.11.5;Initial Catalog=gcafe;Persist Security Info=True;User ID=sa;Password=GcafeGcafe2012";
            converter.Convert();
        }
    }
}
