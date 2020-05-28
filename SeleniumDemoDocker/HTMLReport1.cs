using System;
using System.IO;
using System.Text.RegularExpressions;

namespace SeleniumDemoDocker
{
    internal class HTMLReport1
    {
        public static string FileName;

        public HTMLReport1(string fileName1)
        {
            string datetimeString = string.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            FileName = (fileName1 + "-" + datetimeString + ".html");
        }

        public void AddHeader(string[] headerData)
        {
            string reportContent2 = "";

            int colLength = headerData.Length;

            if (colLength > 0)
            {
                reportContent2 = reportContent2 + " <div class='container-fluid' id='report'>";
                reportContent2 = reportContent2 + "<div class='row'>";
                reportContent2 = reportContent2 + "<div class='col-md-10 col-md-offset-1'>";
                reportContent2 = reportContent2 + "<table class='stats-table sortable'>";
                reportContent2 = reportContent2 + "<thead><tr>";

                int i = 0;

                while (i < headerData.Length)
                {
                    if (i == 1 || i == 2)
                    {
                        reportContent2 = reportContent2 + "<th class='tagname'>" + headerData[i] + "</th>";
                    }
                    else
                    {
                        reportContent2 = reportContent2 + "<th class='tagname'>" + headerData[i] + "</th>";
                    }
                    i++;
                }
                reportContent2 = reportContent2 + "</tr></thead>";
            }

            using (var sw = new StreamWriter(FileName, true))
            {
                sw.WriteLine(reportContent2);
            }
        }

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
                        reportContent2 = reportContent2 + "<td class ='passed'>" + reportData[j] + "</td>";
                    }
                    else
                    {
                        if (reportData[j].Equals("Fail", StringComparison.CurrentCultureIgnoreCase))
                        {
                            reportContent2 = reportContent2 + "<td class ='failed'>" + reportData[j] + "</td>";
                        }
                        else
                        {
                            reportContent2 = reportContent2 + "<td class ='tagname'>" + reportData[j] + "</td>";
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

        public void InitHtmlReport()
        {
            LogInit();
            InitReport();
            GetResultCount();
        }

        private void InitReport()
        {
            if (File.Exists(FileName))
            {
                using (var sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("<html>");

                    sw.WriteLine("      <meta content=text/html; charset=ISO-8859-1 http-equiv=content-type>");
                    sw.WriteLine("      <title>Test Report</title>");
                    sw.WriteLine("<head>");
                    sw.WriteLine("<link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' integrity='sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u' crossorigin='anonymous'>");
                    sw.WriteLine("      <style>");
                    sw.WriteLine("body{padding-top:60px}h2{font-size:24px}a{color:#0097da}a:hover{color:#00587f}.header-tag-name{color:gray;font-style:italic}.keyword{font-weight:700}.indention{padding-left:3px}.inner-level{margin-top:5px;margin-left:20px;padding-bottom:2px;padding-left:1px}.element{margin-bottom:15px;padding-left:3px}.element,.steps,.hooks-after,.hooks-before{box-shadow:-1px 0 lightgray;transition:box-shadow 0.3s}.element:hover,.steps:hover,.hooks-after:hover,.hooks-before:hover{box-shadow:-3px 0 #6ce}.description{font-style:italic;background-color:beige;white-space:pre}.message,.output,.embedding{background-color:#dfdfdf;overflow:auto}.embedding-content{padding:10px;margin-left:10px;margin-right:10px;margin-bottom:10px;font-size:13px;overflow-x:auto;line-height:1.42857143;color:#333;word-break:break-all;word-wrap:break-word;background-color:#f5f5f5;border:1px solid #ccc;border-radius:4px}.html-content{position:relative;padding:0 0 56.25%;height:0;overflow:hidden}.html-content iframe{position:absolute;top:0;left:0;width:100%;height:100%;border:none}.download-button{float:right;margin-right:10px;color:#333}.passed{background-color:#92DD96}.failed{background-color:#F2928C}.skipped{background-color:#8AF}.pending{background-color:#F5F28F}.undefined{background-color:#F5B975}.lead-duration{float:right;padding-right:15px}table.stats-table{background-color:white;color:black;margin-bottom:20px;width:100%}table.stats-table th,table.stats-table td{border:1px solid gray;padding:5px;text-align:center}table.stats-table tr.header{background-color:#6CE}table.stats-table tfoot{font-weight:700}tfoot.total,td.total,th.total{background-color:lightgray}table.stats-table td.duration{text-align:right;white-space:nowrap}table.stats-table td.tagname{text-align:left}table.stats-table td.location,.location{font-family:monospace;text-align:left}table.step-arguments{margin-bottom:5px;margin-left:25px;margin-top:3px}table.step-arguments th,table.step-arguments td{border:1px solid gray;padding:3px;text-align:left}table#tablesorter thead tr:not(.dont-sort) th{cursor:pointer}tr:hover{transition:background-color 0.3s}.collapsable-control{cursor:pointer}.chevron:after{content:'f078'}.collapsed .chevron:after{content:'f054'}.footer{font-size:smaller;text-align:center;margin-top:30px}.carousel-indicators{bottom:0}.carousel-indicators li{border:1px solid black}.carousel-indicators .active{background-color:black}.carousel-control{font-size:40px;padding-top:150px;}.carousel-control.right,.carousel-control.left{background-image:none;color:#eee}pre{margin:10px}");

                    sw.WriteLine("      </style>");
                    sw.WriteLine("<script src=" + "http://www.kryogenix.org/code/browser/sorttable/sorttable.js" +
                                 " type=" + "text/javascript" + "></script>");
                    sw.WriteLine("<script type='text/javascript' src='https://www.gstatic.com/charts/loader.js'></script>");
                    sw.WriteLine("<script type='text/javascript'>");

                    sw.WriteLine("google.charts.load('current', {'packages':['corechart']});");
                    sw.WriteLine("google.charts.setOnLoadCallback(drawChart);");

                    sw.WriteLine("function drawChart() {");
                    sw.WriteLine("var data = google.visualization.arrayToDataTable([");
                    sw.WriteLine("['Task', 'Hours per Day'],");
                    sw.WriteLine("['Pass', PassCOunt],");
                    sw.WriteLine("['Fail',FailCount]");
                    sw.WriteLine("]);");

                    sw.WriteLine("var options = {  width: 400,  height: 240,    colors: ['#92DD96', '#F2928C']};");

                    sw.WriteLine("var chart = new google.visualization.PieChart(document.getElementById('piechart'));");
                    sw.WriteLine("chart.draw(data, options);");

                    sw.WriteLine("}");
                    sw.WriteLine("</script>");

                    sw.WriteLine("      </head>");
                    sw.WriteLine("      <body>");
                    sw.WriteLine("      <br>");

                    sw.WriteLine("<div class='container-fluid'>");
                    sw.WriteLine("<div class='col-md-5 col-md-offset-1'>");

                    sw.WriteLine("<table class='table table-bordered'  id='build-info'>");
                    sw.WriteLine("<thead><tr>");
                    sw.WriteLine(" <th>Date</th><th>HostName</th></tr></thead><tbody><tr class='info'>");
                    sw.WriteLine("<td>" + string.Format("{0:yyyyMMdd-hhmmss}", DateTime.Now) + "</td>");
                    sw.WriteLine(" <td>" + System.Net.Dns.GetHostName() + "</td></tr></tbody></table>");

                    sw.WriteLine(" <div id='report-lead' class='container-fluid'><div class='col-md-10 col-md-offset-1'></div></div>");
                }
            }
        }

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

            File.WriteAllText(FileName, Regex.Replace(File.ReadAllText(FileName), "PassCOunt", (PassCOunt - 4).ToString()));
            File.WriteAllText(FileName, Regex.Replace(File.ReadAllText(FileName), "FailCount", (FailCount - 4).ToString()));
        }

        private int GetCounts(string textToCheck)
        {
            try
            {
                string text = File.ReadAllText(FileName);
                MatchCollection matches = Regex.Matches(text, textToCheck);

                return matches.Count;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private void GetResultCount()
        {
            if (File.Exists(FileName))
            {
                using (var sw = new StreamWriter(FileName, true))
                {
                    sw.WriteLine("<table class='table table-bordered'  id='build-info' ");
                    sw.WriteLine("<thead>");
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<th class='bborder_left tagname'>Pass</td>");
                    sw.WriteLine("<td class='bborder_leftt passed'>  PassCOunt </td>");
                    sw.WriteLine("</tr>");
                    sw.WriteLine("<tr>");
                    sw.WriteLine("<th class='bborder_left tagname'>Fail</td>");
                    sw.WriteLine("<td class='bborder_leftt1 failed'> FailCount </td>");
                    sw.WriteLine("</tr>");
                    sw.WriteLine("</table>");
                    sw.WriteLine("<div id='piechart'></div>");
                }
            }
        }

        public void AddInfoTable(string[,] data)
        {
            string reportContent2 = "";
            int colLength = data.GetLength(1);

            reportContent2 = reportContent2 + " <div class='container-fluid' id='report'>";
            reportContent2 = reportContent2 + "<div class='row'>";
            reportContent2 = reportContent2 + "<div class='col-md-10 col-md-offset-1'>";
            reportContent2 = reportContent2 + "<table class='stats-table'>";
            reportContent2 = reportContent2 + "<thead><tr>";

            for (int i = 0; i < colLength; i++)
            {
                reportContent2 = reportContent2 + "<th class='tagname'>" + data[0, i] + "</th>";
            }
            reportContent2 = reportContent2 + "</tr></thead><tr>";

            for (int j = 0; j < colLength; j++)
            {
                reportContent2 = reportContent2 + "<td class='tagname'>" + data[1, j] + "</td>";
            }
            reportContent2 = reportContent2 + "</tr>";

            using (var sw = new StreamWriter(FileName, true))
            {
                sw.WriteLine(reportContent2);
            }
        }
    }
}