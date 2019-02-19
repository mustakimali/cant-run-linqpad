namespace CantRunLinqPad.Core
{
    public interface IDumper
    {
        bool CanDump(object obj);
        void Dump(object obj);
    }
}
