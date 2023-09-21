namespace GameData
{
    // 交互设定
    public class InteractiveSetting
    {
        public enum Type
        {
            None,
            Shop,
            Doctor,
            Teacher,
            Treasure,
        }
        public class InteractiveInfo
        {
            public Type Type;
            public uint Serial;
        }
    }
}