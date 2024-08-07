namespace CineMan.Errors.ErrorConstants;

public static class UserErrors
{
    // TODO: Add more user errors
    public static Error UserNotFound = new("user.id.notfound", "User could not be found", 404);
    public static Error AgeTooSmall = new("user.age.tooyoung", "User age has to be atleast 13years", 403);
    public static Error InvalidDate = new("user.dateofbirth.invalid", "Date of birth cannot be in the future", 400);
    public static Error NewEmailBlank = new("user.newemail.blank", "Kindly enter a valid email address", 400);
    public static Error PasswordEmpty = new("user.password.empty", "Kindly enter valid password", 400);
    public static Error IncorrectOldPassword = new("user.oldpassword.incorrect", "Make sure the old password is correct with proper casing", 400);
    public static Error IncorrectNewPassword = new("user.newpassword.policynotmet", "Make sure the new password meets the password policy", 400);
    public static Error CredentialsInvalid = new("user.credentials.invalid", "Make sure the username/email and password are correct", 401);
    public static Error InvalidRefreshToken = new("user.refreshtoken.invalid", "Authenticate again!", 401);
    public static Error EmptyCredentials = new("user.register.credentialsnotprovided", "Please provide a valid email and password", 400);
    public static Error UserAlreadyExists = new("user.email.alreadyexists", "Try to login from another email or reset your password", 400);
    public static Error PasswordPolicyNotMet = new("user.password.policynotmet", "1 Uppercase, 1 Lowercase, 1 Symbol, 1 Number", 400);
    public static Error TokenInvalid = new("user.confirmationtoken.invalid", "Make sure the token specified is correct", 400);
    public static Error EmailFormatInvalid = new("user.email.formatinvalid", "Make sure you're entering the correct email address", 400);
    public static Error EmailNotConfirmed = new("user.email.notconfirmed", "Please confirm your primary email address", 400);
    public static Error EmailNotConfirmedAndPasswordReset = new("user.email.notconfirmed", "Your email wasn't confirmed, can't authorize forgot password. Please contact support", 403);
    public static Error EmailNotConfirmedAndUserExists = new("user.existingemail.notconfirmed", "A user with this email already exists in the database, please confirm the email", 403);
}