using System;
using System.ComponentModel.DataAnnotations;

namespace MyBills.Core
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        [Display(Name = "Log Level")]
        public LogLevel LogLevel { get; set; }
        [Display(Name = "Date and Time")]
        public DateTime TimeStamp { get; set; }
        [Display(Name = "Method Name"), MaxLength(100)]
        public string CurrentMethod { get; set; }
        [Display(Name = "Error Message"), MaxLength(4000)]
        public string ErrorMessage { get; set; }
        [Display(Name = "Stack Trace")]
        public string StackTrace { get; set; }
        [Display(Name = "User Name"), MaxLength(50)]
        public string UserName { get; set; }
    }

    public enum LogLevel
    {
        Debug = 0,
        Warning = 1,
        Error = 2,
        Contact = 3
    }
}
