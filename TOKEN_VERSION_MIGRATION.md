# Token Version Migration Guide

## Overview
A new `token_version` column has been added to the `users` table to enable immediate invalidation of all active JWT tokens when "Logout All Sessions" is used.

## Database Changes

### Add Column to Users Table

If you're using migrations, create a migration. If using `EnsureCreated()`, the column will be added automatically on next run.

**Manual SQL (if needed):**
```sql
ALTER TABLE users ADD COLUMN token_version BIGINT DEFAULT 0;
```

## How It Works

1. **Token Generation**: Each JWT access token now includes a `token_version` claim that matches the user's current `TokenVersion` in the database.

2. **Token Validation**: The `JwtCookieMiddleware` checks if the token's version matches the user's current version in the database. If they don't match, the token is rejected.

3. **Logout All Sessions**: When "Logout All Sessions" is clicked:
   - All active refresh tokens are revoked
   - The user's `TokenVersion` is incremented
   - All existing access tokens become invalid immediately (even if not expired)

## Benefits

- **Immediate Invalidation**: Access tokens are invalidated instantly, not after expiration
- **Security**: Prevents unauthorized access even if tokens are stolen
- **Backward Compatible**: Old tokens without version claims are handled gracefully

## Testing

1. Log in on multiple devices
2. Click "Logout All Sessions" from one device
3. Verify that all other devices are logged out immediately
4. Verify that new login works correctly
