namespace AuthMEANORM.Utils
{
    public class UpdateUserModels
    {
        public class UpdateUserRequest
        {
            public string Identifier { get; set; } = string.Empty;

            public List<UpdateOperation> Data { get; set; } = new();
        }

        public class UpdateOperation
        {
            public string Op { get; set; } = string.Empty; // "replace", "add", "remove", etc.
            public string Path { get; set; } = string.Empty; // The path in the object, e.g., "/userName"
            public object Value { get; set; } // The value for the operation
        }
    }
}
