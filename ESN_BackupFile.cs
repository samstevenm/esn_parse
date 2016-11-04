using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ESN_Utilities
{
    public enum COMPONENT_FLAGS
    {
        FLAG_IS_COMPONENT_VALID_BIT = 0x80
    }

    public class ESNDumper
    {
        protected string unpack = string.Empty;
        public ESNBackupFile esnbu = null;
        public bool International = false;
        public UInt32 DeviceType = 0xFFFFFFFF;

        virtual public string Unpack()
        {
            return "Unknown Device Type";
        }
    }


    public class ESNBackupFile
    {
        private string strESN_SerialNo;
        private string strESN_Name;
        private UInt32 DeviceType = 0xFFFFFFFF;
        private DateTime dtStampImage;
        private byte [] imagebytes;
        private int    position;

        public UInt16 DatabaseRevision = 0xFFFF;

        /// <summary>
        /// Initialize Dumper and load backup file
        /// </summary>
        /// <param name="filename"></param>
        public ESNBackupFile(string filename)
        {
            StreamReader srImage = new System.IO.StreamReader(filename, Encoding.BigEndianUnicode);
            strESN_SerialNo = srImage.ReadLine();
            strESN_Name = srImage.ReadLine();
            DeviceType = UInt32.Parse(srImage.ReadLine());
            {
                string strDT = srImage.ReadLine();
                int Year = int.Parse(strDT.Substring(0,4));
                int Month = int.Parse(strDT.Substring(4,2));
                int Day = int.Parse(strDT.Substring(6,2));
                int Hour = int.Parse(strDT.Substring(9,2));
                int Min = int.Parse(strDT.Substring(11,2));
                dtStampImage = new DateTime(Year, Month, Day, Hour, Min,0);
            }
            string buffer = srImage.ReadLine();
            imagebytes = new byte[buffer.Length / 2];
            int bufferpos = 0;
            for (int idx = 0; idx < imagebytes.Length; idx++)
            {
                imagebytes[idx] = byte.Parse(buffer.Substring(bufferpos, 2),System.Globalization.NumberStyles.AllowHexSpecifier);
                bufferpos += 2;
            }
            position = 0;
        }

        /// <summary>
        /// Get Device specific dumper
        /// </summary>
        /// <returns></returns>
        public ESNDumper GetDumper()
        {
            ESNDumper dmpr;
            switch (DeviceType)
            {
                case 0x09020000:    // ECO
                    dmpr = new DumperECO();
                    dmpr.International = false;
                    break;

                case 0x09080000:    // ECO INTERNATIONAL
                    dmpr = new DumperECO();
                    dmpr.International = true;
                    break;

                case 0x09030000:    // 0-10 INTERNATIONAL
                case 0x09040000:    // SWITCHING INTERNATIONAL
                    dmpr = new Dumper0To10();
                    dmpr.International = true;
                    break;

                case 0x09050000:    // 0-10 DOMESTIC
                case 0x09060000:    // SOFT SWITCH DOMESTIC
                    dmpr = new Dumper0To10();
                    dmpr.International = false;
                    break;


                case 0x090A0000:    // PHASE ADAPTIVE INTERNATIONAL
                case 0x090B0000:    // PHASE ADAPTIVE DOMESTIC
                case 0x09010000:    // DALI
//                case 0x09070000:    // ECO 1 LOOP *** DOESN'T REALLY EXIST
//                case 0x09090000:    // ECO 1 LOOP INTERNATIONAL
                default:
                    dmpr = new ESNDumper();
                    break;
            }
            dmpr.DeviceType = DeviceType;
            dmpr.esnbu = this;
            return dmpr;
            
        }

        /// <summary>
        /// Display current position in File
        /// </summary>
        /// <returns></returns>
        public string DumpPosition()
        {
            return "\n*** POSITION " + position.ToString() + " ***\n";
        }

        /// <summary>
        /// Get a char from the dump file
        /// </summary>
        /// <returns></returns>
        public char GetChar()
        {
            return (char)imagebytes[position++];
        }

        /// <summary>
        /// Get a 8 bit enum from file
        /// </summary>
        /// <returns></returns>
        public byte GetEnum8()
        {
            return imagebytes[position++];
        }

        /// <summary>
        /// Get UNICode character from file
        /// </summary>
        /// <returns></returns>
        public char GetUniChar()
        {
            position++;
            return (char)imagebytes[position++];
        }

        /// <summary>
        /// Get a byte (UINT8) from the file
        /// </summary>
        /// <returns></returns>
        public byte GetByte()
        {
            return imagebytes[position++];
        }

        /// <summary>
        /// Get a UINT16 from the file
        /// </summary>
        /// <returns></returns>
        public UInt16 GetUint16()
        {
            UInt16 temp = (UInt16)(imagebytes[position++] * 256);
            return (UInt16)(temp + imagebytes[position++]);
        }

        /// <summary>
        /// Get a UINT32 from the file
        /// </summary>
        /// <returns></returns>
        public UInt32 GetUint32()
        {
            UInt32 temp = (UInt32)imagebytes[position++] * 256 * 256 * 256;
            temp += (UInt32)imagebytes[position++] * 256 * 256;
            temp += (UInt32)imagebytes[position++] * 256;
            return temp + (UInt32)imagebytes[position++];
        }


        /// <summary>
        /// Get a string from the file
        /// </summary>
        /// <param name="length = length of string in file"></param>
        /// <returns></returns>
        public string GetString(int length)
        {
            string strRet = string.Empty;
            char val;
            bool hadTerminator = false;
            for (int idx = 0; idx < length; idx++)
            {
                val = GetChar();
                if (val == 0)
                {
                    hadTerminator = true;
                }
                else if (!hadTerminator)
                {
                    strRet += val;
                }
            }
            return strRet;
        }

        /// <summary>
        /// Get an object type from the file
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseObjectType()
        {
            return GetUint16().ToString("X4");
        }

        /// <summary>
        /// Get a GROUP_ADDRESS from the file
        /// </summary>
        /// <returns></returns>
        public string GetGroupAddress()
        {
            string unpack = string.Empty;
            unpack += GetUint32().ToString("X8");
            unpack += ":" + GetByte().ToString("X2");
            return unpack;
        }

        /// <summary>
        /// Get a COMPONENET_ADDRESS from the file
        /// </summary>
        /// <returns></returns>
        public string GetComponentAddress()
        {
            string unpack = string.Empty;
            unpack = GetGroupAddress();
            unpack += ":" + GetUint16().ToString();
            return unpack;
        }

        /// <summary>
        /// Skip bytes in the file
        /// </summary>
        /// <param name="length = # bytes to skip"></param>
        /// <returns></returns>
        public string SkipBytes(int length)
        {
            position += length;
            return "\nSkipped " + length.ToString() + " Bytes.";
        }

        /// <summary>
        /// Get bytes from the file, displayed as 16 bytes per line dump
        /// </summary>
        /// <param name="length = # of bytes to fetch"></param>
        /// <returns></returns>
        public string GetBytes(int length)
        {
            string unpack = string.Empty;
            bool   fDone = false;
            while (!fDone)
            {
                unpack += "\n";
                for (int idx = 0; idx < 16; idx++)
                {
                    if (0 == length--)
                    {
                        fDone = true;
                        break;
                    }
                    else
                    {
                        unpack += " ";
                        unpack += GetByte().ToString("X2");
                    }
                }
            }
            return unpack;
        }

        /// <summary>
        /// Get UINT16s from the file, displayed as 8 UINT16s per line dump
        /// </summary>
        /// <param name="length = # of UINT16s"></param>
        /// <returns></returns>
        public string GetUint16s(int length)
        {
            string unpack = string.Empty;
            bool fDone = false;
            while (!fDone)
            {
                unpack += "\n";
                for (int idx = 0; idx < 8; idx++)
                {
                    if (0 == length--)
                    {
                        fDone = true;
                        break;
                    }
                    else
                    {
                        unpack += " ";
                        unpack += GetUint16().ToString("X4");
                    }
                }
            }
            return unpack;
        }

        /// <summary>
        /// Get UNICODE String
        /// </summary>
        /// <param name="length = length of unicode string in characters"></param>
        /// <returns></returns>
        public string GetUniString(int length)
        {
            string strRet = string.Empty;
            char val;
            bool hadTerminator = false;
            for (int idx = 0; idx < length; idx++)
            {
                val = GetUniChar();
                if (val == 0)
                {
                    hadTerminator = true;
                }
                else if (!hadTerminator)
                {
                    strRet += val;
                }
            }
            return strRet;
        }

        /// <summary>
        /// Unpack Switchleg Flat File from current database position
        /// </summary>
        /// <returns></returns>
        public string UnpackSwitchlegFlatFile()
        {
            string unpack = string.Empty;
            unpack += "\n      <DatabaseObjectType>" + GetDatabaseObjectType() +  "</DatabaseObjectType>";
            unpack += "\n                <ObjectID>" + GetUint32().ToString("X8") + "</ObjectID>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n                <LoadType>" + GetByte().ToString("X2")+"</LoadType>";
            unpack += "\n         <OutputNumOnLink>" + GetUint16().ToString("X4")+"</OutputNumOnLink>";
            string ZoneID = GetGroupAddress();
            ZoneID = ZoneID.Remove(ZoneID.Length - 3);
            unpack += "\n                  <ZoneID>" + ZoneID +"</ZoneID>";
            string AreaID = GetGroupAddress();
            AreaID = AreaID.Remove(AreaID.Length - 3);
            unpack += "\n                  <AreaID>" + AreaID + "</AreaID>";
            string GainGroupID = GetGroupAddress();
            GainGroupID = GainGroupID.Remove(GainGroupID.Length - 3);
            unpack += "\n             <GainGroupID>" + GainGroupID + "</GainGroupID>";
            unpack += "\n             <PhysicalMin>" + GetUint16().ToString("X4")+"</PhysicalMin>";
            unpack += "\n             <PhysicalMax>" + GetUint16().ToString("X4")+"</PhysicalMax>";
            unpack += "\n              <DeviceType>" + GetByte().ToString("X2") + "</DeviceType>";
            unpack += "\n                 <!--     " + GetByte().ToString("X2") + "-->";
            unpack += "\n                 <ModelID>" + GetUint16().ToString("X4")+"</ModelID>";
            unpack += "\n                 <HighEnd>" + GetUint16().ToString("X4")+"</HighEnd>";
            unpack += "\n                  <LowEnd>" + GetUint16().ToString("X4")+"</LowEnd>";
            unpack += "\n   <ElectronicBypassLevel>" + GetUint16().ToString("X4")+"</ElectronicBypassLevel>";
            unpack += "\n     <ManualOverrideLevel>" + GetUint16().ToString("X4")+"</ManualOverrideLevel>";
            unpack += "\n        <AbsoluteMinLevel>" + GetUint16().ToString("X4")+"</AbsoluteMinLevel>";
            unpack += "\n        <AbsoluteMaxLevel>" + GetUint16().ToString("X4")+"</AbsoluteMaxLevel>";
            unpack += "\n           <LightingFlags>" + GetUint16().ToString("X4")+"</LightingFlags>";
            unpack += "\n                <BlipData>" + GetByte().ToString("X2")+"</BlipData>";
            unpack += "\n             <InrushDelay>" + GetByte().ToString("X2")+"</InrushDelay>";
            unpack += "\n              <BurnInTime>" + GetByte().ToString("X2")+"</BurnInTime>";
            unpack += "\n   <LampRunHoursThreshold>" + GetUint16().ToString("X4") + "</LampRunHoursThreshold>";
            unpack += "\n                 <!--     " + GetUint16().ToString("X4");
            unpack += "\n                          " + GetUint16().ToString("X4");
            unpack += "\n                          " + GetByte().ToString("X2")+"-->";
            unpack += "\n        <RaiseLowerConfig>" + GetByte().ToString("X2")+"</RaiseLowerConfig>";
            unpack += "\n          <EmergencyLevel>" + GetUint16().ToString("X4")+"</EmergencyLevel>";
            unpack += "\n    <AccessoryControlType>" + GetByte().ToString("X2")+"</AccessoryControlType>";
            unpack += "\n         <PhotosensorGain>" + GetUint32().ToString("X8")+"</PhotosensorGain>";
            unpack += "\n             <TargetLevel>" + GetUint16().ToString("X4")+"</TargetLevel>";
            unpack += "\n          <DualStageLevel>" + GetUint16().ToString("X4")+"</DualStageLevel>";
            unpack += "\n       <DaylightingLowEnd>" + GetUint16().ToString("X4")+"</DaylightingLowEnd>";
            unpack += "\n     <DaylightingFadeTime>" + GetByte().ToString("X2")+"</DaylightingFadeTime>";
            unpack += "\n   <DaylightingDelaytoOff>" + GetByte().ToString("X2")+"</DaylightingDelaytoOff>";
            unpack += "\n          <PhotoSensorID1>" + GetComponentAddress()+"</PhotoSensorID1>";
            unpack += "\n          <PhotoSensorID2>" + GetComponentAddress()+"</PhotoSensorID2>";
            unpack += "\n   <CurrentLoadshedAmount>" + GetByte().ToString()+"</CurrentLoadshedAmount>";
            unpack += "\n  <SwitchedDaylighting><On>" + GetByte().ToString("X2") + "</On><Off>" + GetByte().ToString("X2") + "</Off></SwitchedDaylighting>";
            return unpack;
        }

        public string UnpackIRSensor()
        {
            string unpack = string.Empty;
            unpack += "\n       <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n         <DatabaseObjectID>"+ GetUint32().ToString("X8")+"</DatabaseObjectID>";
            unpack += "\n              <SensorType>" + GetByte().ToString("X2")+"</SensorType>";
            return unpack;
        }

        public string UnpackHHDIRSensor()
        {
            string unpack = string.Empty;
            unpack += "\n      <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n      <ConnectedSerialNum>" + GetUint32().ToString("X8")+"</ConnectedSerialNum>";
            return unpack;
        }


        public string UnpackPhotoSensor()
        {
            string unpack = string.Empty;
            unpack += "\n       <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n         <DatabaseObjectID>"+ GetUint32().ToString("X8")+"</DatabaseObjectID>";
            unpack += "\n              <SensorType>" + GetByte().ToString("X2")+"</SensorType>";
            unpack += "\n    <FixtureLightFeedback>" + GetByte().ToString()+"</FixtureLightFeedback>";
            unpack += "\n          <ReadingScaling>" + GetByte().ToString()+"</ReadingScaling>";
            unpack += "\n           <ReportingRate>" + GetByte().ToString()+"</ReportingRate>";
            return unpack;
        }

        public string UnpackHHDPhotoSensor()
        {
            string unpack = string.Empty;
            unpack += "\n       <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n    <FixtureLightFeedback>" + GetByte().ToString()+"</FixtureLightFeedback>";
            unpack += "\n          <ReadingScaling>" + GetByte().ToString()+"</ReadingScaling>";
            unpack += "\n           <ReportingRate>" + GetByte().ToString()+"</ReportingRate>";
            unpack += "\n      <ConnectedSerialNum>" + GetUint32().ToString("X8")+"</ConnectedSerialNum>";
            unpack += "\n                    <Used>" + GetByte().ToString("X2")+"</Used>";
            return unpack;
        }

        public string UnpackOccSensor()
        {
            string unpack = string.Empty;
            unpack += "\n       <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n             <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n         <DatabaseObjectID>" + GetUint32().ToString("X8")+"</DatabaseObjectID>";
            unpack += "\n               <SensorType>" + GetByte().ToString("X2")+"</SensorType>";
            unpack += "\n                  <Timeout>" + GetByte().ToString()+"</Timeout>";
            return unpack;
        }

        public string UnpackHHDOccSensor()
        {
            string unpack = string.Empty;
            unpack += "\n      <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n                  <AreaID>" + GetUint32().ToString("X8")+"</AreaID>";
            unpack += "\n      <ConnectedSerialNum>" + GetUint32().ToString("X8")+"</ConnectedSerialNum>";
            unpack += "\n                 <AreaID2>" + GetUint32().ToString("X8")+"</AreaID2>";
            unpack += "\n                 <AreaID3>" + GetUint32().ToString("X8")+"</AreaID3>";
            unpack += "\n                 <AreaID4>" + GetUint32().ToString("X8")+"</AreaID4>";
            return unpack;
        }

        public string UnpackHHDCCIComponent()
        {
            string unpack = string.Empty;
            unpack += "\n      <DatabaseObjectType>" + GetDatabaseObjectType()+"</DatabaseObjectType>";
            unpack += "\n         <DatabaseObjectID>"+ GetUint32().ToString("X8")+"</DatabaseObjectID>";
            unpack += "\n            <ComponentNum>" + GetUint16().ToString()+"</ComponentNum>";
            unpack += "\n      <ConnectedSerialNum>" + GetUint32().ToString("X8")+"</ConnectedSerialNum>";
            unpack += "\n              <InputType>";
            byte InputType = GetByte();
            switch (InputType)
            {
                case 0:
                    unpack += "Not Programmed";
                    break;

                case 1:
                    unpack += "Afterhours";
                    break;

                case 2:
                    unpack += "Loadshed";
                    break;

                case 3:
                    unpack += "Scene Select";
                    break;

                case 4:
                    unpack += "Zone Level";
                    break;

                default:
                    unpack += "UNKNOWN";
                    break;
            }
            unpack += "\n              </InputType>";
            unpack += "\n            <NormalState>";
            byte NormalState = GetByte();
            if (NormalState == 0)
            {
                unpack += "Closed";
            }
            else if (NormalState == 1)
            {
                unpack += "Open";
            }
            else
            {
                unpack += "Unknown";
            }
            unpack += "\n            </NormalState>";
            if (InputType == 2)
            {   // loadshed
                unpack += "\n                <Loadshed>" + GetByte().ToString() + "</Loadshed>";
                unpack += "\n                          " + GetByte().ToString("X2") + GetByte().ToString("X2");
            }
            else if (InputType == 3)
            {   // scene select
                byte action = GetByte();
                unpack += "\n            <SceneSelect>";
                if (action == 0)
                {
                    unpack += "Single";
                }
                else if (action == 1)
                {
                    unpack += "Toggle";
                }
                else
                {
                    unpack += "Unknown";
                }
                unpack += "\n            </SceneSelect>";
                unpack += "\n            <SceneNumber>" + GetByte().ToString() + "</SceneNumber>";
                unpack += "\n                <ContactClosureSetting>";
                if (action == 0)
                {
                    unpack += "Momentary";
                }
                else if (action == 1)
                {
                    unpack += "Maintained";
                }
                else
                {
                    unpack += "Unknown";
                }
                unpack += "\n                </ContactClosureSetting>";
            }
            else
            {
                unpack += "\n " + GetByte().ToString("X2") + GetByte().ToString("X2") + GetByte().ToString("X2") + "</ParentID>";
            }
            unpack += "\n              </InputType>";
            return unpack;
        }

        public string UnpackZone()
        {
            string unpack = string.Empty;
            unpack += "\n                  <ZoneID>" + GetUint32().ToString("X8")+ "</ZoneID>";
            unpack += "\n                  <AreaID>" + GetUint32().ToString("X8") + "</AreaID>";
            unpack += "\n                <ZoneName>" + GetUniString(46) + "</ZoneName>";
            unpack += "\n                 <SlotNum>" + GetByte().ToString() + "</SlotNum>";
            unpack += "\n                 <LoopNum>" + GetByte().ToString() + "</LoopNum>";
            unpack += "\n        <RaiseLowerconfig>" + GetByte().ToString("X2") + "</RaiseLowerconfig>";
            return unpack;
        }

        public string UnpackArea()
        {
            string unpack = string.Empty;
            unpack += "\n                  <AreaID>" + GetUint32().ToString("X8") + "</AreaID>";
            unpack += "\n                <AreaName>" + GetUniString(46) + "</AreaName>";
            unpack += "\n                 <SlotNum>" + GetByte().ToString() + "</SlotNum>";
            unpack += "\n              <OccGroupID>" + GetUint32().ToString("X8") + "</OccGroupID>";
            return unpack;
        }

        public string UnpackDaylightRegion()
        {
            string unpack = string.Empty;
            unpack += "\n        <DaylightRegionID>" + GetUint32().ToString("X8") + "</DaylightRegionID>";
            unpack += "\n                  <AreaID>" + GetUint32().ToString("X8") + "</AreaID>";
            unpack += "\n          <TargetSetPoint>" + GetUint16().ToString("X4") + "</TargetSetPoint>";
            unpack += "\n       <DesiredLightLevel>" + GetUint16().ToString("X4") + "</DesiredLightLevel>";
            unpack += "\n                   <lFull>" + GetUint16().ToString("X4") + "</lFull>";
            unpack += "\n       <DaylightinglowEnd>" + GetUint16().ToString("X8") + "</DaylightinglowEnd>";
            unpack += "\n     <AffectofDaylighting>" + GetUint16().ToString("X8") + "</AffectofDaylighting>";
            unpack += "\n   <PhotoSensor1Component>" + GetUint16().ToString("X4") + "</PhotoSensor1Component>";
            unpack += "\n      <PhotoSensor1Serial>" + GetUint32().ToString("X8") + "</PhotoSensor1Serial>";
            unpack += "\n   <PhotoSensor2Component>" + GetUint16().ToString("X4") + "</PhotoSensor2Component>";
            unpack += "\n      <PhotoSensor2Serial>" + GetUint32().ToString("X8") + "</PhotoSensor2Serial>";
            return unpack;
        }

        public string UnpackGainGroup()
        {
            string unpack = string.Empty;
            unpack += "\n             <GainGroupID>" + GetUint32().ToString("X8") + "</GainGroupID>";
            unpack += "\n                  <AreaID>" + GetUint32().ToString("X8") + "</AreaID>";
            unpack += "\n                    <Gain>" + GetUint32().ToString("X8") + "</Gain>";
            unpack += "\n                <Leveloff>" + GetUint16().ToString("X4") + "</Leveloff>";
            unpack += "\n                 <Levelon>" + GetUint16().ToString("X4") + "</Levelon>";
            unpack += "\n                    <Slot>" + GetByte().ToString() + "</Slot>";
            return unpack;
        }

        public string UnpackScene()
        {
            string unpack = string.Empty;
            unpack += "\n                 <SceneID>" + GetUint32().ToString("X8") + "</SceneID>";
            unpack += "\n                  <AreaID>" + GetUint32().ToString("X8") + "</AreaID>";
            unpack += "\n             <SceneNumber>" + GetByte().ToString() + "</SceneNumber>";
            //unpack += "\n             <SceneName>" + GetString(46) + "</SceneName>"; This Line breaks shit
            unpack += "\n               <SceneName>" + GetUniString(46) + "</SceneName>";
            if (GetByte() != 0)
            {
                unpack += "\n           <Daylighting>Enabled</Daylighting>";
            }
            else
            {
                unpack += "\n          <Daylighting>NotEnabled</Daylighting>";
            }
            return unpack;
        }

        public string UnpackRemotePM()
        {
            string unpack = string.Empty;
            unpack += "\n<RemotePMDeviceGroupNum>" + GetUint32().ToString("X8") + "</RemotePMDeviceGroupNum>";
            unpack += "\n     <ShortComponentNum>" + GetByte().ToString("X2") + "</ShortComponentNum>";
            unpack += "\n     <AffectedBitmaps>";
            unpack += "\n               <Bitmap1>" + GetUint32().ToString("X8") + "</Bitmap1>";
            unpack += "\n               <Bitmap2>" + GetUint32().ToString("X8") + "</Bitmap2>";
            unpack += "\n               <Bitmap3>" + GetUint32().ToString("X8") + "</Bitmap3>";
            unpack += "\n               <Bitmap4>" + GetUint32().ToString("X8") + "</Bitmap4>";
            unpack += "\n     </AffectedBitmaps>";
            unpack += "\n  <EXTRA_FROM_BAD_MACRO>" + GetUint32().ToString("X8") + "</EXTRA_FROM_BAD_MACRO>";
            unpack += "\n          <PresetAction>" + GetByte().ToString("X2") + "</PresetAction>";
            unpack += "\n            <ColumnType>" + GetByte().ToString("X2") + "</ColumnType>";
            unpack += "\n    <AreaLoadshedAmounts>";
            for (int idx = 0;idx < 128;idx++)
            {
                if (0 == (idx % 16))
                {
                    unpack += "\n          Areas " + (idx + 1).ToString("D3") + " - " + (idx + 16).ToString("D3") + ": |";
                }
                unpack += GetByte().ToString("D3") + "|";
            }

            unpack += "\n    </AreaLoadshedAmounts>";
            return unpack;
        }

        public string UnpackOccGroups()
        {
            string unpack = string.Empty;
            int type;
            unpack += "\n <OCCGroupID>" + GetUint32().ToString("X8") + "</OCCGroupID>";
            unpack += "\n <OccupancyType>";
            type = GetByte();
            switch (type)
            {
                case 0:
                    unpack += "\n Occupancy is diabled";
                    break;
                case 1:
                    unpack += "Occupancy";
                    break;
                case 2:
                    unpack += "Vacancy";
                    break;
                case 3:
                    unpack += "Afterhours Enabled";
                    break;
                case 4:
                    unpack += "Afterhours follow";
                    break;
                default:
                    unpack += "Unknown 0x" + type.ToString("X2");
                    break;
            }
            unpack += "\n </OccupancyType>";
            unpack += "\n     <Occupied_Scene_num>" + GetByte().ToString("X2") + "</Occupied_Scene_num>";
            unpack += "\n  <Un-Occupied_Scene_num>" + GetByte().ToString("X2") + "</Un-Occupied_Scene_num>";
            unpack += "\n <Afterhours_Timeout_num>" + GetByte().ToString("X2") + "</Afterhours_Timeout_num>";
            unpack += "\n <Blink-warn_timeout_num>" + GetByte().ToString("X2") + "</Blink-warn_timeout_num>";
            unpack += "\n <Afterhours_source_device_type>";
            type = GetByte();
            switch (type)
            {
                case 0:
                    unpack += "GRAFIK EYE";
                    break;
                case 5:
                    unpack += "CONTROL INTERFACE";
                    break;
                case 17:
                    unpack += "KEYSWITCH";
                    break;
                default:
                    unpack += "Other 0x" + type.ToString("X2");
                    break;
            }
            unpack += "\n </Afterhours_source_device_type>";
            unpack += "\n <Afterhours_source_serialNumber_num>" + GetUint32().ToString("X8") + "</Afterhours_source_serialNumber_num>";
            unpack += "\n <Sensor_timeout_num>" + GetUint32().ToString("X8") + "</Sensor_timeout_num>";
            unpack += "\n <OCC_Group_Depends_on_Number>" + GetByte().ToString("X2") + "</OCC_Group_Depends_on_Number>";
            unpack += "\n <OCC_group_Dependency_ID_num>";
            for (int idx = 0; idx < 16; idx++)
            {
                unpack += GetUint32().ToString("X8") + "|";
            }

            unpack += "\n </OCC_group_Dependency_ID_num>";
            return unpack;
            
        }
       
        public string UnpackPresetAssignments()
        {
            string unpack = string.Empty;
            unpack += "\n  <PresetAssignmentsID>" + GetUint32().ToString("X8") + "</PresetAssignmentsID>";
            unpack += "\n             <AreaID>" + GetUint32().ToString("X8") + "</AreaID>";
            unpack += "\n       <AssignedZoneID>" + GetUint32().ToString("X8") + "</AssignedZoneID>";
            unpack += "\n    <AssignmentCmdType>" + GetByte().ToString() + "</AssignmentCmdType>";
            unpack += "\n               <Param1>" + GetUint16().ToString("X4") + "</Param1>";
            unpack += "\n               <Param2>" + GetUint16().ToString("X4") + "</Param2>";
            unpack += "\n               <Param3>" + GetUint16().ToString("X4") + "</Param3>";
            return unpack;
        }

        public string UnpackSMSCEEProm()
        {
            string unpack = string.Empty;
            UInt16 value;
            unpack += "\n<!--SMSC EEPROM-->\n";
            unpack += "\n<IPAddress>" + GetByte().ToString() + "." + GetByte().ToString() + "." + GetByte().ToString() + "." + GetByte().ToString() + "</IPAddress>";
            unpack += "\n<Subnet>" + GetByte().ToString() + "." + GetByte().ToString() + "." + GetByte().ToString() + "." + GetByte().ToString() + "</Subnet>";
            unpack += "\n<Gateway>" + GetByte().ToString() + "." + GetByte().ToString() + "." + GetByte().ToString() + "." + GetByte().ToString() + "</Gateway>";
            value = GetUint16();
            unpack += "\n<IPSetKey>" + value.ToString("X4") + "</IPSetKey>";
            if (0 != (value & 0x0001))
                unpack += "\n       PING DISABLED";
            if (0 != (value & 0x0002))
                unpack += "\n       DHCP ENABLED";
            if (0 != (value & 0x0004))
                unpack += "\n       PROC NAME DISABLED";
            if (0 != (value & 0x0008))
                unpack += "\n       UNIQUE NAME DISABLED";
            if (0 != (value & 0x0010))
                unpack += "\n       USER DEFINED NAME DISABLED";
            if (0 != (value & 0x0020))
                unpack += "\n       100 MBIT DISABLED";
            if (0 != (value & 0x0040))
                unpack += "\n       10 MBIT DISABLED";
            if (0 != (value & 0x0080))
                unpack += "\n       HALF DUPLEX DISABLED";
            if (0 != (value & 0x0100))
                unpack += "\n       FULL DUPLEX DISABLED";
            if (0 != (value & 0x0200))
                unpack += "\n       UNKNOWN FLAG 0x0200"; 
            if (0 != (value & 0x0400))
                unpack += "\n       UNKNOWN FLAG 0x0400"; 
            if (0 != (value & 0x0800))
                unpack += "\n       UNKNOWN FLAG 0x0800"; 
            if (0 != (value & 0x1000))
                unpack += "\n       UNKNOWN FLAG 0x1000"; 
            if (0 != (value & 0x2000))
                unpack += "\n       UNKNOWN FLAG 0x2000"; 
            if (0 != (value & 0x4000))
                unpack += "\n       UNKNOWN FLAG 0x4000"; 
            if (0 != (value & 0x8000))
                unpack += "\n       UNKNOWN FLAG 0x8000";
            unpack += "\n<TelnetPort>" + GetUint16().ToString() + "</TelnetPort>";
            unpack += "\n<FTPPort>" + GetUint16().ToString() + "</FTPPort>";
            unpack += "\n<Flags>" + GetUint16().ToString() + "</Flags>";
            unpack += "\n<ProcName>" + GetString(16) + "</ProcName>";
            unpack += "\n<HTTPPort>" + GetUint16().ToString() + "</HTTPPort>";
            unpack += "\n<Reserved>" + GetUint16s(13) + "</Reserved>";
            unpack += "\n<MACAddress>" + GetByte().ToString("X2") + " " + GetByte().ToString("X2") + " " + GetByte().ToString("X2") + " " + GetByte().ToString("X2") + " " + GetByte().ToString("X2") + " " + GetByte().ToString("X2") + "</MACAddress>";
            unpack += "\n<Reserved>" + GetUint16s(29) + "</Reserved>";
            return unpack;            
        }
        public string UnpackPROG_CCI_COMPONENT_HHD_STRUCT()
        {
            string unpack = string.Empty;
            unpack += "\n    <DatabaseObjectType>" + GetDatabaseObjectType() + "</DatabaseObjectType>";
            unpack += "\n      <DatabaseObjectID>" + GetUint32().ToString("X8") + "</DatabaseObjectID>";
            unpack += "\n             <ComponentNum>" + GetUint16().ToString() + "</ComponentNum>";
            unpack += "\n      <ConnectedSerial>" + GetUint32().ToString("X8") + "</ConnectedSerial>";
            unpack += "\n              <InputType>";
            byte ccityp = GetByte();
            switch (ccityp)
            {
                case 0:
                    unpack += "Not Programmed";
                    break;

                case 1:
                    unpack += "After Hours";
                    break;

                case 2:
                    unpack += "Load Shed";
                    break;

                case 3:
                    unpack += "Scene select";
                    break;
                default:
                    unpack += "UNKNOWN";
                    break;
            }
            unpack += "</InputType>";
            unpack += "\n            <NormalState>";
            byte ccins = GetByte();
            switch (ccins)
            {
                case 0:
                    unpack += "Closed";
                    break;

                case 1:
                    unpack += "Open";
                    break;

                default:
                    unpack += "UNKNOWN";
                    break;
            }
            unpack += "</NormalState>";
            if (ccityp == 2)
            {
                unpack += "\n           <percent_of_Loadshed>" + GetByte().ToString() + "</percent_of_Loadshed>";
                SkipBytes(2);
            }
            else if (ccityp == 3)
            {
                unpack += "\n         <CCIActiontype>";
                byte cciact = GetByte();
                switch (cciact)
                {
                    case 0:
                        unpack += "Single";
                        break;

                    case 1:
                        unpack += "Toggle";
                        break;

                    case 2:
                        unpack += "UNKNOWN";
                        break;
                }
                unpack += "\n         </CCIActiontype>";
                unpack += "\n         <CCIsettingstype>";
                unpack += "\n               <SceneNum>" + GetByte().ToString();
                byte cciset = GetByte();
                switch (cciact)
                {
                    case 0:
                        unpack += "Momentary";
                        break;

                    case 1:
                        unpack += "Maintained";
                        break;

                    case 2:
                        unpack += "UNKNOWN";
                        break;
                }

                unpack += "\n       </CCIsettingstype>" + GetByte().ToString() + "</SceneNum>";
            }
            else
            {
                SkipBytes(3);
            }
            return unpack;
        }
    }
}

/*
typedef packed struct
{
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
} FLASH_DATABASE_LAYOUT;
*/