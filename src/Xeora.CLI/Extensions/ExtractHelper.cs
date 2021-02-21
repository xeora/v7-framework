using System;
using System.IO;
using Xeora.CLI.Extensions.Build;

namespace Xeora.CLI.Extensions
{
    public class ExtractHelper
    {
        public void Extract(string xeoraDomainFile, byte[] password, bool listContent, string output)
        {
            Extractor extractor = 
                new Extractor(xeoraDomainFile, password);

            if (listContent)
            {
                extractor.QueryList(xeoraFileInfo =>
                {
                    Console.Write("   ");
                    Console.Write("reading");
                    Console.Write(" ");
                    Console.WriteLine(string.Concat(xeoraFileInfo.RegistrationPath, xeoraFileInfo.FileName));

                    return true;
                });

                return;
            }
            
            extractor.QueryList(xeoraFileInfo =>
            {
                Console.Write("   ");
                Console.Write("extracting");
                Console.Write(" ");
                Console.Write(string.Concat(xeoraFileInfo.RegistrationPath, xeoraFileInfo.FileName));
                Console.Write(" ");

                Stream outputStream = null;
                try
                {
                    string extractLocation =
                        xeoraFileInfo.RegistrationPath.Replace('\\', Path.DirectorySeparatorChar);
                    extractLocation = extractLocation.Substring(1);
                    extractLocation = Path.Combine(output, extractLocation);

                    if (!Directory.Exists(extractLocation))
                        Directory.CreateDirectory(extractLocation);
                    
                    outputStream = 
                        new FileStream(
                            Path.Combine(extractLocation, xeoraFileInfo.FileName), 
                            FileMode.Create, 
                            FileAccess.ReadWrite, 
                            FileShare.None
                        );
                    extractor.Read(xeoraFileInfo.Index, xeoraFileInfo.CompressedLength, ref outputStream);

                    Console.WriteLine("done!");

                    return true;
                }
                catch (InvalidDataException)
                {
                    Console.WriteLine("Failed! Password is incorrect");

                    return false;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed! {e.Message}");
                    
                    return false;
                }
                finally
                {
                    outputStream?.Close();
                }
            });
        }
    }
}
