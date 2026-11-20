namespace Services;
//throw when role of the user doesnt allow an action
public class NotAllowedException(string message): Exception(message);