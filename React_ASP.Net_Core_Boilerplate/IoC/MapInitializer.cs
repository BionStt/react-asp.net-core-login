using Service_Repository_Layer.Service.Configuration;

namespace React_ASP.Net_Core_Boilerplate.IoC
{
    public static class MapInitializer
    {
        public static void Initialize()
        {
            MapperInitialize.Configure();
        }
    }
}
