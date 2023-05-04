using Avalonia.Media;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SimpleLogViewer;

    [XmlRoot(ElementName = "event", Namespace = "http://jakarta.apache.org/log4j/")]
    public class EventLog4J : ObservableObject
    {

        [XmlElement(ElementName = "message", Namespace = "http://jakarta.apache.org/log4j/")]
        public string? Message { get; set; }

        [XmlAttribute(AttributeName = "log4j", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Log4j { get; set; }

        [XmlAttribute(AttributeName = "logger", Namespace = "")]
        public string? Logger { get; set; }

        [XmlAttribute(AttributeName = "timestamp", Namespace = "")]
        public double Timestamp { get; set; }

        [XmlAttribute(AttributeName = "level", Namespace = "")]
        public string Level { get; set; }

        [XmlText]
        public string? Text { get; set; }
    }
    [DataContract]
    public class Event : ObservableObject
    {
       
        public Event(EventLog4J log4J)
        {
            if (log4J == null) throw new ArgumentNullException(nameof(log4J));
            Message = log4J.Message ?? log4J.Text ?? String.Empty;
            Level = log4J.Level ?? "TRACE";
            Text = log4J.Text ?? log4J.Message ?? String.Empty;
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(log4J.Timestamp.ToString())).UtcDateTime.ToLocalTime();
            Logger = log4J.Logger ?? String.Empty;
            if (this.Level.ToUpper() == "DEBUG")
            {
                BackgroundBrush = Brush.Parse("DarkGray");
                ForegroundBrush = Brush.Parse("Black");
            }
            if (this.Level.ToUpper() == "TRACE")
            {
                BackgroundBrush = Brush.Parse("LightGray");
                ForegroundBrush = Brush.Parse("Black");
            }
            if (this.Level.ToUpper() == "INFO")
            {
                BackgroundBrush = Brush.Parse("Green");
                ForegroundBrush = Brush.Parse("Black");

            }
            if (this.Level.ToUpper() == "ERROR")
            {
                BackgroundBrush = Brush.Parse("Red");
                ForegroundBrush = Brush.Parse("White");

            }
            if (this.Level.ToUpper() == "WARNING" || this.Level.ToUpper() == "WARN")
            {
                BackgroundBrush = Brush.Parse("Yellow");
                ForegroundBrush = Brush.Parse("Black");

            }
            IsNotSame = Text.ToUpper().Trim() != Message.ToUpper().Trim();
        }
        [DataMember]
        public IBrush BackgroundBrush { get; set; } = Brush.Parse("Black");
        [DataMember]
        public IBrush ForegroundBrush { get; set; } = Brush.Parse("White");
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public DateTime Timestamp { get; set; }
        [DataMember]
        public string Logger { get; set; }
        [DataMember]
        public string Level { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public bool IsNotSame { get; set; }
    }