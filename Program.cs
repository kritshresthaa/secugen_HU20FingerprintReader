using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using SecuGen.FDxSDKPro.Windows;


namespace Secugen_HU20
{
    internal class Program
    {
        private static SGFingerPrintManager m_FPM;
        private static int m_ImageWidth;
        private static int m_ImageHeight;
        static byte[] CaptureAndDisplayImage()
        {
            // buffer for image
            Byte[] fp_image = new Byte[m_ImageWidth * m_ImageHeight];
            Console.WriteLine("In capture and display method");
            // fingerprint capturing
            Int32 max_template_size = 0;
            m_FPM.GetMaxTemplateSize(ref max_template_size);
            Byte[] m_RegMin1 = new Byte[max_template_size];
            Int32 iError = m_FPM.GetImage(fp_image);

            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {

                Console.WriteLine("Capturing Image");
                Console.WriteLine(fp_image);

                m_FPM.CreateTemplate(fp_image, m_RegMin1);
                Console.WriteLine(Encoding.Default.GetString(m_RegMin1));

            }
            else
            {
                Console.WriteLine("Error: " + iError);
            }
            return m_RegMin1;

        }

        static void InitilzingDevice()
        {
            SGFPMDeviceName device_name = SGFPMDeviceName.DEV_AUTO;


            m_FPM = new SGFingerPrintManager();


            //initilalizing the device

            //Console.WriteLine(device_name);

            Int32 port_addr = (int)SGFPMPortAddr.USB_AUTO_DETECT;

            Int32 iError = m_FPM.Init(device_name);
            if (iError == (Int32)SGFPMError.ERROR_NONE)
                Console.WriteLine("Initialization Success");
            else
                Console.WriteLine("Initializaion() Error : " + iError);
            //m_FPM.OpenDevice(port_addr);

            iError = (Int32)m_FPM.OpenDevice(port_addr);
            //Console.WriteLine(iError);
            if (iError == (Int32)SGFPMError.ERROR_NONE)
                Console.WriteLine("Deviced Oppened Success");
            else
                Console.WriteLine("OpenDevice() Error : " + iError);
            //getting the device info
            SGFPMDeviceInfoParam pInfo = new SGFPMDeviceInfoParam();

            iError = m_FPM.GetDeviceInfo(pInfo);


            if (iError == (Int32)SGFPMError.ERROR_NONE)
            {
                //This should be done GetDeviceInfo();
                m_ImageWidth = pInfo.ImageWidth;
                m_ImageHeight = pInfo.ImageHeight;
            }



        }

        static void Main(string[] args)
        {
            Byte[] f1 = null; // Initialize as null or with appropriate size
            Byte[] f2 = null; // Initialize as null or with appropriate size

            InitilzingDevice(); // This function needs to be defined

            Console.WriteLine("Enter your choice");
            int ch;
            while (true)
            {
                Console.WriteLine(" 1. Read finger 1\n 2. Read finger 2\n 3. Verify\n 4. Exit");
                if (int.TryParse(Console.ReadLine(), out ch)) // Read user input as an integer
                {
                    switch (ch)
                    {
                        case 1:
                            f1 = CaptureAndDisplayImage(); // Store fingerprint image for finger 1
                            break;
                        case 2:
                            f2 = CaptureAndDisplayImage(); // Store fingerprint image for finger 2
                            break;
                        case 3:
                            if (f1 != null && f2 != null) // Check if both fingerprints are captured
                            {
                                bool matched = false;
                                SGFPMSecurityLevel secu_level = SGFPMSecurityLevel.HIGHEST;
                                m_FPM.MatchTemplate(f1, f2, secu_level, ref matched); // Verify if fingerprints match
                                Console.WriteLine(Convert.ToString(matched)); // Output the verification result
                            }
                            else
                            {
                                Console.WriteLine("Please capture both fingerprints first.");
                            }
                            break;
                        case 4:
                            return; // Exit the loop and terminate the program
                        default:
                            Console.WriteLine("Invalid Option");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Input. Please enter a number.");
                }
            }
        }






    }
}