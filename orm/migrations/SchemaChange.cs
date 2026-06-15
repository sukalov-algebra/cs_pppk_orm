namespace orm.migrations
{
    public enum SchemaChangeKind
    {
        CreateTable,
        AddColumn,
        DropColumn
    }

    public sealed class SchemaChange
    {
        public SchemaChangeKind Kind { get; }
        public string Name { get; }
        public string Up { get; }
        public string Down { get; }

        public SchemaChange(SchemaChangeKind kind, string name, string up, string down)
        {
            Kind = kind;
            Name = name;
            Up = up;
            Down = down;
        }
    }
}
