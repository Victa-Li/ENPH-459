using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;

namespace readandwrite
{
    class Program
    {
        static void Main(string[] args)
        {
            Microsoft.Office.Interop.Excel.Application oXL;
            Microsoft.Office.Interop.Excel._Workbook oWB;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            Microsoft.Office.Interop.Excel.Range oRng;
            object misvalue = System.Reflection.Missing.Value;

            oXL = new Microsoft.Office.Interop.Excel.Application();
            oXL.Visible = true;
            oWB = (Microsoft.Office.Interop.Excel._Workbook)(oXL.Workbooks.Add(""));
            
            oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet;
            oSheet.Cells[1, 1] = "Target Accel x";
            oSheet.Cells[1, 2] = "Target Accel y";
            oSheet.Cells[1, 3] = "Target Accel z";
            oSheet.Cells[1, 4] = "Actual Accel x";
            oSheet.Cells[1, 5] = "Actual Accel y";
            oSheet.Cells[1, 6] = "Actual Accel z";
            oSheet.get_Range("A1", "D1").Font.Bold = true;
            oSheet.get_Range("A1", "D1").VerticalAlignment =
                Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            string text = System.IO.File.ReadAllText("dt.txt");
            string[] context = System.IO.File.ReadAllLines(@"dt.txt");
            string[] stringSeparators1 = new string[] { "Target Accel,", ", Sim Accel" };
            string[] stringSeparators2 = new string[] { "Total Accel, " };
            string[] stringsep = { ","};
            int linenumber = 2;
            foreach (string line in context)
            {
                string[] str1 = line.Split(stringSeparators1, StringSplitOptions.None);
                string[] str1e = str1[1].Split(stringsep,StringSplitOptions.None);
                string[] str2 = line.Split(stringSeparators2, StringSplitOptions.None);
                string[] str2e = str2[1].Split(stringsep, StringSplitOptions.None);
                oSheet.Cells[linenumber, 1] = double.Parse(str1e[0]);
                oSheet.Cells[linenumber, 2] = double.Parse(str1e[1]);
                oSheet.Cells[linenumber, 3] = double.Parse(str1e[2]);
                oSheet.Cells[linenumber, 4] = double.Parse(str2e[0]);
                oSheet.Cells[linenumber, 5] = double.Parse(str2e[1]);
                oSheet.Cells[linenumber, 6] = double.Parse(str2e[2]);
                linenumber++;
            }
            oWB.SaveAs("C:\\Users\\lxiang\\ENPH-459\\testn.xls", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault, Type.Missing, Type.Missing,
                    false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            oWB.Close();
        }
    }
}
