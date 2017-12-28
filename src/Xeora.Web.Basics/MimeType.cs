using System;
using System.Collections.Generic;

namespace Xeora.Web.Basics
{
    public class MimeType
    {
        private enum MimeLookups
        {
            Type,
            Extention
        }

        private static Dictionary<string, string> _ExtensionMap = null;
        private static Dictionary<string, string> MimeMapping
        {
            get
            {
                if (MimeType._ExtensionMap == null)
                {
                    MimeType._ExtensionMap = new Dictionary<string, string>();
                    MimeType._ExtensionMap.Add(".323", "text/h323");
                    MimeType._ExtensionMap.Add(".asx", "video/x-ms-asf");
                    MimeType._ExtensionMap.Add(".acx", "application/internet-property-stream");
                    MimeType._ExtensionMap.Add(".ai", "application/postscript");
                    MimeType._ExtensionMap.Add(".aif", "audio/x-aiff");
                    MimeType._ExtensionMap.Add(".aiff", "audio/aiff");
                    MimeType._ExtensionMap.Add(".axs", "application/olescript");
                    MimeType._ExtensionMap.Add(".aifc", "audio/aiff");
                    MimeType._ExtensionMap.Add(".asr", "video/x-ms-asf");
                    MimeType._ExtensionMap.Add(".avi", "video/x-msvideo");
                    MimeType._ExtensionMap.Add(".asf", "video/x-ms-asf");
                    MimeType._ExtensionMap.Add(".au", "audio/basic");
                    MimeType._ExtensionMap.Add(".application", "application/x-ms-application");
                    MimeType._ExtensionMap.Add(".bin", "application/octet-stream");
                    MimeType._ExtensionMap.Add(".bas", "text/plain");
                    MimeType._ExtensionMap.Add(".bcpio", "application/x-bcpio");
                    MimeType._ExtensionMap.Add(".bmp", "image/bmp");
                    MimeType._ExtensionMap.Add(".cdf", "application/x-cdf");
                    MimeType._ExtensionMap.Add(".cat", "application/vndms-pkiseccat");
                    MimeType._ExtensionMap.Add(".crt", "application/x-x509-ca-cert");
                    MimeType._ExtensionMap.Add(".c", "text/plain");
                    MimeType._ExtensionMap.Add(".css", "text/css");
                    MimeType._ExtensionMap.Add(".cer", "application/x-x509-ca-cert");
                    MimeType._ExtensionMap.Add(".crl", "application/pkix-crl");
                    MimeType._ExtensionMap.Add(".cmx", "image/x-cmx");
                    MimeType._ExtensionMap.Add(".csh", "application/x-csh");
                    MimeType._ExtensionMap.Add(".cod", "image/cis-cod");
                    MimeType._ExtensionMap.Add(".cpio", "application/x-cpio");
                    MimeType._ExtensionMap.Add(".clp", "application/x-msclip");
                    MimeType._ExtensionMap.Add(".crd", "application/x-mscardfile");
                    MimeType._ExtensionMap.Add(".deploy", "application/octet-stream");
                    MimeType._ExtensionMap.Add(".dll", "application/x-msdownload");
                    MimeType._ExtensionMap.Add(".dot", "application/msword");
                    MimeType._ExtensionMap.Add(".doc", "application/msword");
                    MimeType._ExtensionMap.Add(".dvi", "application/x-dvi");
                    MimeType._ExtensionMap.Add(".dir", "application/x-director");
                    MimeType._ExtensionMap.Add(".dxr", "application/x-director");
                    MimeType._ExtensionMap.Add(".der", "application/x-x509-ca-cert");
                    MimeType._ExtensionMap.Add(".dib", "image/bmp");
                    MimeType._ExtensionMap.Add(".dcr", "application/x-director");
                    MimeType._ExtensionMap.Add(".disco", "text/xml");
                    MimeType._ExtensionMap.Add(".exe", "application/octet-stream");
                    MimeType._ExtensionMap.Add(".etx", "text/x-setext");
                    MimeType._ExtensionMap.Add(".evy", "application/envoy");
                    MimeType._ExtensionMap.Add(".eml", "message/rfc822");
                    MimeType._ExtensionMap.Add(".eps", "application/postscript");
                    MimeType._ExtensionMap.Add(".flr", "x-world/x-vrml");
                    MimeType._ExtensionMap.Add(".fif", "application/fractals");
                    MimeType._ExtensionMap.Add(".gtar", "application/x-gtar");
                    MimeType._ExtensionMap.Add(".gif", "image/gif");
                    MimeType._ExtensionMap.Add(".gz", "application/x-gzip");
                    MimeType._ExtensionMap.Add(".hta", "application/hta");
                    MimeType._ExtensionMap.Add(".htc", "text/x-component");
                    MimeType._ExtensionMap.Add(".htt", "text/webviewhtml");
                    MimeType._ExtensionMap.Add(".h", "text/plain");
                    MimeType._ExtensionMap.Add(".hdf", "application/x-hdf");
                    MimeType._ExtensionMap.Add(".hlp", "application/winhlp");
                    MimeType._ExtensionMap.Add(".html", "text/html");
                    MimeType._ExtensionMap.Add(".htm", "text/html");
                    MimeType._ExtensionMap.Add(".hqx", "application/mac-binhex40");
                    MimeType._ExtensionMap.Add(".isp", "application/x-internet-signup");
                    MimeType._ExtensionMap.Add(".iii", "application/x-iphone");
                    MimeType._ExtensionMap.Add(".ief", "image/ief");
                    MimeType._ExtensionMap.Add(".ivf", "video/x-ivf");
                    MimeType._ExtensionMap.Add(".ins", "application/x-internet-signup");
                    MimeType._ExtensionMap.Add(".ico", "image/x-icon");
                    MimeType._ExtensionMap.Add(".jpg", "image/jpeg");
                    MimeType._ExtensionMap.Add(".jfif", "image/pjpeg");
                    MimeType._ExtensionMap.Add(".jpe", "image/jpeg");
                    MimeType._ExtensionMap.Add(".jpeg", "image/jpeg");
                    MimeType._ExtensionMap.Add(".js", "application/x-javascript");
                    MimeType._ExtensionMap.Add(".lsx", "video/x-la-asf");
                    MimeType._ExtensionMap.Add(".latex", "application/x-latex");
                    MimeType._ExtensionMap.Add(".lsf", "video/x-la-asf");
                    MimeType._ExtensionMap.Add(".manifest", "application/x-ms-manifest");
                    MimeType._ExtensionMap.Add(".mhtml", "message/rfc822");
                    MimeType._ExtensionMap.Add(".mny", "application/x-msmoney");
                    MimeType._ExtensionMap.Add(".mht", "message/rfc822");
                    MimeType._ExtensionMap.Add(".mid", "audio/mid");
                    MimeType._ExtensionMap.Add(".mpv2", "video/mpeg");
                    MimeType._ExtensionMap.Add(".man", "application/x-troff-man");
                    MimeType._ExtensionMap.Add(".mvb", "application/x-msmediaview");
                    MimeType._ExtensionMap.Add(".mpeg", "video/mpeg");
                    MimeType._ExtensionMap.Add(".m3u", "audio/x-mpegurl");
                    MimeType._ExtensionMap.Add(".mdb", "application/x-msaccess");
                    MimeType._ExtensionMap.Add(".mpp", "application/vnd.ms-project");
                    MimeType._ExtensionMap.Add(".m1v", "video/mpeg");
                    MimeType._ExtensionMap.Add(".mpa", "video/mpeg");
                    MimeType._ExtensionMap.Add(".me", "application/x-troff-me");
                    MimeType._ExtensionMap.Add(".m13", "application/x-msmediaview");
                    MimeType._ExtensionMap.Add(".movie", "video/x-sgi-movie");
                    MimeType._ExtensionMap.Add(".m14", "application/x-msmediaview");
                    MimeType._ExtensionMap.Add(".mpe", "video/mpeg");
                    MimeType._ExtensionMap.Add(".mp2", "video/mpeg");
                    MimeType._ExtensionMap.Add(".mov", "video/quicktime");
                    MimeType._ExtensionMap.Add(".mp3", "audio/mpeg");
                    MimeType._ExtensionMap.Add(".mpg", "video/mpeg");
                    MimeType._ExtensionMap.Add(".ms", "application/x-troff-ms");
                    MimeType._ExtensionMap.Add(".nc", "application/x-netcdf");
                    MimeType._ExtensionMap.Add(".nws", "message/rfc822");
                    MimeType._ExtensionMap.Add(".oda", "application/oda");
                    MimeType._ExtensionMap.Add(".ods", "application/oleobject");
                    MimeType._ExtensionMap.Add(".pmc", "application/x-perfmon");
                    MimeType._ExtensionMap.Add(".p7r", "application/x-pkcs7-certreqresp");
                    MimeType._ExtensionMap.Add(".p7b", "application/x-pkcs7-certificates");
                    MimeType._ExtensionMap.Add(".p7s", "application/pkcs7-signature");
                    MimeType._ExtensionMap.Add(".pmw", "application/x-perfmon");
                    MimeType._ExtensionMap.Add(".ps", "application/postscript");
                    MimeType._ExtensionMap.Add(".p7c", "application/pkcs7-mime");
                    MimeType._ExtensionMap.Add(".pbm", "image/x-portable-bitmap");
                    MimeType._ExtensionMap.Add(".ppm", "image/x-portable-pixmap");
                    MimeType._ExtensionMap.Add(".pub", "application/x-mspublisher");
                    MimeType._ExtensionMap.Add(".png", "image/png");
                    MimeType._ExtensionMap.Add(".pnm", "image/x-portable-anymap");
                    MimeType._ExtensionMap.Add(".pml", "application/x-perfmon");
                    MimeType._ExtensionMap.Add(".p10", "application/pkcs10");
                    MimeType._ExtensionMap.Add(".pfx", "application/x-pkcs12");
                    MimeType._ExtensionMap.Add(".p12", "application/x-pkcs12");
                    MimeType._ExtensionMap.Add(".pdf", "application/pdf");
                    MimeType._ExtensionMap.Add(".pps", "application/vnd.ms-powerpoint");
                    MimeType._ExtensionMap.Add(".p7m", "application/pkcs7-mime");
                    MimeType._ExtensionMap.Add(".pko", "application/vndms-pkipko");
                    MimeType._ExtensionMap.Add(".ppt", "application/vnd.ms-powerpoint");
                    MimeType._ExtensionMap.Add(".pmr", "application/x-perfmon");
                    MimeType._ExtensionMap.Add(".pma", "application/x-perfmon");
                    MimeType._ExtensionMap.Add(".pot", "application/vnd.ms-powerpoint");
                    MimeType._ExtensionMap.Add(".prf", "application/pics-rules");
                    MimeType._ExtensionMap.Add(".pgm", "image/x-portable-graymap");
                    MimeType._ExtensionMap.Add(".qt", "video/quicktime");
                    MimeType._ExtensionMap.Add(".ra", "audio/x-pn-realaudio");
                    MimeType._ExtensionMap.Add(".rgb", "image/x-rgb");
                    MimeType._ExtensionMap.Add(".ram", "audio/x-pn-realaudio");
                    MimeType._ExtensionMap.Add(".rmi", "audio/mid");
                    MimeType._ExtensionMap.Add(".ras", "image/x-cmu-raster");
                    MimeType._ExtensionMap.Add(".roff", "application/x-troff");
                    MimeType._ExtensionMap.Add(".rtf", "application/rtf");
                    MimeType._ExtensionMap.Add(".rtx", "text/richtext");
                    MimeType._ExtensionMap.Add(".sv4crc", "application/x-sv4crc");
                    MimeType._ExtensionMap.Add(".spc", "application/x-pkcs7-certificates");
                    MimeType._ExtensionMap.Add(".setreg", "application/set-registration-initiation");
                    MimeType._ExtensionMap.Add(".snd", "audio/basic");
                    MimeType._ExtensionMap.Add(".stl", "application/vndms-pkistl");
                    MimeType._ExtensionMap.Add(".setpay", "application/set-payment-initiation");
                    MimeType._ExtensionMap.Add(".stm", "text/html");
                    MimeType._ExtensionMap.Add(".shar", "application/x-shar");
                    MimeType._ExtensionMap.Add(".sh", "application/x-sh");
                    MimeType._ExtensionMap.Add(".sit", "application/x-stuffit");
                    MimeType._ExtensionMap.Add(".spl", "application/futuresplash");
                    MimeType._ExtensionMap.Add(".sct", "text/scriptlet");
                    MimeType._ExtensionMap.Add(".scd", "application/x-msschedule");
                    MimeType._ExtensionMap.Add(".sst", "application/vndms-pkicertstore");
                    MimeType._ExtensionMap.Add(".src", "application/x-wais-source");
                    MimeType._ExtensionMap.Add(".sv4cpio", "application/x-sv4cpio");
                    MimeType._ExtensionMap.Add(".tex", "application/x-tex");
                    MimeType._ExtensionMap.Add(".tgz", "application/x-compressed");
                    MimeType._ExtensionMap.Add(".t", "application/x-troff");
                    MimeType._ExtensionMap.Add(".tar", "application/x-tar");
                    MimeType._ExtensionMap.Add(".tr", "application/x-troff");
                    MimeType._ExtensionMap.Add(".tif", "image/tiff");
                    MimeType._ExtensionMap.Add(".txt", "text/plain");
                    MimeType._ExtensionMap.Add(".texinfo", "application/x-texinfo");
                    MimeType._ExtensionMap.Add(".trm", "application/x-msterminal");
                    MimeType._ExtensionMap.Add(".tiff", "image/tiff");
                    MimeType._ExtensionMap.Add(".tcl", "application/x-tcl");
                    MimeType._ExtensionMap.Add(".texi", "application/x-texinfo");
                    MimeType._ExtensionMap.Add(".tsv", "text/tab-separated-values");
                    MimeType._ExtensionMap.Add(".ustar", "application/x-ustar");
                    MimeType._ExtensionMap.Add(".uls", "text/iuls");
                    MimeType._ExtensionMap.Add(".vcf", "text/x-vcard");
                    MimeType._ExtensionMap.Add(".wps", "application/vnd.ms-works");
                    MimeType._ExtensionMap.Add(".wav", "audio/wav");
                    MimeType._ExtensionMap.Add(".wrz", "x-world/x-vrml");
                    MimeType._ExtensionMap.Add(".wri", "application/x-mswrite");
                    MimeType._ExtensionMap.Add(".wks", "application/vnd.ms-works");
                    MimeType._ExtensionMap.Add(".wmf", "application/x-msmetafile");
                    MimeType._ExtensionMap.Add(".wcm", "application/vnd.ms-works");
                    MimeType._ExtensionMap.Add(".wrl", "x-world/x-vrml");
                    MimeType._ExtensionMap.Add(".wdb", "application/vnd.ms-works");
                    MimeType._ExtensionMap.Add(".wsdl", "text/xml");
                    MimeType._ExtensionMap.Add(".xml", "text/xml");
                    MimeType._ExtensionMap.Add(".xlm", "application/vnd.ms-excel");
                    MimeType._ExtensionMap.Add(".xaf", "x-world/x-vrml");
                    MimeType._ExtensionMap.Add(".xla", "application/vnd.ms-excel");
                    MimeType._ExtensionMap.Add(".xls", "application/vnd.ms-excel");
                    MimeType._ExtensionMap.Add(".xof", "x-world/x-vrml");
                    MimeType._ExtensionMap.Add(".xlt", "application/vnd.ms-excel");
                    MimeType._ExtensionMap.Add(".xlc", "application/vnd.ms-excel");
                    MimeType._ExtensionMap.Add(".xsl", "text/xml");
                    MimeType._ExtensionMap.Add(".xbm", "image/x-xbitmap");
                    MimeType._ExtensionMap.Add(".xlw", "application/vnd.ms-excel");
                    MimeType._ExtensionMap.Add(".xpm", "image/x-xpixmap");
                    MimeType._ExtensionMap.Add(".xwd", "image/x-xwindowdump");
                    MimeType._ExtensionMap.Add(".xsd", "text/xml");
                    MimeType._ExtensionMap.Add(".z", "application/x-compress");
                    MimeType._ExtensionMap.Add(".zip", "application/x-zip-compressed");

                    // Load Custom Mime Definitions
                    foreach (Configuration.IMimeItem item in Configurations.Xeora.Application.CustomMimes)
                        MimeType._ExtensionMap[item.Extension] = item.Type;
                }

                return MimeType._ExtensionMap;
            }
        }

