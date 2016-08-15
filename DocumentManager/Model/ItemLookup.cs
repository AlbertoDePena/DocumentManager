namespace DocumentManager.Model
{
    public class ItemLookup
    {
        public int Id { get; private set; }

        public string Value { get; private set; }

        public ItemLookup WithId(int id)
        {
            Id = id;

            return this;
        }

        public ItemLookup WithValue(string value)
        {
            Value = value;

            return this;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ItemLookup)) return false;

            return ((ItemLookup) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}