import { FormattedMessage } from '@umijs/max';

export type DepartmentTreeNode = API.DepartmentListDto & {
  children?: DepartmentTreeNode[];
};

export type DepartmentSelectNode = {
  id: string;
  parentId?: string | null;
  name: string;
  disabled?: boolean;
};

export const departmentStatusValueEnum = {
  true: {
    text: (
      <FormattedMessage id="common.button.enable" defaultMessage="Enable" />
    ),
    status: 'Success',
  },
  false: {
    text: (
      <FormattedMessage id="common.button.disable" defaultMessage="Disable" />
    ),
    status: 'Default',
  },
} as const;

export function buildDepartmentTree(
  departments: API.DepartmentListDto[],
): DepartmentTreeNode[] {
  const map = new Map<string, DepartmentTreeNode>();
  const roots: DepartmentTreeNode[] = [];

  departments.forEach((department) => {
    map.set(department.id, { ...department });
  });

  departments.forEach((department) => {
    const node = map.get(department.id);

    if (!node) {
      return;
    }

    if (department.parentId && map.has(department.parentId)) {
      const parent = map.get(department.parentId);

      if (parent) {
        parent.children ??= [];
        parent.children.push(node);
      }

      return;
    }

    roots.push(node);
  });

  return roots;
}

export function toDepartmentSelectNodes(
  departments: API.DepartmentListDto[],
): DepartmentSelectNode[] {
  return departments.map((department) => ({
    id: department.id,
    parentId: department.parentId,
    name: department.name,
    disabled: !department.isEnabled,
  }));
}

export function toDepartmentTypeValueEnum(
  items: API.LookupActiveItemDto[],
): Record<string, { text: string; color?: string }> {
  return items.reduce<Record<string, { text: string; color?: string }>>(
    (valueEnum, item) => {
      valueEnum[item.value] = {
        text: item.label,
        color: item.color,
      };

      return valueEnum;
    },
    {},
  );
}
