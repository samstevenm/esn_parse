using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESN_Utilities
{
    public class Dumper0To10 : ESNDumper
    {
        /// <summary>
        /// Device specific unpack for ESN 0-10 and switching
        /// </summary>
        /// <returns></returns>
        override public string Unpack()
        {
            int     idx;

            unpack = "";
            unpack += "<!--" + esnbu.DumpPosition();
            unpack += Unpack_DEVICE_INFO_STRUCT();
            sunpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += Unpack_DEVICE_DATA_STRUCT();
            unpack += "\n";
            pack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nBeginning of user data: " + esnbu.GetByte().ToString();
            unpack += "\n         Is Programmed: " + esnbu.GetByte().ToString();
            unpack += "\n";
            unpack += esnbu.DumpPosition() + "-->";
            unpack += "\nZONE DATA";
            for (idx = 0;idx < 4;idx++)
            {
                unpack += "\n";
                unpack += "\n   Zone " + (idx + 1).ToString();
                unpack += "\n          Power On Level: " + esnbu.GetByte().ToString("X2");
                unpack += "\n             Relay State: " + esnbu.GetByte();
                if (!International)
                {
                    unpack += "\n     CCI On Preset Level: ";
                    unpack += esnbu.GetByte().ToString("X2");
                    unpack += ":" + esnbu.GetByte().ToString("X2");
                    unpack += ":" + esnbu.GetByte().ToString("X2");
                    unpack += ":" + esnbu.GetByte().ToString("X2");
                    unpack += "\n       Timeclock CCI IDs: " + esnbu.GetComponentAddress();
                    unpack += "\n    CCI Timeclock On Lvl: " + esnbu.GetByte();
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nLOADSHED LEVELS";
            unpack += "\n   ";
            unpack += esnbu.GetByte().ToString() + ":";
            unpack += esnbu.GetByte().ToString() + ":";
            unpack += esnbu.GetByte().ToString() + ":";
            unpack += esnbu.GetByte().ToString();
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nAREA SCENES";
            unpack += "\n   ";
            unpack += esnbu.GetByte().ToString() + ":";
            unpack += esnbu.GetByte().ToString() + ":";
            unpack += esnbu.GetByte().ToString() + ":";
            unpack += esnbu.GetByte().ToString();
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nSENSOR NAMES";
            for (idx = 0; idx < 4; idx++)
            {
                unpack += "\n      IR SENSOR " + (idx + 1).ToString() + ":" + esnbu.GetUniString(16);
            }
            for (idx = 0; idx < 4; idx++)
            {
                unpack += "\n     OCC SENSOR " + (idx + 1).ToString() + ":" + esnbu.GetUniString(16);
            }
            for (idx = 0; idx < 4; idx++)
            {
                unpack += "\n   PHOTO SENSOR " + (idx + 1).ToString() + ":" + esnbu.GetUniString(16);
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nSWITCHLEGS";
            for (idx = 0; idx < 4; idx++)
            {
                string unpackedff = esnbu.UnpackSwitchlegFlatFile();
                byte bFlags = esnbu.GetByte();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nSwitchleg " + (idx + 1).ToString();
                    unpack += "\n  Flag: " + bFlags.ToString("X2");
                    unpack += unpackedff;
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nIR SENSOR TYPES";
            unpack += "\n  SENSOR VA Z1 Z2 Z3 Z4 S1 S2 S3 S4";
            unpack += "\n  -------------";
            for (idx = 0; idx < 4; idx++)
            {
                byte byt = esnbu.GetByte();
                unpack += "\n       " + (idx +1).ToString();
                unpack += " " + byt.ToString("X2");
                for (byte mask = 0x80;mask != 0;mask >>= 1)
                {
                    if ((byt & mask) != 0)
                    {
                        unpack += "  1";
                    }
                    else
                    {
                        unpack += "   ";
                    }
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nAREA COMPONENTS";
            for (idx = 0; idx < 4; idx++)
            {
                string unpackedff = esnbu.UnpackArea();
                byte bFlags = esnbu.GetByte();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nArea " + (idx + 1).ToString();
                    unpack += "\n  Flag: " + bFlags.ToString("X2");
                    unpack += unpackedff;
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nZONE COMPONENTS";
            for (idx = 0; idx < 4; idx++)
            {
                string unpackedff = esnbu.UnpackZone();
                byte bFlags = esnbu.GetByte();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nZone " + (idx + 1).ToString();
                    unpack += "\n  Flag: " + bFlags.ToString("X2");
                    unpack += unpackedff;
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nHHD OCC SENSOR COMPONENTS";
            for (idx = 0; idx < 16; idx++)
            {
                string unpackedff = esnbu.UnpackHHDOccSensor();
                byte bFlags = esnbu.GetByte();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nHHD Occ Sensor " + (idx + 1).ToString();
                    unpack += "\n  Flag: " + bFlags.ToString("X2");
                    unpack += unpackedff;
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\nHHD CCI COMPONENT";
            unpack += esnbu.UnpackHHDCCIComponent();
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";





            /*
                    typedef  struct
                    {
   
                       // DEVEIC STATIC & DYNAMIC DATA
                       UINT8                                  beginningOfUserData;
                       BOOLEAN                                isProgrammed;
                       ZONE_DATA_STRUCT                       zoneData[MAX_NUMBER_SWITCHLEGS];                        
                       ZONE_LOADSHED_STRUCT                   zoneLoadShed[MAX_NUMBER_SWITCHLEGS];   
                       UINT8                                  AreaScene[MAX_NUMBER_AREAS]; 
                       COMPONENT_NAME                         IrComponentName[MAX_PHYSICAL_IR_SENSORS];
                       COMPONENT_NAME                         OccComponentName[MAX_PHYSICAL_OCC_SENSORS];   
                       COMPONENT_NAME                         DaylightComponentName[MAX_PHYSICAL_DAYLIGHT_SENSORS];   
                       SWITCH_LEG_COMPONENT_STRUCT            SwitchlegComp[MAX_NUMBER_SWITCHLEGS];
                       IRZONEBITMAP_UNION                     IrProgramming[MAX_PHYSICAL_IR_SENSORS];
                       AREA_OBJECT_FLAT_FILE_STRUCT           AreaObject[MAX_NUMBER_AREAS];   
                       ZONE_OBJECT_FLAT_FILE_STRUCT           ZoneObject[MAX_NUMBER_ZONES];   
                       OCC_SENSOR_COMPONENT_STRUCT            OccSensorComponent[MAX_OCC_SENSORS_ALLOWED];
                       TIMECLOCK_CCI_COMPONENT_STRUCT         TimeclockCciComponent;
//$$$ DONE TO HERE   
                       #ifdef ESN_DOMESTIC
                          DRY_CONTACT_CCI_COMPONENT_STRUCT       SwitchCciComponent[MAX_PHYSICAL_TOGGLE_CCI_SW];
                       #endif
   
                       DAYLIGHT_SENSOR_COMPONENT_STRUCT       DaylightSensorComponent[MAX_PS_SENSORS_ALLOWED_PER_ZONE * MAX_NUMBER_ZONES];   
                       OCC_GROUP_OBJECT_FLAT_FILE_STRUCT      OccGroupObject[MAX_NUMBER_AREAS];   
                       DAYLIGHTING_REGION_OBJECT_FLAT_FILE_STRUCT  DaylightRegionObject[MAX_NUMBER_AREAS];   
                       GAIN_GROUP_OBJECT_FLAT_FILE_STRUCT     GainGroupObject[MAX_NUMBER_SWITCHLEGS];    
                       SCENE_OBJECT_FLAT_FILE_STRUCT          SceneObject[MAX_SCENES*MAX_NUMBER_AREAS];  
                       PRESET_ASSIGNMENT_OBJECT_FLAT_FILE_STRUCT  PresetAssignmentObject[MAX_SCENES * MAX_NUMBER_ZONES];      
                       UINT32                                 QSMSoureceAddress;
                       UINT8                                  OccSensorState;   
                       UINT8                                  OccConnectedState;
                       IR_PRESET_ZONE_LEVELS                  IRSensorPresetLevel[MAX_PHYSICAL_IR_SENSORS]; 
                       UINT8                                  DaylightConnectedState;   
                       UINT8                                  IrConnectedState;
                       UINT8                                  IRSensorType[MAX_PHYSICAL_IR_SENSORS];
   
                       #ifdef ESN_INTERNATIONAL
                       UINT8                                  TempCrossedThresholdCount;
                       #endif
   
                       UINT8                                  OccEnableBitMapForArea;
                       UINT8                                  DaylightEnableBitMapForSwLeg;


                       //***************************************************
                       // Make sure the preset table is after all flat files
                       //***************************************************
                       FPAT_NV_STRUCT                         presetEntry[FPAT__NUMBER_OF_ENTRIES];
     
                       //******************************
                       // Initialization signature is the last thing in the DB
                       //******************************  
                       UINT16                           IntializationSignature;   
                       UINT16                           databaSizeOffset;    
   
                       // This is outside the database (it doesn't get backed up)                
                       ENUM_DATABASE_RESTORE_SIGNATURE  databaRestoreSignature;
   
   
                    } FLASH_DATABASE_LAYOUT;

            */
            unpack += "\n End of Database 1172409 ";
            return unpack;
        }

        /// <summary>
        /// Unpack DEVICE_INFO_STRUCT
        /// </summary>
        /// <returns></returns>
        public string Unpack_DEVICE_INFO_STRUCT()
        {
            string unpack = string.Empty;
            unpack += "\nDEVICE INFO STRUCTURE\n";
            unpack += "\n    Device Serial Number: " + esnbu.GetUint32().ToString("X8");
            unpack += "\n\n  ---- Reserved ----";
            unpack += esnbu.GetBytes(64);
            unpack += "\n  ---- Device Model Numbers ----";
            unpack += esnbu.GetBytes(32);
            unpack += "\n  ---- Date Code ----";
            unpack += esnbu.GetBytes(8);
            unpack += "\n\n                Family: " + esnbu.GetByte().ToString("X2");
            unpack += "\n                 Product: " + esnbu.GetByte().ToString("X2");
            unpack += "\n                Hardware: " + esnbu.GetByte().ToString("X2");
            unpack += "\n                  Custom: " + esnbu.GetByte().ToString("X2");
            unpack += "\n    15 Minutes Signature: " + esnbu.GetByte().ToString("X2");
            esnbu.DatabaseRevision = esnbu.GetUint16();
            unpack += "\n       Database Revision: " + esnbu.DatabaseRevision.ToString();
            return unpack;
        }

        /// <summary>
        /// Unpack DEVICE_DATA_STRUCT
        /// </summary>
        public string Unpack_DEVICE_DATA_STRUCT()
        {
            string unpack = string.Empty;
            unpack += "\nDEVICE DATA STRUCTURE\n";
            unpack += "\n             Device mode: " + esnbu.GetByte().ToString("X2");
            unpack += "\n                reserved: " + esnbu.GetByte().ToString("X2");
            unpack += "\n         Device ACK Slot: " + esnbu.GetByte().ToString("X2");
            unpack += "\n\n  ---- Reserved ----";
            unpack += esnbu.GetBytes(4);
            unpack += "\nSecurityLockout Password:" + esnbu.GetUint16().ToString("X4");
            unpack += "\n             Device Name:" + esnbu.GetUniString(32);
            return unpack;
        }
    }
}
