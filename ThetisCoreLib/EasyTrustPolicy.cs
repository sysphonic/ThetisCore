/* 
 * EasyTrustPolicy.xaml.cs
 * 
 * Copyright: (c) 2007-2018, MORITA Shintaro, Sysphonic. All rights reserved.
 * License: Modified BSD License (See LICENSE file)
 * URL: http://sysphonic.com/
 */
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace ThetisCore.Lib
{
    /// <summary>Custom CertificatePolicy to trust servers.</summary>
    public class EasyTrustPolicy
    {
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Return True to force the certificate to be accepted.
            return true;
        }
    }
}
