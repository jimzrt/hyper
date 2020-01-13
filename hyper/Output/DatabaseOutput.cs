using hyper.Database;
using hyper.Database.DAO;
using hyper.Helper;
using hyper.Helper.Extension;
using hyper.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
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
            `key` TEXT,
            `Value` FLOAT,
            `raw` Text NOT NULL,
            `Added` DATETIME NOT NULL)",
connection);
                connection.Open();
                command.ExecuteNonQuery();

            }

            LinqToDB.Data.DataConnection.DefaultSettings = new DatabaseSettings(fileName);
            eventDAO = new EventDAO();


           // Common.logger.Info(Util.ObjToJson(eventDAO.GetAll()));
        }

        public void HandleCommand(object command, byte srcNodeId, byte destNodeId)
        {
            Event evt = new Event();
            evt.NodeId = srcNodeId;

            //var implicitCastMethod =
            //command.GetType().GetMethod("op_Implicit",
            //       new[] { command.GetType() });

            //if (implicitCastMethod == null)
            //{
            //    Common.logger.Warn("byteArray to {0} not possible!", command.GetType().Name);
            //    return;
            //}
            //var report = implicitCastMethod.Invoke(null, new[] { command });
            evt.EventType = command.GetType().Name;// EventType.UNHANDLED;
            var data = Util.ObjToJson(command, false);
            evt.Raw = data;

            var keyValue = command.GetKeyValue(out Enums.EventKey eventKey, out float floatVal);
            if (keyValue)
            {
                evt.EventTypeKeyR = eventKey;
                evt.Value = floatVal;
            }


            //switch (command)
            //{
            //    //case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT alarmReport:
            //    //    evt.EventType = alarmReport.GetType();// EventType.NOTIFICATION;
            //    //    break;
            //    //case COMMAND_CLASS_BASIC_V2.BASIC_SET basicSet:
            //    //    evt.EventType = basicSet.GetType();// EventType.BASIC_SET;
            //    //    evt.Value = basicSet.value;
            //    //    break;
            //    //case COMMAND_CLASS_SWITCH_BINARY_V2.SWITCH_BINARY_REPORT binaryReport:
            //    //    evt.EventType = binaryReport.GetType();// EventType.SWITCH_BINARY_REPORT;
            //    //    evt.Value = binaryReport.targetValue;
            //    //    break;
            //    //case COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NOTIFICATION wakeupReport:
            //    //    evt.EventType = wakeupReport.GetType();// EventType.WAKEUP;
            //    //    break;
            //    //case COMMAND_CLASS_BATTERY.BATTERY_REPORT batteryReport:
            //    //    evt.EventType = batteryReport.GetType();// EventType.BATTERY;
            //    //    evt.Value = batteryReport.batteryLevel;
            //    //    break;

            //    case COMMAND_CLASS_SENSOR_MULTILEVEL_V11.SENSOR_MULTILEVEL_REPORT multilevelReport:
            //        { 
            //        multilevelReport.GetKeyValue(out Enums.EventKey eventKey, out float floatVal);
            //        evt.EventTypeKeyR = eventKey;
            //        evt.Value = floatVal;
            //        goto default;
            //        }
            //    case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT notificationReport:
            //        {

                    
            //        notificationReport.GetKeyValue(out Enums.EventKey eventKey, out float floatVal);
            //            evt.EventTypeKeyR = eventKey;
            //            evt.Value = floatVal;
            //        goto default;
            //        }
            //    //    evt.EventType = multilevelReport.GetType();// EventType.SENSOR_MULTILEVEL_REPORT;
            //    //    evt.Value = multilevelReport.sensorValue[0];
            //    //    break;
            //    //case COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT sensorBinaryReport:
            //    //    evt.EventType = sensorBinaryReport.GetType();// EventType.SENSOR_BINARY;
            //    //    evt.Value = sensorBinaryReport.sensorValue;
            //    //    break;
            //    default:
            //        evt.EventType = command.GetType().Name;// EventType.UNHANDLED;
            //        var data = Util.ObjToJson(command, false);
            //        evt.Raw = data;
            //        break;
            //}

            eventDAO.InsertEventAsync(evt);

        }
    }
}
