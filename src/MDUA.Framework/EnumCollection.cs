using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MDUA.Framework
{

    /// <summary>
    /// Message types shown in Web Admin
    /// </summary>
    public enum MessageTypeEnum
    {
        None = 0,
        Information,
        Success,
        Required,
        Warning,
        Failure,
        DatabaseConnectionError,
        PageLavelUnhandledError,
        ApplicationLavelUnhandledError
    }

    public enum MessageBoxIcons
    {
        Information = 0, Success, Required, Warning, Question, Failure, Error, Exception
    }

    public enum MessageBoxStyle
    {
        White = 0, Blue, Violet, Red   //other style enumerations here..
    }

    public enum ThemeEnum
    {
        Default = 0,
        SkyBlue
    }

    public enum ModuleEnum
    {
        General = 0,
        CarRent = 1,
        Workshop = 2,
        Accounts = 3,
        HumanResource = 4
    }
}
