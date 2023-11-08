const ACCESS_TOKRN_KEY = 'dddify-access-token';
const REFRESH_TOKRN_KEY = 'dddify-refresh-token';

export type TTree<T> = {
  children?: TTree<T>[];
} & T;

/**
 * 保存令牌
 */
export function saveToken(accessToken?: string, refreshToken?: string) {
  localStorage.setItem(ACCESS_TOKRN_KEY, accessToken || '');
  localStorage.setItem(REFRESH_TOKRN_KEY, refreshToken || '');
};

/**
 * 删除令牌
 */
export function removeToken() {
  localStorage.removeItem(ACCESS_TOKRN_KEY);
  localStorage.removeItem(REFRESH_TOKRN_KEY);
};

/**
 * 获取访问令牌
 */
export function getAccessToken() {
  return (localStorage.getItem(ACCESS_TOKRN_KEY) || '') as string;
};

/**
 * 获取刷新令牌
 */
export function getRefreshToken() {
  return (localStorage.getItem(REFRESH_TOKRN_KEY) || '') as string;
};

/**
 * flat list to tree list.
 *
 * @param list - a flat list.
 * @param params - `{ id, parentId }`: id name and parentId name.
 * @example `arrayToTree<IFolder>(folderArr, { id: 'folderId', parentId: 'folderParentId' });`
 * @returns `TTree`
 */
export const arrayToTree = <T>(
  list: T[],
  { id, parentId }: { id: string; parentId: string }
): TTree<T>[] | [] => {
  /** map between id and array position */
  const map: number[] = [];
  const treeList: TTree<T>[] = list as TTree<T>[];

  for (let i = 0; i < treeList.length; i += 1) {
    /** initialize the map */
    map[(treeList[i] as TTree<T> & { [id: string]: number })[id]] = i;
  }

  let node: TTree<T> & { [parentId: string]: number };
  /** return value */
  const roots: TTree<T>[] = [];

  for (const item of treeList) {
    node = item as TTree<T> & { [parentId: string]: number };
    const parentItem = treeList[map[node[parentId]]];
    if (node[parentId] && parentItem) {
      if (!parentItem.children) {
        parentItem.children = [];
      }
      parentItem.children?.push(node);
    } else {
      roots.push(node);
    }
  }
  return roots;
};