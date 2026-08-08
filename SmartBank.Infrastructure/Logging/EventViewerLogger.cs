using System;
using System.Diagnostics;
using System.Text;

namespace SmartBank.Infrastructure.Logging
{
    public static class EventViewerLogger
    {

        private const string Source = "SmartBank";
        private const string LogName = "Application";
        private const string TruncatedMessage = "\n... Message was truncated";
        private const int MaxMessageLength = 30000;

        public static void LogError(Exception exception, string context, int eventId = 1001)
        {
            Write(exception, context, EventLogEntryType.Error, eventId);
        }

        public static void LogWarning(Exception exception, string context, int eventId = 2001)
        {
            Write(exception, context, EventLogEntryType.Warning, eventId);
        }

        private static void Write(Exception exception, string context, EventLogEntryType entryType, int eventId)
        {
            try
            {
                if (!EventLog.SourceExists(Source))
                {
                    EventLog.CreateEventSource(Source, LogName);
                }

                string message = BuildMessage(exception, context);

                if (message.Length > MaxMessageLength)
                {
                    message = message.Substring(0, MaxMessageLength - TruncatedMessage.Length) + TruncatedMessage;
                }

                EventLog.WriteEntry(Source, message, entryType, eventId);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Failed to write to Event Viewer:");
                Debug.WriteLine(ex.ToString());
            }
        }

        private static string BuildMessage(Exception exception, string context)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("SmartBank Error Log");
            builder.AppendLine("------------------------------");
            builder.AppendLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            builder.AppendLine($"Context: {context}");
            builder.AppendLine();

            builder.AppendLine("Exception Details:");
            builder.AppendLine(exception.ToString());

            return builder.ToString();
        }
    }
}
