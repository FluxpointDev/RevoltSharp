namespace RevoltSharp;

internal class AccountMFAJson
{
    public bool email_otp { get; set; }
    public bool trusted_handover { get; set; }
    public bool email_mfa { get; set; }
    public bool totp_mfa { get; set; }
    public bool security_key_mfa { get; set; }
    public bool recovery_active { get; set; }
}
