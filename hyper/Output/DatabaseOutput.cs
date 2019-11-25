using hyper.Database;
using hyper.Database.DAO;
using hyper.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Text;
using ZWave.CommandClasses;

namespace hyper.Output
{
    class DatabaseOutput : IOutput
    {

        private EventDAO eventDAO;

        public DatabaseOutput(string fileName)
        {
            if (!File.Exists(fileName))
            {
                //  using (File.Create("events.db")) ;

                using SQLiteConnection connection = new SQLiteConnection("Data Source="+ fileName + ";");
                using SQLiteCommand command = new SQLiteCommand(
@"CREATE TABLE 'Events' ( 
            `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
            `NodeId` INTEGER NOT NULL,
            `EventType` TEXT NOT NULL,
            `Value` INTEGER,
            `Added` DATETIME NOT NULL)",
connection);
                connection.Open();
                command.ExecuteNonQuery();

            }

            LinqToDB.Data.DataConnection.DefaultSettings = new DatabaseSettings(fileName);
            eventDAO = new EventDAO();
        }

        public void HandleCommand(object command, byte srcNodeId, byte destNodeId)
        {
            Event evt = new Event();
            evt.NodeId = srcNodeId;

            switch (command)
            {
                case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT alarmReport:
                    evt.EventType = EventType.NOTIFICATION;
                    break;
                case COMMAND_CLASS_BASIC_V2.BASIC_SET basicSet:
                    evt.EventType = EventType.BASIC_SET;
                    evt.Value = basicSet.value;
                    break;
                case COMMAND_CLASS_SWITCH_BINARY_V2.SWITCH_BINARY_REPORT binaryReport:
                    evt.EventType = EventType.SWITCH_BINARY_REPORT;
                    evt.Value = binaryReport.targetValue;
                    break;
                case COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NOTIFICATION _:
                    evt.EventType = EventType.WAKEUP;
                    break;
                case COMMAND_CLASS_BATTERY.BATTERY_REPORT batteryReport:
                    evt.EventType = EventType.BATTERY;
                    evt.Value = batteryReport.batteryLevel;
                    break;
                case COMMAND_CLASS_SENSOR_MULTILEVEL_V11.SENSOR_MULTILEVEL_REPORT multilevelReport:
                    evt.EventType = EventType.SENSOR_MULTILEVEL_REPORT;
                    evt.Value = multilevelReport.sensorValue[0];
                    break;
                case COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT sensorBinaryReport:
                    evt.EventType = EventType.SENSOR_BINARY;
                    evt.Value = sensorBinaryReport.sensorValue;
                    break;
                default:
                    evt.EventType = EventType.UNHANDLED;
                    break;
            }

            eventDAO.InsertEventAsync(evt);

        }
    }
}
