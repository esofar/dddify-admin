declare namespace API {
  type AccountLoginRequest = {
    /** 账号，支持邮箱或手机号。 */
    account: string;
    /** 密码。 */
    password: string;
    /** 设备ID。 */
    deviceId: string;
    /** 设备名称。 */
    deviceName: string;
    /** 是否保持登录。 */
    rememberMe: boolean;
  };

  type ApiResult = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
  };

  type ApiResultOfCurrentUserDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | CurrentUserDto;
  };

  type ApiResultOfDepartmentDetailDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | DepartmentDetailDto;
  };

  type ApiResultOfIEnumerableOfDepartmentListDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfIEnumerableOfLookupActiveItemDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfIEnumerableOfLookupItemDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfIEnumerableOfPermissionDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfIEnumerableOfRoleListDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfIEnumerableOfRolePermissionDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfIEnumerableOfUserRoleDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfPagedResultOfLookupDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | PagedResultOfLookupDto;
  };

  type ApiResultOfPagedResultOfRoleListDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | PagedResultOfRoleListDto;
  };

  type ApiResultOfPagedResultOfUserListDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | PagedResultOfUserListDto;
  };

  type ApiResultOfRoleDetailDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | RoleDetailDto;
  };

  type ApiResultOfstring = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: any;
  };

  type ApiResultOfUserDetailDto = {
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
    data?: null | UserDetailDto;
  };

  type ApiResultWithErrors = {
    errors?: any;
    success?: boolean;
    errorCode?: any;
    errorMessage?: any;
    traceId?: any;
  };

  type AssignRolePermissionsParams = {
    /** 角色ID。 */
    id: string;
  };

  type AssignUserRolesParams = {
    /** 用户ID。 */
    id: string;
  };

  type CreateDepartmentRequest = {
    /** 上级部门ID。 */
    parentId: any;
    /** 部门名称。 */
    name: string;
    /** 部门类型。 */
    type: string;
    /** 部门负责人ID。 */
    leaderId: string;
    /** 是否启用。 */
    isEnabled: boolean;
    /** 排序值。 */
    order: number;
  };

  type CreateLookupItemParams = {
    /** 字典ID。 */
    id: string;
  };

  type CreateLookupItemRequest = {
    /** 字典项值。 */
    value: string;
    /** 字典项标签。 */
    label: string;
    /** 显示颜色。 */
    color: any;
  };

  type CreateLookupRequest = {
    /** 字典编码。 */
    code: string;
    /** 字典名称。 */
    name: string;
    /** 字典描述。 */
    description: any;
  };

  type CreateOrUpdatePermissionRequest = {
    /** 上级权限ID。 */
    parentId: any;
    /** 权限标识。 */
    code: string;
    /** 权限名称。 */
    name: string;
    /** 权限类型。 */
    type: string;
    /** 排序值。 */
    order: number;
  };

  type CreateRoleRequest = {
    /** 角色名称。 */
    name: string;
    /** 是否默认角色。 */
    isDefault: boolean;
    /** 排序值。 */
    order: number;
    /** 角色描述。 */
    description: string;
  };

  type CreateUserRequest = {
    /** 密码。 */
    password: string;
    /** 姓名。 */
    name: string;
    /** 昵称。 */
    nickName: any;
    /** 性别。 */
    gender: string;
    /** 出生日期。 */
    birthDate: any;
    /** 邮箱。 */
    email: string;
    /** 手机号。 */
    phoneNumber: string;
    /** 部门ID。 */
    departmentId: string;
  };

  type CurrentUserDto = {
    id: string;
    name: string;
    nickName: any;
    avatar: any;
    email: string;
    phoneNumber: string;
    currentRoleId: string;
    roles: UserRoleDto[];
    permissions: string[];
  };

  type DeleteDepartmentParams = {
    /** 部门ID。 */
    id: string;
  };

  type DeleteLookupParams = {
    /** 字典ID。 */
    id: string;
  };

  type DeletePermissionParams = {
    /** 权限ID。 */
    id: string;
  };

  type DeleteRoleParams = {
    /** 角色ID。 */
    id: string;
  };

  type DeleteUserParams = {
    /** 用户ID。 */
    id: string;
  };

  type DepartmentDetailDto = {
    concurrencyStamp: any;
    id: string;
    parentId: any;
    name: string;
    fullName: string;
    type: any;
    leader: DepartmentLeaderDto;
    isEnabled: boolean;
    order: number;
  };

  type DepartmentLeaderDto = {
    id: string;
    name: string;
  };

  type DepartmentListDto = {
    id: string;
    parentId: any;
    name: string;
    fullName: string;
    type: any;
    leader: DepartmentLeaderDto;
    isEnabled: boolean;
    order: number;
  };

  type DisableLookupItemParams = {
    /** 字典ID。 */
    id: string;
    /** 字典项ID。 */
    itemId: string;
  };

  type DisableUserParams = {
    /** 用户ID。 */
    id: string;
  };

  type EnableLookupItemParams = {
    /** 字典ID。 */
    id: string;
    /** 字典项ID。 */
    itemId: string;
  };

  type EnableUserParams = {
    /** 用户ID。 */
    id: string;
  };

  type GetDepartmentDetailParams = {
    /** 部门ID。 */
    id: string;
  };

  type GetLookupActiveItemsParams = {
    /** 字典编码。 */
    code?: string;
  };

  type GetLookupItemsParams = {
    /** 字典ID。 */
    id: string;
  };

  type GetRoleDetailParams = {
    /** 角色ID。 */
    id: string;
  };

  type GetRolePermissionsParams = {
    /** 角色ID。 */
    id: string;
  };

  type GetUserDetailParams = {
    /** 用户ID。 */
    id: string;
  };

  type GetUserRolesParams = {
    /** 用户ID。 */
    id: string;
  };

  type LookupActiveItemDto = {
    value: string;
    label: string;
    color: string;
  };

  type LookupDto = {
    id: string;
    code: string;
    name: string;
    description: any;
  };

  type LookupItemDto = {
    id: string;
    value: string;
    label: string;
    color: any;
    order: number;
    isPreset: boolean;
    isEnabled: boolean;
  };

  type PagedResultOfLookupDto = {
    total: number;
    items: LookupDto[];
  };

  type PagedResultOfRoleListDto = {
    total: number;
    items: RoleListDto[];
  };

  type PagedResultOfUserListDto = {
    total: number;
    items: UserListDto[];
  };

  type PermissionDto = {
    id: string;
    parentId: any;
    code: string;
    name: string;
    type: string;
    order: number;
  };

  type ResetUserPasswordParams = {
    /** 用户ID。 */
    id: string;
  };

  type RoleDetailDto = {
    concurrencyStamp: any;
    id: string;
    name: string;
    description: any;
    isPreset: boolean;
    isDefault: boolean;
    assignedUserCount: number;
    order: number;
  };

  type RoleListDto = {
    id: string;
    name: string;
    description: any;
    isPreset: boolean;
    isDefault: boolean;
    assignedUserCount: number;
    order: number;
  };

  type RolePermissionDto = {
    permissionId: string;
    permissionCode: string;
  };

  type SearchDepartmentsParams = {
    /** 部门名称。 */
    name?: string;
    /** 部门类型。 */
    type?: string;
    /** 启用状态。 */
    isEnabled?: boolean;
  };

  type SearchLookupsParams = {
    /** 当前页码。 */
    current?: number;
    /** 每页数量。 */
    pageSize?: number;
    /** 字典编码。 */
    code?: string;
    /** 字典名称。 */
    name?: string;
  };

  type SearchPermissionsParams = {
    /** 权限名称。 */
    name?: string;
    /** 权限标识。 */
    code?: string;
  };

  type SearchRolesParams = {
    /** 当前页码。 */
    current?: number;
    /** 每页数量。 */
    pageSize?: number;
    /** 角色名称。 */
    name?: string;
  };

  type SearchUsersParams = {
    /** 当前页码。 */
    Current?: number;
    /** 每页数量。 */
    PageSize?: number;
    /** 姓名。 */
    Name?: string;
    /** 邮箱。 */
    Email?: string;
    /** 手机号。 */
    PhoneNumber?: string;
    /** 部门ID。 */
    DepartmentId?: string;
    /** 角色ID。 */
    RoleId?: string;
    /** 性别。 */
    Gender?: string;
    /** 状态。 */
    Status?: string;
  };

  type SendLoginSmsCodeRequest = {
    /** 手机号。 */
    phoneNumber: string;
  };

  type SmsLoginRequest = {
    /** 手机号。 */
    phoneNumber: string;
    /** 短信验证码。 */
    code: string;
    /** 设备ID。 */
    deviceId: string;
    /** 设备名称。 */
    deviceName: string;
    /** 是否保持登录。 */
    rememberMe: boolean;
  };

  type SortLookupItemsParams = {
    /** 字典ID。 */
    id: string;
  };

  type SwitchUserCurrentRoleRequest = {
    /** 角色ID。 */
    roleId: string;
  };

  type UpdateDepartmentParams = {
    /** 部门ID。 */
    id: string;
  };

  type UpdateDepartmentRequest = {
    /** 上级部门ID。 */
    parentId: any;
    /** 部门名称。 */
    name: string;
    /** 部门类型。 */
    type: string;
    /** 部门负责人ID。 */
    leaderId: string;
    /** 是否启用。 */
    isEnabled: boolean;
    /** 排序值。 */
    order: number;
    /** 并发标记。 */
    concurrencyStamp: any;
  };

  type UpdateLookupItemParams = {
    /** 字典ID。 */
    id: string;
    /** 字典项ID。 */
    itemId: string;
  };

  type UpdateLookupItemRequest = {
    /** 字典项标签。 */
    label: string;
    /** 显示颜色。 */
    color: any;
  };

  type UpdateLookupParams = {
    /** 字典ID。 */
    id: string;
  };

  type UpdateLookupRequest = {
    /** 字典名称。 */
    name: string;
    /** 字典描述。 */
    description: any;
  };

  type UpdatePermissionParams = {
    /** 权限ID。 */
    id: string;
  };

  type UpdateRoleParams = {
    /** 角色ID。 */
    id: string;
  };

  type UpdateRoleRequest = {
    /** 角色名称。 */
    name: string;
    /** 是否默认角色。 */
    isDefault: boolean;
    /** 排序值。 */
    order: number;
    /** 角色描述。 */
    description: string;
    /** 并发标记。 */
    concurrencyStamp: any;
  };

  type UpdateUserParams = {
    /** 用户ID。 */
    id: string;
  };

  type UpdateUserRequest = {
    /** 姓名。 */
    name: string;
    /** 昵称。 */
    nickName: any;
    /** 性别。 */
    gender: string;
    /** 出生日期。 */
    birthDate: any;
    /** 邮箱。 */
    email: string;
    /** 手机号。 */
    phoneNumber: string;
    /** 部门ID。 */
    departmentId: string;
    /** 并发标记。 */
    concurrencyStamp: any;
  };

  type UserDepartmentDto = {
    id: string;
    name: string;
  };

  type UserDetailDto = {
    concurrencyStamp: any;
    id: string;
    name: string;
    nickName: any;
    avatar: any;
    email: string;
    phoneNumber: string;
    birthDate: any;
    gender: string;
    status: string;
    department: UserDepartmentDto;
    roles: UserRoleDto[];
  };

  type UserListDto = {
    id: string;
    name: string;
    nickName: any;
    avatar: any;
    email: string;
    phoneNumber: string;
    birthDate: any;
    gender: string;
    status: string;
    department: UserDepartmentDto;
    roles: UserRoleDto[];
  };

  type UserRoleDto = {
    roleId: string;
    roleName: string;
    isCurrent: boolean;
  };
}
