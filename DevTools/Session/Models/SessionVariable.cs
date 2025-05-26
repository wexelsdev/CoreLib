namespace CoreLib.DevTools.Session.Models
{
    public class SessionVariable
    {
        internal SessionVariable(string key, object value)
        {
            Key = key;
            Value = value;
        }
    
        public string Key { get; }
        public object Value { get; set; }

        public static SessionVariable Create(string key, object value) => new SessionVariable(key, value);
    }
}