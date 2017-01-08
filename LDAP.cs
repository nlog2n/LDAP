using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Security.Principal;


namespace LDAP.ActiveDirectory
{
    public class AD
    {
        public string DomainName = "teak.local.net"; // "10.217.253.161";
        public string ExchangeDomainName = "Exchange"; // "company";
        public string Username = "hfang"; // "administrator";

        public DirectoryEntry Domain = null;  // login domain, root
        public DirectoryEntry ExchangeDomain = null;  // exchange domain, child


        /// <summary>
        /// connect to domain
        /// http://msdn.microsoft.com/zh-cn/library/system.directoryservices.directoryentry.path(v=vs.90).aspx
        /// </summary>
        /// <param name="domainName">root domain name or IP</param>
        /// <param name="userName">user name</param>
        /// <param name="userPwd">password</param>
        /// <param name="exchangedomainname">sub domain name for exchange</param>
        /// <returns></returns>
        public bool Connect(string domainName, string userName, string userPwd, string exchangedomainname)
        {
            string[] arr = userName.Split('@');  // get user id from possible email address
            userName = arr[0];

            DirectoryEntry domain = new DirectoryEntry();
            try
            {
                domain.Path = string.Format("LDAP://{0}", domainName);
                domain.Username = userName;
                domain.Password = userPwd;
                domain.AuthenticationType = AuthenticationTypes.Secure;
                domain.RefreshCache();

                // login successfully, and save
                this.DomainName = domainName;
                this.Domain = domain;
                this.Username = userName;

                // further get exchange OU
                this.ExchangeDomain = GetChildOU(domain, exchangedomainname);
                this.ExchangeDomainName = exchangedomainname;

                return true;
            }
            catch (Exception ex)
            {
                LogRecord.WriteLog("AD Connect error:" + ex.Message);
                return false;
            }
        }


        public AdGroup FindGroup(string groupName)
        {
            string searchName = groupName.Trim('*');

            DirectorySearcher dirSearcher = new DirectorySearcher(this.ExchangeDomain, string.Format("(&(objectclass=organizationalUnit)(name={0}))", searchName));
            //DirectorySearcher mySearcher = new DirectorySearcher(this.ExchangeDomain, "(objectclass=organizationalUnit)"); //查询组织单位                 
            SearchResult result = dirSearcher.FindOne();
            if (result == null) return null;

            AdGroup group = new AdGroup(groupName); // create a group

            // search members
            DirectoryEntry groupRoot = result.GetDirectoryEntry();
            Dictionary<string,AdNode> members = SyncRootOU(groupRoot);  // recursively syn
            foreach (var item in members.Values)
            {
                group.Members.Add(item);
            }

            return group;
        }

        public string FindEmailAddress(string userName)
        {
            DirectorySearcher dirSearcher = new DirectorySearcher(this.ExchangeDomain, string.Format("(&(objectclass=person)(cn={0}))", userName));

            //dirSearcher->PropertiesToLoad->Add("mail");

            SearchResult result = dirSearcher.FindOne();

            if ((result != null) && (result.Properties.Contains("mail")))
                return result.Properties["mail"][0].ToString();

            return "";
        }


        //foreach (string property in subEntry.Properties.PropertyNames)
        //{
        //    LogRecord.WriteLog(string.Format("字段名: {0}   字段值：{1}\r\n", property, subEntry.Properties[property][0].ToString()));
        //}


        public List<string> GetAllEmails()
        {
            // synchronize all user information
            List<AdNode> result = GetAllUsers();

            List<string> emails = new List<string>();
            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.Email))
                {
                    emails.Add(item.Email);
                }
            }
            return emails;
        }


        public List<string> GetAllGroups()
        {
            // synchronize all user information
            List<AdNode> result = GetAllUsers();

            List<string> groups = new List<string>();
            foreach (var item in result)
            {
                if ( item.Type == AdType.OU )
                {
                    groups.Add("*" + item.Name);
                }
            }
            return groups;
        }


        /// <summary>
        /// synchronize all users
        /// </summary>
        /// <param name="entryOU"></param>
        public List<AdNode> GetAllUsers()
        {
            /*
             * refer to：http://msdn.microsoft.com/zh-cn/library/system.directoryservices.directorysearcher.filter(v=vs.80).aspx
             * 
             * -----------------其它------------------------------             
             * 机算机：       (objectCategory=computer)
             * 组：           (objectCategory=group)
             * 联系人：       (objectCategory=contact)
             * 共享文件夹：   (objectCategory=volume)
             * 打印机         (objectCategory=printQueue)
             * ---------------------------------------------------
             */
            DirectorySearcher mySearcher = new DirectorySearcher(this.ExchangeDomain, "(objectclass=organizationalUnit)"); //查询组织单位                 

            DirectoryEntry root = mySearcher.SearchRoot;   //查找根OU

            Dictionary<string,AdNode> list = SyncRootOU(root);
            List<AdNode> result = new List<AdNode>();
            foreach (var item in list.Values)
            {
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// synchronize root OU, return both users and sub-OUs
        /// </summary>
        /// <param name="entry"></param>
        private Dictionary<string, AdNode> SyncRootOU(DirectoryEntry entry)
        {
            Dictionary<string, AdNode> result = new Dictionary<string, AdNode>(); // key = id

            AdNode root = new AdNode(entry, "");
            if (root.Type == AdType.OU)
            {
                result[ root.Id ] = root;
                SyncSubOU(result, entry, root.Id);
            }

            return result;
        }


        /// <summary>
        /// synchronize sub OU and all its users recursively
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="parentId"></param>
        private void SyncSubOU( Dictionary<string, AdNode> result, DirectoryEntry entry, string parentId)
        {
            foreach (DirectoryEntry subEntry in entry.Children)
            {
                AdNode adn = new AdNode(subEntry, parentId);
                if (adn.Type != AdType.OTHER)
                {
                    // add without duplication
                    if (! result.ContainsKey(adn.Id) )
                    {
                        result[adn.Id] = adn; 
                    }

                    if (adn.Type == AdType.OU)
                    {
                        SyncSubOU(result, subEntry, adn.Id);
                    }
                }
            }
        }




        /// <summary>
        /// get child OU by name
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="ou"></param>
        /// <returns></returns>
        private DirectoryEntry GetChildOU(DirectoryEntry entry, string ouName)
        {
            DirectoryEntry ou = null;
            try
            {
                ou = entry.Children.Find("OU=" + ouName.Trim());
                return ou;
            }
            catch (Exception ex)
            {
                LogRecord.WriteLog("AD GetChildOU error：" + ex.Message);
                return null;
            }
        }





    }
}
