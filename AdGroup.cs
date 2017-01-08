using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.DirectoryServices;
using System.Security.Principal;

namespace LDAP.ActiveDirectory
{
    /// <summary>
    /// Construction for AD group
    /// </summary>
    public class AdGroup
    {
        public string Name = "";  // prefix by "*"
        public string SuperUser = "";
        public List<AdNode> Members = new List<AdNode>();

        public AdGroup(string groupName)
        {
            this.Name = groupName;
        }

        /// <summary>
        /// get emails for all member users
        /// </summary>
        /// <returns></returns>
        public List<string> GetAllMemberEmails()
        {
            List<string> result = new List<string>();
            foreach (AdNode node in this.Members)
            {
                if (!string.IsNullOrEmpty(node.Email))
                {
                    result.Add(node.Email);
                }
            }
            return result;
        }

        /// <summary>
        /// determine whether the group contains specific user or not
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool ContainsMember(string userName)
        {
            string[] arr = userName.Split('@');  // get user id from possible email address
            userName = arr[0];

            foreach (AdNode node in this.Members)
            {
                if (node.Name == userName)
                    return true;
            }

            return false;
        }

    }
}