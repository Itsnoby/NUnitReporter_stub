using System.Text;
using NUnitReporter.Reporting.Helpers;

namespace NUnitReporter.Reporting.Generators
{
    public class SuiteResultGenerator
    {
        private StringBuilder _report = new StringBuilder();

        public SuiteResults SuiteResults { get; set; }

        public StringBuilder Report
        {
            get { return _report; }
            set { _report = value; }
        }

        #region head

        private string _style = @"
    <head>
        <style>
            body {
                font-family: Lucida Sans Unicode,Lucida Grande,sans-serif;
                line-height: 1.2em;
                margin: 2em;
            }
			
            #suite_name {
                font-family: Arial,Helvetica,sans-serif;
                font-size: 1.5em;
                font-weight: bold;
                margin-bottom: 0.7em;
            }

            .test_status {
                padding: 1px 10px;
                font-weight: bold;
				text-align: center;
				width: 6em;
            }
		
            .passed_test {
                background-color: #44AA44;
            }
		
            .skipped_test {
                background-color: #FFAA00;
            }
		
            .failed_test {
                background-color: #FF4444;
            }
		
            #test_log {
                width: 100%;
            }		
		
            #test_log thead tr td {
                color: #AAA;
				text-align: center;
            }
		
			#test_log th, #test_log tfoot tr td {
                border-color: #000;
                border-style: solid;
                border-width: 1px 0 0;
            }

            #test_log th {
                background-color: #CCC;
                border-color: #000;
                border-style: solid;
                border-width: 1px 0 0;
                font-size: 1em;
                text-align: left;
                padding-left: 0.3em;
            }
					
            .time_col {
                width: 5em;
            }   
		
            #test_log tbody, #test_log tfoot {
                font-size: 0.8em;
                line-height: 1.2em;
            }
			
			#test_log tfoot tr{
				background-color: #EEE;
			}
					
            .test_result:hover {
                background-color: #E0E0E0; 
            }
			
			.test_result a:hover, .test_result a:active {
				text-decoration: underline;
			}
			
			.test_result a {
			    color: #000;
				text-decoration: none;
			}
			
			.test_result a:visited {
			    color: #B4045F;
			}
        </style>
    </head>";

        #endregion

        public virtual void CreateReport()
        {
            _report.AppendLine("<!DOCTYPE html>");

            #region <html>

            _report.AppendLine("<html>");
            _report.AppendLine(_style);

            #region <body>

            _report.AppendLine("<body>");

            #region <div id="header">

            _report.AppendLine("<div id=\"header\">");
            _report.AppendLine(string.Format("<h1 id=\"test_name\">{0}</h1>", SuiteResults.Name));
            _report.AppendLine("</div>");

            #endregion

            #region <div id="body">

            _report.AppendLine("<div id=\"body\">");

            #region <table id=\"test_log\">

            _report.AppendLine("<table id=\"test_log\">");

            #region <thead>

            _report.AppendLine("<thead>");
            _report.AppendLine("<tr><th colspan=\"3\">Execution results</th></tr>");
            _report.AppendLine("<tr><td>Test case name</td><td>Status</td><td class=\"time_col\">Duration</td></tr>");
            _report.AppendLine("</thead>");

            #endregion

            #region <tbody>

            var totalDuration = 0;
            _report.AppendLine("<tbody>");
            foreach (var result in SuiteResults.Results)
            {
                var testStatusClass = "";
                switch (result.Status)
                {
                    case "Passed":
                        testStatusClass = "passed_test";
                        break;
                    case "Skipped":
                        testStatusClass = "skipped_test";
                        break;
                    case "Failed":
                        testStatusClass = "failed_test";
                        break;
                }
                _report.AppendLine("<tr class=\"test_result\">");
                _report.AppendLine(string.Format("<td><a href=\"{0}\">{1}</a></td>", result.FileLink, result.Name));
                _report.AppendLine(string.Format("<td class=\"test_status {0}\">{1}</td>", testStatusClass,
                    result.Status));
                _report.AppendLine(string.Format("<td valign=\"top\" class=\"time_col\"><span>{0}</span>s</td>",
                    result.Duration));
                _report.AppendLine("</tr>");
                totalDuration = totalDuration + result.Duration;
            }
            _report.AppendLine("</tbody>");

            #endregion

            #region <tfoot>

            _report.AppendLine("<tfoot>");
            _report.AppendLine("<tr>");
            _report.AppendLine("<td colspan=\"2\" style=\"text-align:right;\">Total duration:</td>");
            _report.AppendLine(string.Format("<td class=\"time_col\"><span>{0}</span>s</td>", totalDuration));
            _report.AppendLine("</tr>");
            _report.AppendLine("</tfoot>");

            #endregion

            _report.AppendLine("</table>");

            #endregion

            _report.AppendLine("</div>");

            #endregion

            _report.AppendLine("</body>");

            #endregion

            _report.AppendLine("</html>");

            #endregion
        }
    }
}
