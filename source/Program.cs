using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace SystemTrayModder
{
    class Program
    {
        private static string ProgramName = "";
        private static int Setting = -1;

        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Parameters incorrect!");
                Console.WriteLine("Correct syntax: <ProgramName> <Setting>");
                return 1;
            }
            else
            {
                ProgramName = args[0];
                Setting = Int16.Parse(args[1]);
                if (Setting < 0 || Setting > 2)
                {
                    Console.WriteLine("The setting (2 = show icon and notifications 1 = hide icon and notifications, 0 = only show notifications");
                    return 1;
                }
            }

            UTF8Encoding encText = new System.Text.UTF8Encoding();
            byte[] bytRegKey = (byte[]) Registry.GetValue("HKEY_CURRENT_USER\\Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\TrayNotify", "IconStreams", new byte[] {00});
            string strRegKey = "";

            foreach (var b in bytRegKey)
            {
                string tempString = Convert.ToString(b, 16);
                switch(tempString.Length)
                {
                    case 0:
                        strRegKey += "00";
                        break;
                    case 1:
                        strRegKey += "0" + tempString;
                        break;
                    case 2:
                        strRegKey += tempString;
                        break;
                }
            }

            byte[] bytTempAppPath = encText.GetBytes(ProgramName);
            List<byte> bytAppPath = new List<byte>();
            string strAppPath = "";

            for (int i = 0; i < bytTempAppPath.Length * 2; i++)
            {
                if (i % 2 <= 0)
                {
                    var curbyte = bytTempAppPath[(int)(i / 2)];
                    bytAppPath.Add(Rot13(curbyte));
                }
                else
                {
                    bytAppPath.Add(0);
                }
            }

            foreach (var b in bytAppPath)
            {
                string tempString = Convert.ToString(b, 16);
                switch (tempString.Length)
                {
                    case 0:
                        strAppPath += "00";
                        break;
                    case 1:
                        strAppPath += "0" + tempString;
                        break;
                    case 2:
                        strAppPath += tempString;
                        break;
                }
            }

            if (!strRegKey.Contains(strAppPath))
            {
                Console.WriteLine("Program \"" + ProgramName + "\" not found. Programs are case sensitive.");
                return 1;
            }

            List<byte> header = new List<byte>();
            Dictionary<string, byte[]> items = new Dictionary<string, byte[]>();

            for (int i = 0; i < 20; i++)
                header.Add(bytRegKey[i]);

            for (int i = 0; i < ((bytRegKey.Length - 20) / 1640); i++)
            {
                int startingByte = 20 + (i * 1640);
                byte[] item = bytRegKey.Skip(startingByte - 1).Take(1640).ToArray();
                items.Add(startingByte.ToString(), item);
            }

            foreach (var key in items.Keys)
            {
                var item = items[key];
                string strItem = "";
                string tempString = "";

                foreach (var it in item)
                {
                    tempString = Convert.ToString(it, 16);
                    switch (tempString.Length)
                    {
                        case 0:
                            strItem += "00";
                            break;
                        case 1:
                            strItem += "0" + tempString;
                            break;
                        case 2:
                            strItem += tempString;
                            break;
                    }
                }

                if (strItem.Contains(strAppPath))
                {
                    Console.WriteLine("Item Found with \"" + ProgramName + "\" in item starting with byte " + key);
                    bytRegKey[Convert.ToInt32(key) + 528] = (byte)Setting;
                    Registry.SetValue("HKEY_CURRENT_USER\\Software\\Classes\\Local Settings\\Software\\Microsoft\\Windows\\CurrentVersion\\TrayNotify", "IconStreams", bytRegKey);
                }
            }

            return 0;
        }

        static byte Rot13(byte byteToRot)
        {
            if (byteToRot > 64 && byteToRot < 91)
            {
                return (byte)((byteToRot - 64 + 13) % 26 + 64);
            }
            else if (byteToRot > 96 && byteToRot < 123)
            {
                return (byte)((byteToRot - 96 + 13) % 26 + 96);
            }
            else
            {
                return byteToRot;
            }
        }
    }
}
