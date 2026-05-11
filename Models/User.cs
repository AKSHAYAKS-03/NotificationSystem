namespace Models
{
    
    public partial class User
    {
        //encapsulation - no direct access to fields, only through properties
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;

        public int Id { get; set; }

        public string Name
        {
            get{return _name;}
            set{_name = value?.Trim() ?? string.Empty;}

        }
        public string Email
        {
            get{return _email;}
            set{_email = value?.Trim() ?? string.Empty;}
        }
        public string Phone
        {
            get{return _phone;}
            set{_phone = value?.Trim() ?? string.Empty;}
        }
        
    }
}