﻿using System;

namespace RevoltSharp;


/// <summary>
/// Custom exception for the Revolt client.
/// </summary>
public class RevoltException : Exception
{
    internal RevoltException(string message, int code = 0) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// The status code error for this exception if thrown by the rest client.
    /// </summary>
    public int Code { get; internal set; }
}

/// <summary>
/// Custom exception for the Revolt rest client with code.
/// </summary>
public class RevoltRestException : RevoltException
{
    internal RevoltRestException(string message, int code, RevoltErrorType type) : base(message, code)
    {
        Type = type;
    }

    /// <summary>
    /// The type of rest error triggered.
    /// </summary>
    public RevoltErrorType Type { get; internal set; } = RevoltErrorType.Unknown;

    /// <summary>
    /// The permission require for the error <see cref="RevoltErrorType.MissingPermission"/> or <see cref="RevoltErrorType.MissingUserPermission"/>
    /// </summary>
    public string? Permission { get; internal set; }
}

/// <summary>
/// Custom exception for the Revolt rest client for permission issues.
/// </summary>
public class RevoltPermissionException : RevoltRestException
{
    internal RevoltPermissionException(string permission, int code, bool userPerm) : base(
        userPerm ? $"Request failed due to other user missing permission {permission}" : $"Request failed due to missing permission {permission}",
        code, userPerm ? RevoltErrorType.MissingUserPermission : RevoltErrorType.MissingPermission)
    {
        base.Permission = permission;
    }

    /// <inheritdoc cref="RevoltRestException.Permission"/>
    public new string Permission => base.Permission!;
}

/// <summary>
/// Custom exception for the Revolt client when user enters a missing or wrong argument.
/// </summary>
public class RevoltArgumentException : RevoltException
{
    internal RevoltArgumentException(string message) : base(message, 400)
    {

    }
}