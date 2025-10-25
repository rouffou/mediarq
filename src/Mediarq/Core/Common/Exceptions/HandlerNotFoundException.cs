namespace Mediarq.Core.Common.Exceptions; 
public class HandlerNotFoundException: MediarqException {

    public HandlerNotFoundException(Type requestType)
        : base($"No handler found for request type '{requestType.FullName}'") {
        
    }
}
