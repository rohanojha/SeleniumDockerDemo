using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SeleniumDemoDocker
{
    internal class HtmlReport
    {
        # region Constructor

        //public static string FileName;

        /// <summary>
        /// This is the constructor method which appends the HTML report name with the timestamp to make it unique
        /// </summary>
        /// <param name="fileName1">The file name specified while creating instance of the object.</param>
        public HtmlReport(string fileName1)
        {
            string datetimeString = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            FileName = (fileName1 + "-" + datetimeString + ".html");
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///This method creates the table header for the report to be stored
        /// </summary>
        /// <param name="headerData">A String array with the required table headers</param>
        public void AddHeader(string[] headerData)
        {
            string reportContent2 = "";
            int colLength = headerData.Length;

            if (colLength > 0)
            {
                reportContent2 = reportContent2 + "<br><br> \n";
                reportContent2 = reportContent2 + "<center>";
                reportContent2 = reportContent2 + "<table id ='results' class=" + "sortable" +
                                 " width:auto cellpadding=2 cellspacing=0 border=" + "1" + ">";
                reportContent2 = reportContent2 + "<tbody>";
                reportContent2 = reportContent2 + "<tr>";

                int i = 0;

                while (i < headerData.Length)
                {
                    if (i == 1 || i == 2)
                    {
                        reportContent2 = reportContent2 + "<td class=bborder_left width=30%>" + headerData[i] + "</td>";
                    }
                    else
                    {
                        reportContent2 = reportContent2 + "<td class=bborder_left width=7%>" + headerData[i] + "</td>";
                    }
                    i++;
                }
                reportContent2 = reportContent2 + "</tr>";
            }

            using (var sw = new StreamWriter(FileName, true))
            {
                sw.WriteLine(reportContent2);
            }
        }

        public void AddDataNonTabular(string headerTitle, string data)
        {
            using (var sw = new StreamWriter(FileName, true))
            {
                sw.WriteLine("<br><br><br><br><br>");
                sw.WriteLine("       <table class=testRunDetails align='left'  width=auto> ");

                sw.WriteLine("       </tr>");
                sw.WriteLine("         <tr>");

                sw.WriteLine("        <td class=bborder_left>" + headerTitle + "</td>");
                sw.WriteLine("       </tr>");
                sw.WriteLine("         <tr>");

                sw.WriteLine("        <td>" + data + "</td>");
                sw.WriteLine("       </tr>");
                sw.WriteLine("      </table> ");
            }
        }

        /// <summary>
        /// This method adds the rows with test reports data
        /// </summary>
        /// <param name="reportData">A String array with the required report data.</param>
        public void AddReportData(string[] reportData)
        {
            string reportContent2 = "";
            int lenRep = reportData.Length;

            while (lenRep < reportData.Length)
            {
                reportData[lenRep] = "";
                lenRep++;
            }

            int i = 0;
            while (i < reportData.Length)
            {
                if (reportData[i] == null)
                {
                    reportData[i] = "---";
                }
                else if (reportData[i].Equals(string.Empty))
                {
                    reportData[i] = "---";
                }
                i++;
            }

            if (reportData.Length > 0)
            {
                reportContent2 = reportContent2 + "<tr>";

                int j = 0;

                while (j < reportData.Length)
                {
                    if (reportData[j].Equals("Pass", StringComparison.CurrentCultureIgnoreCase))
                    {
                        reportContent2 = reportContent2 + "<td class=border_left bgcolor=green><p class=normal_text>" +
                                         reportData[j] + "</p></td>";
                    }
                    else
                    {
                        if (reportData[j].Equals("Fail", StringComparison.CurrentCultureIgnoreCase))
                        {
                            reportContent2 = reportContent2 + "<td class=border_left bgcolor=red><p class=normal_text>" +
                                             reportData[j] + "</p></td>";
                        }
                        else
                        {
                            reportContent2 = reportContent2 + "<td class=border_left><p class=normal_text>" +
                                             reportData[j] + "</p></td>";
                        }
                    }
                    j++;
                }
            }
            reportContent2 = reportContent2 + "</tr>";

            using (var sw = new StreamWriter(FileName, true))
            {
                sw.WriteLine(reportContent2);
            }
        }

        /// <summary>
        ///This method closes the HTML report writing and prints the overall status
        /// </summary>
        public void CloseReport()
        {
            int PassCOunt = GetCounts("Pass");
            int FailCount = GetCounts("Fail");

            if (File.Exists(FileName))
            {
                using (var sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("</body>");
                    sw.WriteLine("</html>");
                }
            }

            File.WriteAllText(FileName, Regex.Replace(File.ReadAllText(FileName), "PassCOunt", (PassCOunt - 2).ToString()));
            File.WriteAllText(FileName, Regex.Replace(File.ReadAllText(FileName), "FailCount", (FailCount - 2).ToString()));
        }

        /// <summary>
        /// This function calls the LogInit and the InitReport function
        /// </summary>
        public void InitHtmlReport()
        {
            LogInit();
            InitReport();
        }

        /// <summary>
        ///This method creates the basic structure of the HTML file (CSS and headers)
        /// </summary>
        private void InitReport()
        {
            if (File.Exists(FileName))
            {
                using (var sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("<head>");
                    sw.WriteLine("      <meta content=text/html; charset=ISO-8859-1 http-equiv=content-type>");
                    sw.WriteLine("      <title>Test Report</title>");
                    sw.WriteLine("      <style type=text/css>");
                    sw.WriteLine(
                        "       .title {  font-family: 'Lobster', Georgia, Times, serif; font-size: 40px;  font-weight: bold; color:#806D7E;}");
                    sw.WriteLine("          .bold_text {  font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px;  font-weight: bold;}");
                    sw.WriteLine("           ..normal_text {  font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px;  font-weight: normal;}");
                    sw.WriteLine("           .small_text {  font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 10px;  font-weight: normal; }");
                    sw.WriteLine("          .border { border: 1px solid #000000;}");
                    sw.WriteLine("        .border_left { border-top: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000;}");
                    sw.WriteLine("       .border_right { border-top: 1px solid #045AFD; border-right: 1px solid #000000;}");
                    sw.WriteLine("       .result_ok { font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px;  font-weight: bold; background-color:green;text-align: center; }");
                    sw.WriteLine("       .result_nok { font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px;  font-weight: bold;background-color:red; text-align: center; }");
                    sw.WriteLine("       .overall_ok { font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px; background-color:green; font-weight: bold; text-align: left; }");
                    sw.WriteLine("       .overall_nok { font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px; background-color:red; font-weight: bold; text-align: left; }");
                    sw.WriteLine("        .bborder_left { border-top: 1px solid #000000; border-left: 1px solid #000000; border-bottom: 1px solid #000000; background-color:#000000;font-family: Segoe UI; font-size: 12px;  font-weight: bold;text-align: center; color: white;}");
                    sw.WriteLine("       .bborder_right { border-right: 1px solid #045AFD; background-color:#000000;font-family: 'Adobe Caslon Pro', 'Hoefler Text', Georgia, Garamond, Times, serif; font-size: 12px;  font-weight: bold; text-align: center; color: white;}");

                    sw.WriteLine("           .bborder_leftt { border-top: 1px solid #000000; border-left: 1px solid #000000; border-bottom: 1px solid #000000; background-color:green;font-family: Segoe UI; font-size: 12px;  font-weight: bold;text-align: center; color: white;}");
                    sw.WriteLine("        .bborder_leftt1 { border-top: 1px solid #000000; border-left: 1px solid #000000; border-bottom: 1px solid #000000; background-color:red;font-family: Segoe UI; font-size: 12px;  font-weight: bold;text-align: center; color: white;}");

                    sw.WriteLine("      </style>");
                    sw.WriteLine("<script src=" + "http://www.kryogenix.org/code/browser/sorttable/sorttable.js" +
                                 " type=" + "text/javascript" + "></script>");
                    sw.WriteLine("<script type=" + "'text/javascript'" + ">");

                    sw.WriteLine("var rowVisible = true;");

                    sw.WriteLine("function toggleDisplay(tbl) {");
                    sw.WriteLine("   tbl.style.display=''" + ";");
                    sw.WriteLine("var tblRows = tbl.rows;");

                    sw.WriteLine("   for (i = 0; i < tblRows.length; i++) {");
                    sw.WriteLine("      if (tblRows[i].className != " + "'headerRow'" + ")" + " {");
                    sw.WriteLine(" tblRows[i].style.display = (rowVisible) ? 'none':'' ;");
                    sw.WriteLine("      }");
                    sw.WriteLine("   }");
                    sw.WriteLine("   rowVisible = !rowVisible;");
                    sw.WriteLine("}");
                    sw.WriteLine("</script>");
                    sw.WriteLine("      </head>");
                    sw.WriteLine("      <body>");
                    sw.WriteLine("      <br>");
                    sw.WriteLine("      <center>");
                    sw.WriteLine("      <table width=95% border=0 cellpadding=2 cellspacing=2>");
                    sw.WriteLine("      <tbody>");
                    sw.WriteLine("      <tr>");
                    sw.WriteLine("      <td>");
                    sw.WriteLine("      <table width=100% border=0 cellpadding=2 cellspacing=2>");
                    sw.WriteLine("      <tbody>");
                    sw.WriteLine("      <tr>");
                    sw.WriteLine(
                        "      <td align=center><p class=title>Test Report</p></td></tr><tr> <td align=left></img><td align=right><img src=" +

                        "></img></td></tr>");
                    sw.WriteLine("      </tbody>");
                    sw.WriteLine("      </table>");
                    sw.WriteLine("      <br><br>");

                    sw.WriteLine("       <table class=testRunDetails align='left'  width=30%> ");

                    sw.WriteLine("       </tr>");
                    sw.WriteLine("         <tr>");
                    sw.WriteLine("         <td class=bborder_left>Host Name</td>");
                    sw.WriteLine("        <td class=bborder_left>" + System.Net.Dns.GetHostName() + "</td>");
                    sw.WriteLine("       </tr>");
                    sw.WriteLine("         <tr>");
                    sw.WriteLine("         <td class=bborder_left>Execution Time</td>");
                    sw.WriteLine("        <td class=bborder_left>" + string.Format("{0:yyyyMMdd-hhmmss}", DateTime.Now) + "</td>");
                    sw.WriteLine("       </tr>");
                    sw.WriteLine("      </table> ");

                    sw.WriteLine("</table>");
                    sw.WriteLine("<br>");

                    sw.WriteLine("<table class=testRunDetails align='left'  width=50%> ");
                    sw.WriteLine("</tr>");
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<td class=bborder_left>Pass</td>");
                    sw.WriteLine("<td class=bborder_leftt>  PassCOunt </td>");
                    sw.WriteLine("</tr>");
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<td class=bborder_left>Fail</td>");
                    sw.WriteLine("<td class=bborder_leftt1> FailCount </td>");
                    sw.WriteLine("</tr>");
                    sw.WriteLine("</table>");

                    sw.WriteLine("      <br><br><br><br> ");
                }
            }
        }

        /// <summary>
        /// This method checks if the fileName already exists ,if not creates the physical file.
        /// </summary>
        private void LogInit()
        {
            if (!File.Exists(FileName))
            {
                using (File.Create(FileName))
                {
                }
            }
            else
            {
                File.Delete(FileName);
                using (File.Create(FileName))
                {
                }
            }
        }

        private int GetCounts(string textToCheck)
        {
            string text = File.ReadAllText(FileName);
            MatchCollection matches = Regex.Matches(text, textToCheck);

            return matches.Count;

            //System.IO.StreamReader file =    new System.IO.StreamReader(File.OpenRead(FileName));
            //while ((line = file.ReadLine()) != null)
            //{
            //    //System.Console.WriteLine(line);
            //    counter++;
            //}

            //file.Close();

            //return counter;
        }

        #endregion

        # region Private Members

        private string FileName { get; set; }

        #endregion
    }
}