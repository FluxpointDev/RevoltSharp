namespace RevoltSharp;

public class AccountMFA
{
    internal AccountMFA(AccountMFAJson json)
    {
        EmailOTP = json.email_otp;
        TrustedHandover = json.trusted_handover;
        EmailMFA = json.email_mfa;
        TotpMFA = json.totp_mfa;
        SecurityKeyMFA = json.security_key_mfa;
        RecoveryActive = json.recovery_active;
    }

    public bool EmailOTP;
    public bool TrustedHandover;
    public bool EmailMFA;
    public bool TotpMFA;
    public bool SecurityKeyMFA;
    public bool RecoveryActive;
}