        private static string ResolveMime(MimeLookups mimeLookup, string searchValue)
        {
            switch (mimeLookup)
            {
                case MimeLookups.Type:
                    if (string.IsNullOrEmpty(searchValue))
                        return "application/octet-stream";

                    if (MimeType.MimeMapping.ContainsKey(searchValue))
                        return MimeType.MimeMapping[searchValue];

                    return "application/octet-stream";
                case MimeLookups.Extention:
                    if (string.IsNullOrEmpty(searchValue))
                        return ".dat";

                    if (MimeType.MimeMapping.ContainsValue(searchValue))
                    {
                        foreach (KeyValuePair<string, string> item in MimeType.MimeMapping)
                        {
                            if (string.Compare(item.Value, searchValue) == 0)
                                return item.Key;
                        }
                    }

                    return ".dat";
            }

            throw new Exception("ResolveMime should never reach here!");
        }

        public static string GetMime(string fileExtension)
        {
            if (!string.IsNullOrEmpty(fileExtension) && !fileExtension.StartsWith("."))
                fileExtension = string.Format(".{0}", fileExtension);

            return MimeType.ResolveMime(MimeLookups.Type, fileExtension);
        }

        public static string GetExtension(string mimeType) =>
            MimeType.ResolveMime(MimeLookups.Extention, mimeType);
    }

}
