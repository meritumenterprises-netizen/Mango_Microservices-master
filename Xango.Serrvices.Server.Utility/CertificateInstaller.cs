using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Serrvices.Server.Utility
{
    public class CertificateInstaller
    {
        public static void AddCertificateToTrustedRoot(string pfxPath, string password)
        {
            // Load the certificate from PFX file
            var cert = new X509Certificate2(pfxPath, password, X509KeyStorageFlags.UserKeySet | X509KeyStorageFlags.PersistKeySet);

            // Open the Trusted Root store for Local Machine
            using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadWrite);

                // Check if certificate already exists
                bool exists = false;
                foreach (var existing in store.Certificates)
                {
                    if (existing.Thumbprint == cert.Thumbprint)
                    {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                {
                    store.Add(cert);
                    Console.WriteLine("✅ Certificate added to Trusted Root Certification Authorities.");
                }
                else
                {
                    Console.WriteLine("ℹ️ Certificate already exists in Trusted Root store.");
                }

                store.Close();
            }
        }
    }
}
