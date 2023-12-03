﻿namespace Infrastructure.Authentication;

public static class AuthConstants
{
    public const string AdminUserPolicyName = "Admin";
    public const string AdminUserClaimName = "admin";
    
    public const string TrustedMemberPolicyName = "TrustedMember";
    public const string TrustedMemberClaimName = "trusted_member";

    public const string ApiKeyHeaderName = "x-api-key";
}
