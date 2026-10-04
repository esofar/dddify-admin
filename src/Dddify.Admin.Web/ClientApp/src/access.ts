/**
 * @see https://umijs.org/docs/max/access#access
 * */
export default function access(
  initialState: { currentUser?: API.CurrentUserDto } | undefined,
) {
  const { currentUser } = initialState ?? {};
  const userPermissions = currentUser?.permissions || [];
  const userPermissionsSet = new Set(userPermissions);
  const accessMap: Record<string, boolean> = {};

  userPermissions.forEach((permissionCode) => {
    accessMap[permissionCode] = true;
  });

  return {
    canAdmin: userPermissionsSet.has('*'),
    has: (permissionCode: string) => userPermissionsSet.has(permissionCode),
    ...accessMap,
  };
}
