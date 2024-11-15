namespace Services.Applications
{
    public class AdministratorServiceLocator
    {
        private static readonly Dictionary<Type, object> _Services = new Dictionary<Type, object>();

        public static void RegisterService<T>(T service)
        {
            _Services[typeof(T)] = service!;
        }

        public static T GetService<T>()
        {
            return (T)_Services[typeof(T)];
        }
    }
}
