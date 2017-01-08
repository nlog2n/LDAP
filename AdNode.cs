using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Security.Principal;

namespace LDAP.ActiveDirectory
{
    public enum AdType : int
    {
        /// <summary>
        /// Organization Unit
        /// </summary>
        OU = 1,

        /// <summary>
        /// User
        /// </summary>
        USER = 2,

        /// <summary>
        /// Others
        /// </summary>
        OTHER = 0
    }

    /// <summary>
    /// AD domain node, a simple mapping to DirectoryEntry
    /// </summary>
    public class AdNode
    {
        public string Id = "";
        public string Name = "";
        public AdType Type = AdType.OTHER;
        public string ParentId = "";
        public string Email = "";
        public string DistinguishedName = "";
        public string DisplayName = "";

        public AdNode(DirectoryEntry entry, string parentId)
        {
            // object GUID
            string id = string.Empty;
            if (entry.Properties.Contains("objectGUID"))  
            {
                byte[] bGUID = entry.Properties["objectGUID"][0] as byte[];
                id = BitConverter.ToString(bGUID);
            }

            // unique long name
            string distinguishedName = "";
            if (entry.Properties.Contains("distinguishedName"))
            {
                distinguishedName = entry.Properties["distinguishedName"][0].ToString();
            }



            // another way to identify OU or User
            //if (entry.Properties.Contains("ou")) {}
            switch (entry.SchemaClassName)
            {
                case "organizationalUnit":
                    string[] arr = entry.Name.Split('=');
                    string categoryStr = arr[0];
                    string nameStr = arr[1];
                    // or nameStr = entry.Properties["ou"][0].ToString();

                    {
                        this.Id = id;
                        this.Name = nameStr;  
                        this.Type = AdType.OU;
                        this.ParentId = parentId;
                        this.Email = "";
                        this.DistinguishedName = distinguishedName;
                        this.DisplayName = "";
                    }
                    break;

                case "user":
                    string accountName = "";
                    if (entry.Properties.Contains("samaccountName"))
                    {
                        accountName = entry.Properties["samaccountName"][0].ToString();
                    }

                    string email = "";
                    if (entry.Properties.Contains("mail"))
                    {
                        email = entry.Properties["mail"][0].ToString();
                    }

                    string displayName = "";
                    if (entry.Properties.Contains("displayName"))
                    {
                        displayName = entry.Properties["displayName"][0].ToString();
                    }


                    {
                        this.Id = id;
                        this.Name = accountName;
                        this.Type = AdType.USER;
                        this.ParentId = parentId;
                        this.Email = email;
                        this.DistinguishedName = distinguishedName;
                        this.DisplayName = displayName;
                    }
                    break;
            }
        }
    }
}