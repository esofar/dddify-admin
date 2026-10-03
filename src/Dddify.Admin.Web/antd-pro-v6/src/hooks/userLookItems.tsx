import { useState, useEffect } from 'react';
import type { ProSchemaValueEnumType } from '@ant-design/pro-components';
import { getLookupActiveItems } from '@/services/v1/lookup';

/**
 * 根据字典编码获取并格式化 ProTable/ProForm 所需的 valueEnum 选项。
 * @param code 字典编码，例如 'department_type'
 * @returns 返回一个包含加载状态和格式化后选项的对象
 */
export const useLookupItems = (code: string) => {
  const [loading, setLoading] = useState(false);
  const [items, setItems] = useState<Record<string, ProSchemaValueEnumType>>({});

  useEffect(() => {
    if (!code) return;
    const fetchItems = async () => {
      setLoading(true);
      try {
        const { success, data } = await getLookupActiveItems({ code });
        if (success && data) {
          const formattedItems = data.reduce((acc: { [x: string]: { text: any; status: any; color: any; }; }, item: { value: string | number; label: any; color: any; }) => {
            if (item.value) {
              acc[item.value] = {
                text: item.label,
                status: item.color,
                color: item.color,
              };
            }
            return acc;
          }, {} as Record<string, ProSchemaValueEnumType>);
          setItems(formattedItems);
        }
      } finally {
        setLoading(false);
      }
    };
    fetchItems();
  }, [code]);

  return { items, loading };
};
