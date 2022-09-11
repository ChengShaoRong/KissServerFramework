
namespace KissServerFramework
{
    public sealed class Account : AccountBase
    {

        public enum AccountType
        {
            BuildIn,//Build-in account
            ThirdParty_Test,//Sample for test third party account.
                            //Login flow :
                            //Client got uid and token from third party SDK. 
                            //-> send to our server.
                            //-> our server confirm that uid and token from third party server by HTTP(s).
                            //-> login success/fail
        }
    }
}
