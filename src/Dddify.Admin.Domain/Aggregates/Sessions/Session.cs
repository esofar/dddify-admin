namespace Dddify.Admin.Domain.Aggregates.Sessions;

/// <summary>
/// 用户会话聚合根。
/// </summary>
public class Session : AggregateRoot<Guid>
{
    public const int MaxDeviceIdLength = 100;
    public const int MaxDeviceNameLength = 100;
    public const int MaxIpAddressLength = 45;
    public const int MaxUserAgentLength = 500;
    public const int MaxRefreshTokenHashLength = 88;
    public const int MaxRevokedReasonLength = 100;

    /// <summary>
    /// 用户ID。
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// 设备ID。
    /// </summary>
    public string DeviceId { get; private set; } = default!;

    /// <summary>
    /// 设备名称。
    /// </summary>
    public string DeviceName { get; private set; } = default!;

    /// <summary>
    /// IP地址。
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// 用户代理字符串，通常来源于 HTTP 请求头 User-Agent。
    /// </summary>
    public string? UserAgent { get; private set; }

    /// <summary>
    /// 当前刷新令牌哈希。
    /// </summary>
    public string RefreshTokenHash { get; private set; } = default!;

    /// <summary>
    /// 上一个刷新令牌哈希，用于检测令牌轮换后的复用。
    /// </summary>
    public string? PreviousRefreshTokenHash { get; private set; }

    /// <summary>
    /// 是否持久会话。
    /// </summary>
    public bool IsPersistent { get; private set; }

    /// <summary>
    /// 刷新令牌过期时间。
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }

    /// <summary>
    /// 最近使用时间。
    /// </summary>
    public DateTimeOffset? LastSeenAt { get; private set; }

    /// <summary>
    /// 撤销时间。
    /// </summary>
    public DateTimeOffset? RevokedAt { get; private set; }

    /// <summary>
    /// 撤销原因。
    /// </summary>
    public string? RevokedReason { get; private set; }

    private Session() { }

    public Session(
        Guid id,
        Guid userId,
        string deviceId,
        string deviceName,
        string? ipAddress,
        string? userAgent,
        string refreshTokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset loggedInAt,
        bool isPersistent)
    {
        Id = id;
        UserId = userId;
        DeviceId = deviceId;
        DeviceName = deviceName;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        RefreshTokenHash = refreshTokenHash;
        IsPersistent = isPersistent;
        ExpiresAt = expiresAt;
        LastSeenAt = loggedInAt;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return RevokedAt is null && ExpiresAt > now;
    }

    public bool MatchesCurrentToken(string refreshTokenHash)
    {
        return RefreshTokenHash == refreshTokenHash;
    }

    public bool MatchesPreviousToken(string refreshTokenHash)
    {
        return PreviousRefreshTokenHash == refreshTokenHash;
    }

    public void RotateRefreshToken(string refreshTokenHash, DateTimeOffset expiresAt, DateTimeOffset now)
    {
        PreviousRefreshTokenHash = RefreshTokenHash;
        RefreshTokenHash = refreshTokenHash;
        ExpiresAt = expiresAt;
        LastSeenAt = now;
    }

    public void Revoke(string reason, DateTimeOffset now)
    {
        if (RevokedAt is not null)
        {
            return;
        }

        RevokedAt = now;
        RevokedReason = reason;
    }
}
