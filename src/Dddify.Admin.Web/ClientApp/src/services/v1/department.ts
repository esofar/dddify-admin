// @ts-ignore
/* eslint-disable */
import { request } from "@umijs/max";

/** 查询部门列表。 权限标识：<code>system:department:index</code> GET /api/v1/departments */
export async function searchDepartments(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.SearchDepartmentsParams,
  options?: { [key: string]: any }
) {
  return request<API.ApiResultOfIEnumerableOfDepartmentListDto>(
    "/api/v1/departments",
    {
      method: "GET",
      params: {
        ...params,
      },
      ...(options || {}),
    }
  );
}

/** 新增部门。 权限标识：<code>system:department:create</code> POST /api/v1/departments */
export async function createDepartment(
  body: API.CreateDepartmentRequest,
  options?: { [key: string]: any }
) {
  return request<API.ApiResult>("/api/v1/departments", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    data: body,
    ...(options || {}),
  });
}

/** 获取部门详情。 权限标识：<code>system:department:index</code> GET /api/v1/departments/${param0} */
export async function getDepartmentDetail(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.GetDepartmentDetailParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResultOfDepartmentDetailDto>(
    `/api/v1/departments/${param0}`,
    {
      method: "GET",
      params: { ...queryParams },
      ...(options || {}),
    }
  );
}

/** 修改部门。 权限标识：<code>system:department:update</code> PUT /api/v1/departments/${param0} */
export async function updateDepartment(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.UpdateDepartmentParams,
  body: API.UpdateDepartmentRequest,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/departments/${param0}`, {
    method: "PUT",
    headers: {
      "Content-Type": "application/json",
    },
    params: { ...queryParams },
    data: body,
    ...(options || {}),
  });
}

/** 删除部门。 权限标识：<code>system:department:delete</code> DELETE /api/v1/departments/${param0} */
export async function deleteDepartment(
  // 叠加生成的Param类型 (非body参数swagger默认没有生成对象)
  params: API.DeleteDepartmentParams,
  options?: { [key: string]: any }
) {
  const { id: param0, ...queryParams } = params;
  return request<API.ApiResult>(`/api/v1/departments/${param0}`, {
    method: "DELETE",
    params: { ...queryParams },
    ...(options || {}),
  });
}

/** 获取所有部门。 GET /api/v1/departments/all */
export async function getAllDepartments(options?: { [key: string]: any }) {
  return request<API.ApiResultOfIEnumerableOfDepartmentListDto>(
    "/api/v1/departments/all",
    {
      method: "GET",
      ...(options || {}),
    }
  );
}
