using GlobalAnnotationUtils.Common;

namespace GlobalAnnotationUtils.Errors
{
    public static class UserErrors
    {
        public static readonly Error NotFound =
            new(
                404,
                "USER_NOT_FOUND",
                "User not found.",
                "The requested user does not exist.");

        public static readonly Error EmailAlreadyExists =
            new(
                409,
                "EMAIL_ALREADY_EXISTS",
                "Email already exists.",
                "A user with the given email already exists.");

        public static readonly Error InvalidPassword =
            new(
                401,
                "INVALID_PASSWORD",
                "Invalid password.",
                "The supplied password is incorrect.");
    }
}
