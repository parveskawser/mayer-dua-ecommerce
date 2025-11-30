using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework.Utils
{
    public abstract class ConstantCollection
    {
        public static readonly string DEFAULT_CULTURE_KEY = System.Configuration.ConfigurationManager.AppSettings["DefaultLanguage"];
        public const string CULTURE_KEY = "lang";
        public const string CULTURE_DIRECTION = "dir";
        public const string CURRENT_CLIENT_CONTEXT_KEY = "CRS.Web.Client";
        public const string CULTURE_ID = "culture_id";
        public const string CULTURE_CLASS = "CULTURE_CLASS";

        public const string MESSAGESESSIONKEY = "MyMessage";
        public const string MESSAGEBOXSESSIONKEY = "MyMessageBox";
        public const string ERRORSESSIONKEY = "MyError";
        public const string THEMESESSIONKEY = "MyTheme";
        public const string INSERTSUCCESS_MESSEGEKEY = "InsertSuccessMessege";
        public const string INSERTSUCCESS_MESSEGEVALUE = "Successfully Saved";
        public const string UPDATESUCCESS_MESSEGEKEY = "UpdateSuccessMessege";
        public const string UPDATESUCCESS_MESSEGEVALUE = "Successfully Updated";

        //public const string PERMISSION_COLLECTION = "Permission_Collection";

        #region Messages
        //contsant messages. it does not mean these are fixed for all languages. these are stored here
        //only because some messages are used repeatedly in many pages. to keep consistency, these 
        //messages are stored as constants
        public const string REQUIRED_MESSAGE_SUMMARY_KEY = "REQUIRED_MESSAGE_SUMMARY_KEY";
        public const string REQUIRED_MESSAGE_SUMMARY_VALUE = "Data Modified please Update/Cancel before continuing.";
        public const string ADUSER_MISSING_KEY = "ADUSER_MISSING_KEY";
        public const string ADUSER_MISSING_VALUE = "User does not exist in the current domain.";

        public const string REQUIRED_MESSAGE_DETAIL_KEY = "REQUIRED_MESSAGE_DETAIL_KEY";
        public const string REQUIRED_MESSAGE_DETAIL_VALUE = "Please check required information or complete unsaved operation.";

        public const string REQUIRED_MESSAGE_DETAIL_VALUE_FOR_PROJECT = "Please check required information. Start Date should be greater than End Date.";

        public const string DUPLICATE_OBJECT_MESSAGE_KEY = "DUPLICATE_MESSAGE_KEY";
        public const string DUPLICATE_OBJECT_MESSAGE_VALUE = "Object with same name already exists.";

        public const String EDIT_MENU_TEXT = "Edit";
        public const String IMAGES_ROOT_PATH = "~/App_Themes/Default/Images/";
        public const String EDIT_MENU_IMAGEURL = IMAGES_ROOT_PATH + "ico_16_merge.gif";

        public const String RAMOVE_MENU_TEXT = "Delete";
        public const String REMOVE_MENU_IMAGEURL = IMAGES_ROOT_PATH + "ico_16_delete.gif";

        public const String PRINT_MENU_TEXT = "Print";
        public const String PRINT_MENU_IMAGEURL = IMAGES_ROOT_PATH + "16_print.gif";

        public const String MENU_TARGET = "_self";

        public const String COMMEND_FIELD = "Command";
        public const String SELECTEDROW_CSSCLASS = "SelectedRow";
        #endregion
    }
}
