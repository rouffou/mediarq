namespace Mediarq.Core;

public static class ServiceFactoryExtentions
{
    public static ServiceFactory FromServiceProvider(IServiceProvider serviceProvider)
        => serviceType => serviceProvider.GetService(serviceType);
}
