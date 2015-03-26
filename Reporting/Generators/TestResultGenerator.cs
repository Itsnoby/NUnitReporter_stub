using System.Collections.Generic;
using System.Text;
using NUnitReporter.Reporting.Helpers;

namespace NUnitReporter.Reporting.Generators    
{
    public class TestResultGenerator
    {
        private StringBuilder _report = new StringBuilder();
        private string _status = "Passed";
        private string _statusClass;
        public int Duration { get; set; }
        public List<HtmlReporterHelperMessage> Log { get; set; }
        public string Name { get; set; }

        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                switch (_status)
                {
                    case "Passed":
                        _statusClass = "passed_test";
                        break;
                    case "Skipped":
                        _statusClass = "skipped_test";
                        break;
                    case "Failed":
                        _statusClass = "failed_test";
                        break;
                }
            }
        }

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
			
			    img {
				    height: auto;
				    width: 100%;
			    }

			    .image_block {
				    max-width: 720px;
			    }

                #test_name {
                    font-family: Arial,Helvetica,sans-serif;
                    font-size: 1.5em;
                    font-weight: bold;
                    margin-bottom: 0.7em;
                }
		
                .info {
                    font-size: 0.8em;
                    padding-left: 4em;
                }

                #test_status {
                    padding: 1px 10px;
                    font-weight: bold;
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
                }
		
                #test_log th {
                    background-color: #CCC;
                    border-color: #000000;
                    border-style: solid;
                    border-width: 1px 0 0;
                    font-size: 1em;
                    text-align: left;
                    padding-left: 0.3em;
                }
		
                .time_col {
                    width: 6em;
                }   
		
                .log_step {
                    font-size: 0.8em;
                    line-height: 1.2em;
                }
		
                .log_step:hover {
                    background-color: #E0E0E0; 
                }
		
                .action_step {
                    font-weight: bold;
                }

			    .notify_step {
				    background-color: #F1F1F1;
                    font-style: italic;
                }
		
                .action_step td {
                    padding-top: 0.5em;
                }

			    .skipped_step {
                    color: #FFA500;
                    font-weight: bold;
                }
		
                .failed_step {
                    color: #FF0000;
                    font-weight: bold;
                }

			    .stacktrace_step {
                    background-color: #FFF0F0;
				    color: #CD0000;
                }

            </style>
        </head>";

        #endregion

        public void CreateReport()
        {
            _report.AppendLine("<!DOCTYPE html>");

            #region <html>

            _report.AppendLine("<html>");
            _report.AppendLine(_style);

            #region <body>

            _report.AppendLine("<body>");

            #region <div id="header">

            _report.AppendLine("<div id=\"header\">");
            _report.AppendLine(string.Format("<h1 id=\"test_name\">{0}</h1>", Name));
            _report.AppendLine(string.Format("<p class=\"info\">Test duration: <span>{0}</span>s</p>", Duration));
            _report.AppendLine(
                string.Format("<p class=\"info\">Status: <span id=\"test_status\" class=\"{0}\">{1}</span></p>",
                    _statusClass,_status));
            _report.AppendLine("</div>");

            #endregion

            #region <div id="body">

            _report.AppendLine("<div id=\"body\">");

            #region <table id=\"test_log\">

            _report.AppendLine("<table id=\"test_log\">");

            #region <thead>

            _report.AppendLine("<thead>");
            _report.AppendLine("<tr><th colspan=\"2\">Detailed steps</th></tr>");
            _report.AppendLine("<tr><td class=\"time_col\">Time</td><td>Description</td></tr>");
            _report.AppendLine("</thead>");

            #endregion

            #region <tbody>

            _report.AppendLine("<tbody>");
            foreach (var message in Log)
            {
                var messageClass = "log_step";
                switch (message.Type)
                {
                    case "ActionTitle":
                        messageClass = string.Format("{0} action_step", messageClass);
                        break;
                    case "Notify":
                        messageClass = string.Format("{0} notify_step", messageClass);
                        break;
                    case "Skipped":
                        messageClass = string.Format("{0} skipped_step", messageClass);
                        break;
                    case "Failed":
                        messageClass = string.Format("{0} failed_step", messageClass);
                        break;
                    case "Image":
                        messageClass = string.Format("{0} failed_step", messageClass);
                        break;
                    case "StackTrace":
                        messageClass = string.Format("{0} stacktrace_step", messageClass);
                        break;
                }
                _report.AppendLine(string.Format("<tr class=\"{0}\">", messageClass));
                _report.AppendLine(string.Format("<td valign=\"top\" class=\"time_col\">{0}</td>", message.GetTime()));
                if (message.Type == "Image")
                {
                    _report.AppendLine("<td><div class=\"image_block\">");
                    _report.AppendLine(
                        string.Format("<a href=\"{0}\" title=\"Click to open in full size\"><img src=\"{0}\"></a>",
                            message.Message));
                    _report.AppendLine("</div></td>");
                }
                else
                    _report.AppendLine(string.Format("<td>{0}</td>", message.Message));
                _report.AppendLine("</tr>");
            }
            _report.AppendLine("</tbody>");

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
