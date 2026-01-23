using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework
{
    [Serializable]
    public class Client : System.ServiceModel.IExtension<System.ServiceModel.OperationContext>
    { 
        public enum BrowserType
        {
            Other = 0,
            Netscape = 1,
            Opera = 2,
            IE4 = 3,
            IE5 = 4,
            IE6 = 5,
            IE7 = 6,
            IE8 = 7,
            FireFox = 8,
            Chrome = 9
        } 

        public string FullName = string.Empty;
        public string UserName = string.Empty;
        public int UserID;
        public int UserType = -1;
        private string _IP = string.Empty;
        public string HTTP_USER_AGENT = string.Empty;
        public string HTTP_HOST = string.Empty;
        public BrowserType Browser;
        public int UserGroupID;
        public int BranchID;
        public string BranchName; 
        public string IP
        {
            get
            {
                if (_IP == null)
                    return string.Empty;
                else
                    return _IP;
            }
            set
            {
                _IP = value;
            }
        }

        public Client(string ipAddress, string userAgent, int userID, int branchID)
        {
            IP = ipAddress;
            HTTP_HOST = ipAddress;
            HTTP_USER_AGENT = userAgent;
            UserID = userID;
            BranchID = branchID;
        }

        public override string ToString()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder(100);

            builder.Append("FullName=");
            builder.Append(FullName);

            builder.Append("|UserName=");
            builder.Append(UserName);

            builder.Append("|UserID=");
            builder.Append(UserID);

            builder.Append("|UserType=");
            builder.Append(UserType);

            builder.Append("|IP=");
            builder.Append(IP);

            builder.Append("|HTTP_USER_AGENT=");
            builder.Append(HTTP_USER_AGENT);
            builder.Append("|HTTP_HOST=");
            builder.Append(HTTP_HOST);

            builder.Append("|Browser=");
            builder.Append(Browser.ToString());

            builder.Append("|UserGroupID=");
            builder.Append(UserGroupID);

            return builder.ToString();
        }


        public void Dispose()
        {

        }

        #region IExtension<OperationContext> Members

        public void Attach(System.ServiceModel.OperationContext owner)
        {

        }

        public void Detach(System.ServiceModel.OperationContext owner)
        {

        }

        #endregion
    }
}
