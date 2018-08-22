using Mapster;

namespace Sikiro.SMS.Toolkits
{
    public static class MapperExtension
    {
        public static TResult MapTo<TFrom, TResult>(this TFrom obj)
        {
            return new Adapter().Adapt<TFrom, TResult>(obj);
        }

        public static TResult MapTo<TResult>(this object obj)
        {
            return new Adapter().Adapt<TResult>(obj);
        }
    }
}
