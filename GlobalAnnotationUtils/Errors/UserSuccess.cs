using GlobalAnnotationUtils.Common;

namespace GlobalAnnotationUtils.Errors
{
    public static class UserSuccess
    {

        public static readonly Error UserFound =
            new(
                200,
                "SUCCESS_LOGIN",
                "User LoggedIn Successful",
                "User Logged in is Successful");
    }
}
