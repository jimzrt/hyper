using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZWave.CommandClasses;

namespace hyper.Helper.Extension
{
    public static class Extensions
    {
        public static bool GetKeyValue(this object obj, out Enums.EventKey eventType, out float floatVal)
        {
            //if obj is COMMAND_CLASS_SENSOR_MULTILEVEL_V11.SENSOR_MULTILEVEL_REPORT 
            switch (obj)
            {
                case COMMAND_CLASS_SENSOR_MULTILEVEL_V11.SENSOR_MULTILEVEL_REPORT multiReport:
                    {
                        var size = multiReport.properties1.size;
                        var precision = multiReport.properties1.precision;
                        var type = multiReport.sensorType;

                        eventType = type switch
                        {
                            0x01 => Enums.EventKey.TEMPERATURE,
                            0x04 => Enums.EventKey.POWER,
                            0x05 => Enums.EventKey.HUMIDITY,
                            _ => Enums.EventKey.UNKNOWN,
                        };

                        if (size == 1)
                        {
                            var byteVal = multiReport.sensorValue[0];
                            floatVal = (float)byteVal;

                        }
                        else if (size == 2)
                        {
                            var shortVal = BitConverter.ToInt16(multiReport.sensorValue.Reverse().ToArray(), 0);
                            floatVal = (float)shortVal;
                        }
                        else if (size == 4)
                        {
                            var intVal = BitConverter.ToInt32(multiReport.sensorValue.Reverse().ToArray(), 0);
                            floatVal = (float)intVal;
                        }
                        else
                        {
                            Console.WriteLine("unknown size: {0}", size);
                            goto default;
                            //floatVal = -1;
                            //return false;
                        }
                        floatVal /= MathF.Pow(10f, precision);
                        return true;
                    }
                case COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT notificationReport:
                    {
                        var type = notificationReport.notificationType;
                        var mevent = notificationReport.mevent;
                        // 0x07 Home Security
                        if (type == 0x07)
                        {
                            eventType = mevent switch
                            {
                                0x00 => Enums.EventKey.MOTION,
                                0x03 => Enums.EventKey.TAMPER,
                                0x04 => Enums.EventKey.TAMPER,
                                0x09 => Enums.EventKey.TAMPER,
                                0x07 => Enums.EventKey.MOTION,
                                0x08 => Enums.EventKey.MOTION,
                                0x0a => Enums.EventKey.IMPACT,
                                _ => Enums.EventKey.UNKNOWN,
                            };
                            floatVal = mevent == 0 ? 0.0f : 1.0f;

                            return true;
                        }
                        // 0x06 Access Control
                        else if (type == 0x06)
                        {
                            eventType = mevent switch
                            {
                                0x16 => Enums.EventKey.STATE_CLOSED,
                                0x17 => Enums.EventKey.STATE_CLOSED,
                                _ => Enums.EventKey.UNKNOWN,
                            };

                            floatVal = mevent == 0x17 ? 1.0f : 0.0f;
                            return true;

                        }
                        else
                        {
                            goto default;
                            //eventType = Enums.EventKey.UNKNOWN;
                            //floatVal = -1;
                            //return false;
                        }
                    }
                case COMMAND_CLASS_SENSOR_BINARY_V2.SENSOR_BINARY_REPORT binaryReport:
                    {
                        eventType = Enums.EventKey.BINARY;
                        floatVal = binaryReport.sensorValue == 255 ? 1.0f : 0.0f;

                        return true;
                    }
                case COMMAND_CLASS_WAKE_UP_V2.WAKE_UP_NOTIFICATION wakeUpNotification:
                    {
                        eventType = Enums.EventKey.WAKEUP;
                        floatVal = 1.0f;
                        return true;
                    }
                case COMMAND_CLASS_BATTERY.BATTERY_REPORT batteryReport:
                    {
                        eventType = Enums.EventKey.BATTERY;
                        floatVal = batteryReport.batteryLevel;
                        return true;
                    }
                case COMMAND_CLASS_SWITCH_BINARY_V2.SWITCH_BINARY_REPORT binaryReport:
                    {
                        eventType = Enums.EventKey.STATE_ON;
                        floatVal = binaryReport.currentValue == 255 ? 1.0f : 0.0f;
                        return true;
                    }
                default:
                    floatVal = -1;
                    eventType = Enums.EventKey.UNKNOWN;
                    return false;


            }


        }

        //public static bool GetKeyValue(this COMMAND_CLASS_NOTIFICATION_V8.NOTIFICATION_REPORT notificationReport, out Enums.EventKey eventType, out float floatVal)
        //{
        //    var type = notificationReport.notificationType;
        //    var mevent = notificationReport.mevent;
        //    if(type == 0x07)
        //    {
        //        eventType = mevent switch
        //        {
        //            0x00 => Enums.EventKey.IDLE,
        //            0x03 => Enums.EventKey.TAMPER,
        //            0x04 => Enums.EventKey.TAMPER,
        //            0x09 => Enums.EventKey.TAMPER,
        //            0x07 => Enums.EventKey.MOTION,
        //            0x08 => Enums.EventKey.MOTION,
        //            0x0a => Enums.EventKey.IMPACT,
        //            _ => Enums.EventKey.UNKNOWN,
        //        };
        //        floatVal = 1;
        //        return true;
        //    } else
        //    {
        //        eventType = Enums.EventKey.UNKNOWN;
        //        floatVal = -1;
        //        return false;
        //    }

        //}
    }
}
