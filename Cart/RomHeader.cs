using System;
using Drongo.GameboyEmulator.Utils;
using UnityEngine;

namespace Drongo.GameboyEmulator
{
    public struct RomHeader
    {
        public byte[] entry;
        public byte[] logo;
        public byte[] title;
        public ushort newLicenseCode;
        public byte sgbFlag;
        public byte mbc;
        public byte romSize;
        public byte ramSize;
        public byte destCode;
        public byte licenseCode;
        public byte version;
        public byte checksum;
        public ushort globalChecksum;

        public string readableTitle;

        public void Read(ref byte[] fullRom)
        {
            int index = 256;
            
            entry = Tools.GetSubArray(ref fullRom, ref index, 4); // 4 bytes

            logo = Tools.GetSubArray(ref fullRom, ref index, 48); // 48 bytes
            title = Tools.GetSubArray(ref fullRom, ref index, 16); // 16 bytes

            newLicenseCode = (ushort)(fullRom[index++] |  (fullRom[index++] << 8));

            sgbFlag = fullRom[index++];
            mbc = fullRom[index++];
            romSize = fullRom[index++];
            ramSize = fullRom[index++];
            destCode = fullRom[index++];
            licenseCode = fullRom[index++];
            version = fullRom[index++];
            checksum = fullRom[index++];
            globalChecksum = (ushort)(fullRom[index++] | (fullRom[index++] << 8) );
           
            readableTitle = string.Copy(System.Text.Encoding.UTF8.GetString(title));
            
            // running checksum
            // from https://gbdev.io/pandocs/The_Cartridge_Header.html
            ushort checksumTest = 0;
            for (ushort address = 0x0134; address <= 0x014C; address++) {
                checksumTest = (ushort)(checksumTest - fullRom[address] - 1);
            }

            byte checksumTestByte = fullRom[0x14D];
            int checkSumBytes = checksumTest & 0xFF; 
            
            if (checkSumBytes != checksumTestByte)
                 throw new Exception("ROM Checksum failed");
            
            string debugText = $"title: {readableTitle}";
            Debug.Log(debugText);

            debugText = $"ROM mbc: {RomHeaderConstants.ROM_TYPES[mbc]}\n" +
                        $"LicCode: {RomHeaderConstants.LIC_CODE[licenseCode]}\n" +
                        $"RomSize: {romSize}\n" +
                        $"RamSize: {ramSize}\n" +
                        $"RomVersion: {version}\n" +
                        $"Rom Checksum passed: {Convert.ToString(checksumTestByte, 16)}";
            
            Debug.Log(debugText);
        }
    }
}