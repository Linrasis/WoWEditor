﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace WoWEditor6
{
    public enum LogLevel
    {
        Fatal,
        Error,
        Warning,
        Debug
    }

    interface ILogSink
    {
        void AddMessage(LogLevel logLevel, string title, string message);
    }

    class ConsoleLogSink : ILogSink
    {
        private static void SetLevelColor(LogLevel level)
        {
            switch(level)
            {
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        public void AddMessage(LogLevel logLevel, string title, string message)
        {
            lock(this)
            {
                SetLevelColor(logLevel);
                Console.Write(title);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(message);
            }
        }
    }

    static class Log
    {
        private static readonly List<ILogSink> Sinks = new List<ILogSink>();
        private static LogLevel gLogLevel = LogLevel.Debug;

        static Log()
        {
            Sinks.Add(new ConsoleLogSink());
        }

        private static string GetTitle(string fileName, int line)
        {
            return string.Format("{0:HH:mm:ss} {1}:{2}", DateTime.Now, Path.GetFileName(fileName), line);
        }

        private static void AddMessage(LogLevel level, string title, string message)
        {
            if ((int) level > (int) gLogLevel)
                return;

            lock(Sinks)
            {
                foreach (var sink in Sinks)
                    sink.AddMessage(level, title, message);
            }
        }

        public static void AddSink(ILogSink sink)
        {
            lock (Sinks)
                Sinks.Add(sink);
        }

        public static void RemoveSink(ILogSink sink)
        {
            lock (Sinks)
                Sinks.Remove(sink);
        }

        public static void Debug(string message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Debug, GetTitle(fileName, line) + " - Debug: ", message);
        }

        public static void Warning(string message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Warning, GetTitle(fileName, line) + " - Warning: ", message);
        }

        public static void Error(string message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Error, GetTitle(fileName, line) + " - ERROR: ", message);
        }

        public static void Fatal(string message, [CallerFilePath] string fileName = "", [CallerLineNumber] int line = 0)
        {
            AddMessage(LogLevel.Fatal, GetTitle(fileName, line) + " - FATAL: ", message);
        }
    }
}
