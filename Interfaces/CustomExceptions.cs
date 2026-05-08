using System;

namespace Interfaces
{

    //notification related exceptions
    public class NotificationException : Exception
    {
        public NotificationException(string message) : base(message) { }
        //when one exception happens because of another we use inner exception 
        public NotificationException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

//validation exceptions
    public class ValidationException : NotificationException
    {
        public ValidationException(string message) : base(message) { }
    }

//notification sending related exceptions
    public class NotificationSendException : NotificationException
    {
        public NotificationSendException(string message) : base(message) { }
        public NotificationSendException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

    //data exception 

    public class DataAccessException : NotificationException
    {
        public DataAccessException(string message) : base(message) { }
        public DataAccessException(string message, Exception innerException) 
            : base(message, innerException) { }
    }

//input related exceptions
    public class InvalidInputException : NotificationException
    {
        public InvalidInputException(string message) : base(message) { }
    }
}
