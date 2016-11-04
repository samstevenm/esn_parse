using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ESN_Utilities
{
    public class DumperECO : ESNDumper
    {
        override public string Unpack()
        {
            int loop;
            int idx;
            int aidx;

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <BOOT_AND_OS_SHARED_DATABASE_DATA>\n";

            unpack += "\nBeginning Of Flash:  " + esnbu.GetByte().ToString("X2");
            unpack += "\nProtective Buffer:  " + esnbu.GetBytes(15);
            unpack += "\n ";
            unpack += "\n  STATIC DEVICE DATA\n";
            unpack += "\nSerial No:  " + esnbu.GetUint32().ToString("X8");
            unpack += "\nFamily: " + esnbu.GetByte().ToString("X2");
            unpack += "\nProduct: " + esnbu.GetByte().ToString("X2");
            unpack += "\nHardware: " + esnbu.GetByte().ToString("X2");
            unpack += "\nCustom: " + esnbu.GetByte().ToString("X2");
            unpack += "\nModel #s:" + esnbu.GetBytes(33);
            unpack += "\n \n";
            unpack += "\nProtective Buffer: " + esnbu.GetBytes(16);

            unpack += "\n </BOOT_AND_OS_SHARED_DATABASE_DATA>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n <TEMP_OS_METADATA>\n";

            unpack += "\nSafe Area:" + esnbu.GetBytes(256);
            unpack += "\n\nReboot ReProgram OS Flag: " + esnbu.GetUint32().ToString("X8");
            unpack += "\n# of srecords_expected: " + esnbu.GetUint32().ToString("X8");
            unpack += "\nAddress of OS Start: " + esnbu.GetUint32().ToString("X8");
            unpack += "\nSafe Area:" + esnbu.GetBytes(256);

            unpack += "\n</TEMP_OS_METADATA>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
           
            unpack += "\n <RUNTIME_DEVICE_INFO>";

            unpack += "\nInitialization Signature: " + esnbu.GetUint32().ToString("X8");
            unpack += "\nDatabase Revision: " + esnbu.GetUint32().ToString("X8");
            unpack += "\nDatabase Size: " + esnbu.GetUint32().ToString();
            unpack += "\nESN Name: " + esnbu.GetUniString(16);
            unpack += "\nDevice GUI Group #: " + esnbu.GetByte().ToString("X2");
            unpack += "\nDevice Mode: " + esnbu.GetEnum8().ToString("X2");
            unpack += "\nDevice Submode: " + esnbu.GetEnum8().ToString("X2");
            unpack += "\nDevice Programmed: " + esnbu.GetByte().ToString("X2");
            unpack += "\nPreset Table Signature: " + esnbu.GetUint32().ToString("X8");
            unpack += "\nDevice Scene Save Mode: " + esnbu.GetEnum8().ToString("X4");
            unpack += "\nMinimum Level Enum: " + esnbu.GetEnum8().ToString("X4");
            unpack += "\nSensor Enabled Flags: " + esnbu.GetUint16().ToString("X4");
            unpack += "\n# Switchlegs: " + esnbu.GetUint16().ToString();
            unpack += "\n# IR Sensors: " + esnbu.GetUint16().ToString();
            unpack += "\n# Photo Sensors: " + esnbu.GetUint16().ToString();
            unpack += "\n# Occ Sensors: " + esnbu.GetUint16().ToString();
            unpack += "\n# Zones: " + esnbu.GetUint16().ToString();
            unpack += "\n# Areas: " + esnbu.GetUint16().ToString();
            unpack += "\n# Gain Groups: " + esnbu.GetUint16().ToString();
            unpack += "\n# Scenes: " + esnbu.GetUint16().ToString();
            unpack += "\n# Occ Groups: " + esnbu.GetUint16().ToString();
            unpack += "\n# Preset Assignments: " + esnbu.GetUint16().ToString();
            unpack += "\nProgrammed By: " + esnbu.GetEnum8().ToString();
            unpack += "\nDevice Password: " + esnbu.GetString(34);
            unpack += "\nDevice Password Key: " + esnbu.GetUint16().ToString();
            unpack += "\nAfter Hours Master Serial #: " + esnbu.GetUint32().ToString("X8");
            unpack += "\nOOBing Status: " + esnbu.GetEnum8().ToString();

            unpack += "\n </RUNTIME_DEVICE_INFO>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            
            unpack += "\n<UserData>" + esnbu.GetByte().ToString("X2");
            unpack += "\n </UserData>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<EEProm>" + esnbu.UnpackSMSCEEProm();
            unpack += "\n</EEProm>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<TempOSStorageStartingLocation>" + esnbu.GetByte().ToString("X2");
            unpack += "\n</TempOSStorageStartingLocation>";
            unpack += "\n";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<SWITCHLEGS>";

            for (idx = 0;idx < 128;idx++)
            {
                string unpackedff = esnbu.UnpackSwitchlegFlatFile();
                byte bZone = esnbu.GetByte();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<Switchleg>";
                    unpack += "\n<SwitchlegNum>" + (idx + 1).ToString() + "</SwitchlegNum>";
                    unpack += "\n  <Zone>" + bZone.ToString()+"</Zone>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2")+ "</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</Switchleg>";
                }
            }
            unpack += "\n</SWITCHLEGS>";

            unpack += "\n";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<IR_SENSOR_COMPONENTS>";

            for (idx = 0; idx < 132; idx++)
            {
                string unpackedff = esnbu.UnpackIRSensor();
                byte bFlags = esnbu.GetByte();
                byte bType = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<IRSensor" + (idx + 1).ToString() + ">";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2")+"</Flag>";
                    unpack += "\n  <Type>" + bType.ToString()+"</Type>";
                    unpack += unpackedff;
                    unpack += "\n</IRSensor" + (idx + 1).ToString() + ">";
                }
            }
            unpack += "\n</IR_SENSOR_COMPONENTS>";

            unpack += "\n";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<HHD_IR_SENSOR_COMPONENTS>";
            
            for (idx = 0; idx < 232; idx++)
            {
                string unpackedff = esnbu.UnpackHHDIRSensor();
                byte bFlags = esnbu.GetByte();
                byte bType = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<HDDIRSensor>";

                    unpack += "\n<HDDIRSensorNum>" + (idx + 1).ToString() + "</HDDIRSensorNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") + "</Flag>";
                    unpack += "\n  <Type>" + bType.ToString() + "</Type>";
                    unpack += unpackedff;

                    unpack += "\n</HDDIRSensor>";
                }
            }
            unpack += "\n</HHD_IR_SENSOR_COMPONENTS>";
            unpack += "\n";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<PHOTO_SENSOR_COMPONENTS>";

            for (idx = 0;idx < 132;idx++)
            {
                string unpackedff = esnbu.UnpackPhotoSensor();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<HHDPhotoSensor>";
                    unpack += "\n<PhotoSensorNum>" + (idx + 1).ToString() + "</PhotoSensorNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") + "</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</HHDPhotoSensor>";
                }
            }
            unpack += "\n";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n</PHOTO_SENSOR_COMPONENTS>";
            unpack += "\n<HHDPhotoSensorIDs>";

            for (idx = 0;idx < 232;idx++)
            {
                string unpackedff = esnbu.UnpackHHDPhotoSensor();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<HHDPhotoSensorNum>" + (idx + 1).ToString() + "</HHDPhotoSensorNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") + "</Flag>";
                    unpack += unpackedff;
                    
                }
            }
            unpack += "\n</HHDPhotoSensorIDs>";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n<OCC_SENSOR_COMPONENTS>";
            for (idx = 0;idx < 132;idx++)
            {
                string unpackedff = esnbu.UnpackOccSensor();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<OccSensor>";
                    unpack += "\n<OccSensorNum>" + (idx + 1).ToString() + "</OccSensorNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") + "</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</OccSensor>";
                }
            }
            unpack += "\n";

            unpack += "\n</OCC_SENSOR_COMPONENTS>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n<HHD_OCC_SENSOR_COMPONENTS>";
            for (idx = 0;idx < 232;idx++)
            {
                string unpackedff = esnbu.UnpackHHDOccSensor();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<HHDOccSensor>";
                    unpack += "\n<HHDOccSensorNum>" + (idx + 1).ToString() + "</HHDOccSensorNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") + "</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</HHDOccSensor>";
                }
            }
            unpack += "\n</HHD_OCC_SENSOR_COMPONENTS>";
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n<ZONE_COMPONENTS>";
            for (idx = 0;idx < 128;idx++)
            {
                string unpackedff = esnbu.UnpackZone();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<Zone>";
                    unpack += "\n<ZoneNum>" + (idx + 1).ToString()+"</ZoneNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2")+"</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</Zone>";
                }
            }
            unpack += "\n";

            unpack += "\n</ZONE_COMPONENTS>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n<AREA_COMPONENTS>";
            for (idx = 0;idx < 128;idx++)
            {
                string unpackedff = esnbu.UnpackArea();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n <Area>";
                    unpack += "\n <AreaNum>" + (idx + 1).ToString()+"</AreaNum>";
                    unpack += "\n <Flag>" + bFlags.ToString("X2") + "</Flag>";
                    unpack += unpackedff;
                    unpack += "\n </Area>";
                    
                }
            }
            
            unpack += "\n";

            unpack += "\n</AREA_COMPONENTS>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<RemoteSceneControllers>";

            for (idx = 0;idx < 100;idx++)
            {
                unpack += "\n<RemoteSceneController>";
                unpack += "\n <RemoteSceneControllerNum>" + idx.ToString() +"</RemoteSceneControllerNum>";
              
                unpack += esnbu.UnpackRemotePM();
                unpack += "\n</RemoteSceneController>";
            }
            unpack += "<number_of_valid_remote_scene_controllers>" + esnbu.GetByte().ToString();
            unpack += "\n </number_of_valid_remote_scene_controllers>";
            unpack += "\n";

            unpack += "\n</RemoteSceneControllers>";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n<DAYLIGHT_REGIONS_70374>";
            for (idx = 0; idx < 128; idx++)
            {
                string unpackedff = esnbu.UnpackDaylightRegion();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n<DaylightRegion>";
                    unpack += "\n<DaylightRegionNum>" + (idx + 1).ToString() + "</DaylightRegionNum>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") +"</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</DaylightRegion>";
                }
            }
            

            unpack += "\n</DAYLIGHT_REGIONS_70374>";
           
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
                       
            unpack += "\n<GAIN_GROUPS_74342>";

            for (idx = 0; idx < 128; idx++)
            {
                string unpackedff = esnbu.UnpackGainGroup();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\n<GainGroup>" + (idx + 1).ToString() +"</GainGroup>";
                    unpack += "\n  <Flag>" + bFlags.ToString("X2") +"</Flag>";
                    unpack += unpackedff;
                }
            }
            unpack += "\n";

            unpack += "\n</GAIN_GROUPS_74342>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <SCENES_76646>";
            for (aidx = 0; aidx < 128; aidx++)
            {
                for (idx = 0; idx < 17; idx++)
                {
                        string unpackedff = esnbu.UnpackScene();
                        byte bFlags = esnbu.GetByte();
                        if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0) //scene flag is not valid if scene is not modified.
                        {
                            unpack += "\n";
                            unpack += "\n<Area>" + (aidx + 1).ToString() +"</Area>";
                            unpack += "\n<Scenes>" + (idx).ToString()+ "</Scenes>";
                            unpack += "\n  <Flag>" + bFlags.ToString("X2") +"</Flag>";
                            unpack += unpackedff;
                        }
                }
            }
            unpack += "\n";

            unpack += "\n </SCENES_76646>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n<OccGroups_300774>";
            for (idx = 0; idx < 128; idx++)
            {
                string unpackedff = esnbu.UnpackOccGroups();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n<OccGroup>";
                    unpack += "\n<OccGroupNum>" + (idx + 1).ToString() + "</OccGroupNum>";
                    unpack += "\n<Flag>" + bFlags.ToString("X2") +"</Flag>";
                    unpack += unpackedff;
                    unpack += "\n</OccGroup>";
                }
            }
            unpack += "\n";

            unpack += "\n </OccGroups_300774>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";

            unpack += "\n <presetAssignments_311526>";
            for (aidx = 0; aidx < 128; aidx++)
            {
                for (idx = 0; idx < 17; idx++)
                {
                    string unpackedff = esnbu.UnpackPresetAssignments();
                    byte bFlags = esnbu.GetByte();
                    if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0) //scene flag is not valid if scene is not modified.
                    {
                        unpack += "\n";
                        unpack += "\n<Preset>" + (idx).ToString() + "</Preset>";
                        unpack += "\n<Zone>" + (aidx + 1).ToString() + "</Zone>";
                        unpack += "\n<Flag>" + bFlags.ToString("X2") + "</Flag>";
                        unpack += unpackedff;
                    }
                }
            }
            unpack += "\n";

            unpack += "\n </presetAssignments_311526>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <OCC_SENSOR_NAMES_355046>";
            unpack += "\n";
            for (idx = 0; idx < 132; idx++)
            {
                unpack += "\n <OccSensorComponent>" + (idx + 136).ToString("X3") + ":  " + esnbu.GetUniString(16) + "</OccSensorComponent>";
            }
            unpack += "\n";

            unpack += "\n </OCC_SENSOR_NAMES_355046>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <PHOTO_SENSOR_NAMES_359270>";
            unpack += "\n";
            for (idx = 0; idx < 132; idx++)
            {
                unpack += "\n <PhotoSensorComponent>" + (idx + 268).ToString("X3") + ":  " + esnbu.GetUniString(16) + "</PhotoSensorComponent>";
            }
            unpack += "\n";

            unpack += "\n </PHOTO_SENSOR_NAMES_359270>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <IR_SENSOR_NAMES_363494>";
            unpack += "\n";
            for (idx = 0; idx < 132; idx++)
            {
                unpack += "\n <IRSensorComponent>" + (idx + 400).ToString("X3") + ":  " + esnbu.GetUniString(16) + "</IRSensorComponent>";
            }
            unpack += "\n";

            unpack += "\n </IR_SENSOR_NAMES_363494>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <areaLoadShedPercentage367718>";
            unpack += esnbu.GetBytes(128);
            unpack += "\n";

            unpack += "\n </areaLoadShedPercentage367718>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <EsnEcoInternalTableBuiltForOOB367846>";
            unpack += esnbu.GetBytes(1);
            unpack += "\n";

            unpack += "\n </EsnEcoInternalTableBuiltForOOB367846>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <ESN_ECO_INTERNAL_ZONE_TABLE_367847>";
            for (idx = 0; idx < 128; idx++)
            {
                UInt16 bFlags = esnbu.GetUint16();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nZone " + (idx + 1).ToString();
                    unpack += "\nValid " + bFlags.ToString("X4");
                    unpack += "\n          Zone Component Number: " + esnbu.GetUint16().ToString();
                    unpack += "\n Internal Zone Component Number: " + esnbu.GetUint16().ToString() + ":" + esnbu.GetUint16().ToString();
                    unpack += "\n               Switchleg Bitmap: ";
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += "|";
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += "\n";
                }
                else
                {
                    esnbu.SkipBytes(22);
                }
            }
            unpack += "\n";

            unpack += "\n </ESN_ECO_INTERNAL_ZONE_TABLE_367847>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <EsnEcoInternalZoneBitmap370919>";
            unpack += esnbu.GetBytes(16);
            unpack += "\n";

            unpack += "\n </EsnEcoInternalZoneBitmap370919>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <ESN_ECO_INTERNAL_AREA_TABLE_370935>";
            for (idx = 0; idx < 128; idx++)
            {
                UInt16 bFlags = esnbu.GetUint16();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nArea " + (idx + 1).ToString();
                    unpack += "\nValid " + bFlags.ToString("X4");
                    unpack += "\n          Area Component Number: " + esnbu.GetUint16().ToString();
                    unpack += "\n Internal Area Component Number: " + esnbu.GetUint16().ToString() + ":" + esnbu.GetUint16().ToString();
                    unpack += "\n               Switchleg Bitmap: ";
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += "|";
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += "\n";
                }
                else
                {
                    esnbu.SkipBytes(22);
                }
            }
            unpack += "\n";

            unpack += "\n </ESN_ECO_INTERNAL_AREA_TABLE_370935>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <EsnEcoInternalAreaBitmap374007>";
            unpack += esnbu.GetBytes(16);
            unpack += "\n";

            unpack += "\n </EsnEcoInternalAreaBitmap374007>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <ESN_ECO_INTERNAL_GAIN_GROUP_TABLE_374023>";
            for (idx = 0; idx < 128; idx++)
            {
                UInt16 bFlags = esnbu.GetUint16();
                if (bFlags != 0)
                {
                    unpack += "\n";
                    unpack += "\nArea " + (idx + 1).ToString();
                    unpack += "\nValid " + bFlags.ToString("X4");
                    unpack += "\n          G.G. Component Number: " + esnbu.GetUint16().ToString();
                    unpack += "\n Internal G.G. Component Number: " + esnbu.GetUint16().ToString() + ":" + esnbu.GetUint16().ToString();
                    unpack += "\n               Switchleg Bitmap: ";
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += "|";
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += esnbu.GetUint32().ToString("X8");
                    unpack += "\n";
                }
                else
                {
                    esnbu.SkipBytes(22);
                }
            }
            unpack += "\n";

            unpack += "\n </ESN_ECO_INTERNAL_GAIN_GROUP_TABLE_374023>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <EsnEcoInternalGainGroupBitmap377095>";
            unpack += esnbu.GetBytes(16);
            unpack += "\n";

            unpack += "\n </EsnEcoInternalGainGroupBitmap377095>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n  <PHOTO_SENSOR_TABLE_377111>\n";
            for (loop = 0; loop < 2; loop++)
            {
                for (idx = 0; idx < 16; idx++)
                {
                    byte bFlag = esnbu.GetByte();
                    if (bFlag != 0)
                    {
                        unpack += "\n    Photo Sensor Table Element " + (loop + 1).ToString() + " - " + (idx + 1).ToString() + ":";
                        unpack += "\n            Valid Flag: " + bFlag.ToString();
                        unpack += "\n              Serial #: " + esnbu.GetUint32().ToString("X8");
                        unpack += "\n            Componet #: " + esnbu.GetUint16().ToString("X4");
                        unpack += "\n   Internal Componet #: " + esnbu.GetUint16().ToString("X4");
                    }
                    else
                    {
                        esnbu.SkipBytes(8);
                    }
                }
            }

            unpack += "\n   Update With Eco Status: " + esnbu.GetByte().ToString();
            unpack += "\n";

            unpack += "\n  </PHOTO_SENSOR_TABLE_377111>\n";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <EsnPhotoSesnorTableUpdateRequired377400>";
            unpack += "\n   Photo Sesnor Table Update Required: " + esnbu.GetByte().ToString();
            unpack += "\n";

            unpack += "\n </EsnPhotoSesnorTableUpdateRequired377400>";

            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n <CCIComponentFlatfiles377401>\n";
            for (idx = 0; idx < 1; idx++)
            {
                string strUnp = esnbu.UnpackPROG_CCI_COMPONENT_HHD_STRUCT();
                byte bFlags = esnbu.GetByte();
                if (((COMPONENT_FLAGS)bFlags & COMPONENT_FLAGS.FLAG_IS_COMPONENT_VALID_BIT) != 0)
                {
                    unpack += "\n";
                    unpack += "\nCCI " + (idx + 1).ToString();
                    unpack += "\nValid " + bFlags.ToString("X2");
                    unpack += strUnp;

                }
            }
            unpack += "\n </CCIComponentFlatfiles377401>\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n deviceDatabaseTypeSignature	377419 ";
            unpack += esnbu.GetBytes(4);
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n  Addressed and Channelized Bitmaps	377423 ";
            unpack += "\n               Addressed Bitmap: ";
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += "|";
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += "\n             Channelized Bitmap: ";
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += "|";
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += esnbu.GetUint32().ToString("X8");
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n External Area Table	377455 ";
            for (idx = 0; idx < 200; idx++)
            {
                UInt32 rdSerialNo = esnbu.GetUint32();
                UInt16 raCompNo = esnbu.GetUint16();
                UInt16 SensorCompNo = esnbu.GetUint16();
                if (rdSerialNo != 0)
                {
                    unpack += "\n";
                    unpack += "\n    Remote Device Serial Number: " + rdSerialNo.ToString("X8");
                    unpack += "\n   Remote Area Component Number: " + raCompNo.ToString("X4");
                    unpack += "\n        Sensor Component Number: " + SensorCompNo.ToString("X4");
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n daliBoardDBSyncStatus	379055 ";
            unpack += esnbu.GetBytes(4);
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n Preset Table	379059 ";
            for (idx = 0; idx < 1025; idx++)
            {
                UInt32 prgrp32 = esnbu.GetUint32();
                byte prgrpt1 = esnbu.GetByte();
                byte prgrpt2 = esnbu.GetByte();
                if (prgrp32 != 0)
                {
                    unpack += "\n";
                    unpack += "\n Preset Entry " + (idx + 1).ToString();
                    unpack += "\n            Preset Group Number: " + prgrp32.ToString("X8") + "|" + prgrpt1.ToString("X2") + "|" + prgrpt1.ToString("X2");
                    for (int idx2 = 0;idx2 < 128;idx2++)
                    {
                        if(0 == (idx2 % 8))
                        {
                            unpack += "\n                Zones ";
                            unpack += idx2.ToString("D3");
                            unpack += " - ";
                            unpack += (idx2 + 7).ToString("D3");
                        }
                        UInt16 level = esnbu.GetUint16();
                        UInt16 fade = esnbu.GetUint16();
                        UInt16 delay = esnbu.GetUint16();
                        unpack += "[ ";
                        if (level != 0xFF00)
                        {
                            unpack += level.ToString("X4");
                        }
                        else
                        {
                            unpack += "    ";
                        }
                        unpack += ", ";
                        if (fade != 8)
                        {
                            unpack += fade.ToString("X4");
                        }
                        else
                        {
                            unpack += "    ";
                        }
                        unpack += ", ";
                        if (delay != 0)
                        {
                            unpack += delay.ToString("X4");
                        }
                        else
                        {
                            unpack += "    ";
                        }
                        unpack += "]";
                    }
                }
                else
                {
                    esnbu.SkipBytes(128 * 6);
                }
            }
            unpack += "\n";
            unpack += "<!--" + esnbu.DumpPosition() + "-->";
            unpack += "\n End of Database 1172409";
            return unpack;
        }

/*
  
   //THIS HAS TO BE THE FIRST THING IN HERE!!!!!!!!!!!!!!!//
   BOOT_AND_OS_SHARED_DATABASE_DATA boot_and_os_shared_data;
   //THIS HAS TO BE THE FIRST THING IN HERE!!!!!!!!!!!!!!!//
   
   UINT8                         beginningOfUserData;
   
// OOPS... wrong structure was put here
// should have had SMSC_EEPROM instead of ETHERNET_DATA
// added padding to keep the rest the same in existing databases
//   ETHERNET_DATA                 ethernetData;
   SMSC_EEPROM                   ethernetAddressData;
//   UINT8                         padding1[sizeof(ETHERNET_DATA) - sizeof(SMSC_EEPROM)];
// NOTE: the MAGIC NUMBER was the ETHERNET_DATA - SMSC_EEPROM before the socket count change.
// end of ethernet stuff   

                                        
   //UINT8                         os_reprogram_metadata_safe_area1[256]; //TBD-moved to device.h file, BOOT_AND_OS_SHARED_DATABASE_DATA, TEMP_OS_METADATA
   //UINT32                        reboot_reprogram_os_flag;              //TBD-moved to device.h file, BOOT_AND_OS_SHARED_DATABASE_DATA, TEMP_OS_METADATA
   //UINT32                        num_s_records_expected_for_os_upgrade; //TBD-moved to device.h file, BOOT_AND_OS_SHARED_DATABASE_DATA, TEMP_OS_METADATA
   //UINT8                         os_reprogram_metadata_safe_area2[256]; //TBD-moved to device.h file, BOOT_AND_OS_SHARED_DATABASE_DATA, TEMP_OS_METADATA
   
   UINT8                         temp_os_storage_starting_location;                            
                                        
   SWLEG_COMPONENT_STRUCT        swlComponents[MAX_NUMBER_OF_SWITCHLEGS];
   IR_SENSOR_COMPONENT_STRUCT    irSensorComponents[MAX_NUMBER_OF_IR_SENSORS]; 
   IR_SENSOR_HHD_STRUCT          irSensorHHDComponents[MAX_NUMBER_OF_IR_SENSORS + MAX_ALLOWED_EXTERNAL_IR_SENSORS]; 
   PHOTO_SENSOR_STRUCT           photoSensorComponents[MAX_NUMBER_OF_PHOTO_SENSORS];
   PHOTO_SENSOR_HHD_STRUCT       photoSensorHHDComponents[MAX_NUMBER_OF_PHOTO_SENSORS + MAX_ALLOWED_EXTERNAL_PHOTO_SENSORS];
   OCC_SENSOR_STRUCT             occSensorComponents[MAX_NUMBER_OF_OCC_SENSORS];  
   OCC_SENSOR_HHD_STRUCT         occSensorHHDComponents[MAX_NUMBER_OF_OCC_SENSORS + MAX_ALLOWED_EXTERNAL_OCC_SENSORS];  

 * 
 * 
 * 
   ZONE_STRUCT                                           zones[MAX_NUMBER_OF_ZONES];
   AREA_STRUCT                                           areas[MAX_NUMBER_OF_AREAS];
   DFC_REMOTE_SCENECONTROLLERS_PM_DEVICES_TABLE_STRUCT   dfcRemoteSceneControllersPMdeviceTable;
   DAYLIGHTING_REGION_STRUCT                             daylightingRegions[MAX_NUMBER_OF_DAYLIGHTING_REGIONS];
   GAINGROUP_STRUCT                                      gainGroups[MAX_NUMBER_OF_GAINGROUPS];  
   SCENE_STRUCT                                          scenes[MAX_NUMBER_OF_SCENES];   
   OCCGRP_STRUCT                                         occGroups[MAX_NUMBER_OF_OCC_GROUPS];      

   PRESETASSGN_STRUCT                                    presetAssignments[MAX_NUMBER_OF_ZONES * MAX_NUMBER_OF_SCENES_PER_AREA];  
   
   OCC_SENSOR_NAMES_STRUCT                               occSensorNames[MAX_NUMBER_OF_OCC_SENSORS];
   PHOTO_SENSOR_NAMES_STRUCT                             photoSensorNames[MAX_NUMBER_OF_PHOTO_SENSORS];
   IR_SENSOR_NAMES_STRUCT                                irSensorNames[MAX_NUMBER_OF_IR_SENSORS];
   
   UINT8                                                 areaLoadShedPercentage[MAX_NUMBER_OF_AREAS];       

   BOOL                                                  EsnEcoInternalTableBuiltForOOB;
   ESN_ECO_INTERNAL_ZONE_TABLE                           EsnEcoInternalZoneMapTable;
   ESN_ECO_INTERNAL_ZONE_BITMAP                          EsnEcoInternalZoneBitmap;
   ESN_ECO_INTERNAL_AREA_TABLE                           EsnEcoInternalAreaMapTable;
   ESN_ECO_INTERNAL_AREA_BITMAP                          EsnEcoInternalAreaBitmap;
   ESN_ECO_INTERNAL_GAIN_GROUP_TABLE                     EsnEcoInternalGainGroupTable;
   ESN_ECO_INTERNAL_GAIN_GROUP_BITMAP                    EsnEcoInternalGainGroupBitmap;

   PHOTO_SENSOR_TABLE                                    EsnEcoPhotoSensortable;
   BOOL                                                  EsnPhotoSesnorTableUpdateRequired;

   PROG_CCI_COMPONENT_HHD_STRUCT                       	progCCIHHDComponents[MAX_NUMBER_OF_PROG_CCI];

   //TELLS US WHAT TO DO WITH THE RECEIVED DATABASE
   UINT32                                                deviceDatabaseTypeSignature;

   //we need to store the addressed and channelized bitmap from the bottom board
   //so that the db xfer/cloning/device replacement works.
   UINT8                         daliBoardAddressedAndChannelizedBitmaps[MAX_BITMAP_DATA_SIZE];

   EXTERNAL_AREA_ASSIGNMENT_TABLE                        externalAreaTable[MAX_NUMBER_OF_EXTERNAL_ASSIGNED_AREAS];

   //this flag will indicate whether we are in sync with the bottom board
   //Needs to be set to its proper value once all the FFs and bitmaps are
   //pushed back to the bottom board after db conversion/xfer.
   UINT32                        daliBoardDBSyncStatus;

   //******************************
   // Make sure the preset table is the last
   // thing in this structure!!!!
   //******************************
   PRESET_TABLE_STRUCT           presetTable;
   //******************************
   // Make sure the preset table is the last
   // thing in this structure!!!!
   //****************************** 
 */ 
    }
}
