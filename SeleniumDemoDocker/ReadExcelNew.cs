using System;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SeleniumDemoDocker
{
    internal class ReadExcelNew
    {
        public string[,] ReadExcel(string fileName, string sheetName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fs, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();

                    //WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    WorksheetPart worksheetPart = GetWorksheetFromSheetName(workbookPart, sheetName);
                    Worksheet sheet = worksheetPart.Worksheet;
                    string dimensions = string.Empty;
                    dimensions = sheet.SheetDimension.Reference.InnerText;

                    var cells = sheet.Descendants<Cell>();
                    var rows = sheet.Descendants<Row>();

                    int colInx = 0;
                    int rowInx = 0;
                    string value = "";

                    int numOfColumns = 0;
                    int numOfRows = 0;
                    CalculateDataTableSize(dimensions, ref numOfColumns, ref numOfRows);

                    SharedStringTablePart stringTablePart = spreadsheetDocument.WorkbookPart.SharedStringTablePart;
                    SheetData sheetData = sheet.GetFirstChild<SheetData>();

                    string[,] cellValues = new string[numOfColumns, numOfRows];

                    foreach (Row row in rows)
                    {
                        for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                        {
                            //  *DON'T* assume there's going to be one XML element for each column in each row...
                            Cell cell = row.Descendants<Cell>().ElementAt(i);
                            if (cell.CellValue == null || cell.CellReference == null)
                                continue;                       //  eg when an Excel cell contains a blank string

                            //  Convert this Excel cell's CellAddress into a 0-based offset into our array (eg "G13" -> [6, 12])
                            colInx = GetColumnIndexByName(cell.CellReference);             //  eg "C" -> 2  (0-based)
                            rowInx = GetRowIndexFromCellAddress(cell.CellReference) - 1;     //  Needs to be 0-based

                            //  Fetch the value in this cell
                            value = cell.CellValue.InnerXml;
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                value = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                            }

                            cellValues[colInx, rowInx] = value;
                        }
                    }

                    return cellValues;
                }
            }
        }

        public string[,] ReadExcelUlta(string fileName, string sheetName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(fs, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    SharedStringTablePart sstpart = workbookPart.GetPartsOfType<SharedStringTablePart>().First();

                    //WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    WorksheetPart worksheetPart = GetWorksheetFromSheetName(workbookPart, sheetName);
                    Worksheet sheet = worksheetPart.Worksheet;
                    string dimensions = string.Empty;
                    dimensions = sheet.SheetDimension.Reference.InnerText;

                    var cells = sheet.Descendants<Cell>();
                    var rows = sheet.Descendants<Row>();

                    int colInx = 0;
                    int rowInx = 0;
                    string value = "";

                    int numOfColumns = 0;
                    int numOfRows = 0;
                    CalculateDataTableSize(dimensions, ref numOfColumns, ref numOfRows);

                    SharedStringTablePart stringTablePart = spreadsheetDocument.WorkbookPart.SharedStringTablePart;
                    SheetData sheetData = sheet.GetFirstChild<SheetData>();

                    string[,] cellValues = new string[numOfRows, numOfColumns];

                    foreach (Row row in rows)
                    {
                        for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                        {
                            //  *DON'T* assume there's going to be one XML element for each column in each row...
                            Cell cell = row.Descendants<Cell>().ElementAt(i);
                            if (cell.CellValue == null || cell.CellReference == null)
                                continue;                       //  eg when an Excel cell contains a blank string

                            //  Convert this Excel cell's CellAddress into a 0-based offset into our array (eg "G13" -> [6, 12])
                            colInx = GetColumnIndexByName(cell.CellReference);             //  eg "C" -> 2  (0-based)
                            rowInx = GetRowIndexFromCellAddress(cell.CellReference) - 1;     //  Needs to be 0-based

                            //  Fetch the value in this cell
                            value = cell.CellValue.InnerXml;
                            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
                            {
                                value = stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
                            }

                            cellValues[rowInx, colInx] = value;
                        }
                    }

                    return cellValues;
                }
            }
        }

        private int GetRowIndexFromCellAddress(string cellAddress)
        {
            //  Convert an Excel CellReference column into a 1-based row index
            //  eg "D42"  ->  42
            //     "F123" ->  123
            string rowNumber = System.Text.RegularExpressions.Regex.Replace(cellAddress, "[^0-9 _]", "");
            return int.Parse(rowNumber);
        }

        private int GetColumnIndexByName(string cellAddress)
        {
            //  Convert an Excel CellReference column into a 0-based column index
            //  eg "D42" ->  3
            //     "F123" -> 5
            var columnName = System.Text.RegularExpressions.Regex.Replace(cellAddress, "[^A-Z_]", "");
            int number = 0, pow = 1;
            for (int i = columnName.Length - 1; i >= 0; i--)
            {
                number += (columnName[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number - 1;
        }

        private void CalculateDataTableSize(string dimensions, ref int numOfColumns, ref int numOfRows)
        {
            //  How many columns & rows of data does this Worksheet contain ?
            //  We'll read in the Dimensions string from the Excel file, and calculate the size based on that.
            //      eg "B1:F4" -> we'll need 6 columns and 4 rows.
            //
            //  (We deliberately ignore the top-left cell address, and just use the bottom-right cell address.)
            try
            {
                string[] parts = dimensions.Split(':');     // eg "B1:F4"
                if (parts.Length != 2)
                    throw new Exception("Couldn't find exactly *two* CellAddresses in the dimension");

                numOfColumns = 1 + GetColumnIndexByName(parts[1]);     //  A=1, B=2, C=3  (1-based value), so F4 would return 6 columns
                numOfRows = GetRowIndexFromCellAddress(parts[1]);
            }
            catch
            {
                throw new Exception("Could not calculate maximum DataTable size from the worksheet dimension: " + dimensions);
            }
        }

        private WorksheetPart GetWorksheetFromSheetName(WorkbookPart workbookPart, string sheetName)
        {
            Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name == sheetName);
            if (sheet == null) throw new Exception(string.Format("Could not find sheet with name {0}", sheetName));
            else return workbookPart.GetPartById(sheet.Id) as WorksheetPart;
        }

        public Sheets GetAllWorksheets(string fileName)
        {
            Sheets theSheets = null;
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (SpreadsheetDocument document =
                    SpreadsheetDocument.Open(fs, false))
                {
                    WorkbookPart wbPart = document.WorkbookPart;
                    theSheets = wbPart.Workbook.Sheets;
                }
            }
            return theSheets;
        }
    }
}