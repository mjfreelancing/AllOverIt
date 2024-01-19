namespace ObservableDemo.Entities
{
    // A non-observable model
    internal sealed class PersonEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                if (FirstName is null && LastName is null)
                {
                    return null;
                }

                if (FirstName is null || LastName is null)
                {
                    return FirstName ?? LastName;
                }

                return $"{FirstName} {LastName}";
            }
        }
    }
}