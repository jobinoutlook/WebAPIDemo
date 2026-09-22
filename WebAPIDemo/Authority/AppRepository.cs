namespace WebAPIDemo.Authority
{
    public class AppRepository
    {
        private static List<Application> _application = new List<Application>()
        {
            new Application
            {
                ApplicationId = 1,
                ApplicationName="MVCWebApp",
                ClientId="BF97CC45-1831-49A6-AB39-DEB9C4A03ED7",
                Secret="7697211C-397D-4B83-90EE-B1D7695C6027",
                Scopes="read,write"
            }

        };

        

        public static Application? GetApplicationByClientId(string clientId)
        {
            return _application.FirstOrDefault(x => x.ClientId == clientId);
        }


    }
}
