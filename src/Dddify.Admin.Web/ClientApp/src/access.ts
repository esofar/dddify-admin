/**
 * @see https://umijs.org/docs/max/access#access
 * */
export default function access(
  initialState: { currentUser?: API.CurrentUserDto } | undefined,
) {
  const { currentUser } = initialState ?? {};

  // 使用 Set 存储权限，方便快速查找 O(1) 复杂度
  const userPermissions = currentUser?.permissions || [];
  const userPermissionsSet = new Set(userPermissions);

  // 为每个用户拥有的权限码动态生成对应的属性
  const accessMap: Record<string, boolean> = {};
  userPermissions.forEach((permissionCode) => {
    accessMap[permissionCode] = true;
  });

  const access = {
    canAdmin: currentUser?.permissions?.includes('*') ?? false,

    // 直接判断用户是否拥有某个权限码（不经过转换）
    has: (permissionCode: string) => userPermissionsSet.has(permissionCode),

    // 动态生成的权限属性，可以直接使用，例如 `system:user:create`
    ...accessMap,

    // 也可以保留一些固定的，不需要转换的权限判断，不建议
    // isAdmin: userRoles.includes('admin'),
    // isGuest: userRoles.includes('guest'),
  };

  return access;
}
