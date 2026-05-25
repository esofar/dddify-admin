import type { ActionType } from '@ant-design/pro-components';
import { PageContainer } from '@ant-design/pro-components';
import { FormattedMessage } from '@umijs/max';
import { Empty, Space, Typography, theme } from 'antd';
import React, { useCallback, useRef, useState } from 'react';
import LookupItemList from './components/LookupItemList';
import LookupList from './components/LookupList';

const { Text } = Typography;

const LookupPage: React.FC = () => {
  const { token } = theme.useToken();
  const [selectedLookup, setSelectedLookup] = useState<API.LookupDto>();

  const actionRefLookup = useRef<ActionType | null>(null);
  const actionRefLookupItem = useRef<ActionType | null>(null);

  const handleLookupLoaded = useCallback(
    (list?: API.LookupDto[]) => {
      if (list && list.length > 0) {
        if (!selectedLookup || !list.find((x) => x.id === selectedLookup.id)) {
          setSelectedLookup(list[0]);
        }
      } else {
        setSelectedLookup(undefined);
      }
      actionRefLookupItem.current?.reload();
    },
    [selectedLookup],
  );

  const handleSelect = useCallback((row: API.LookupDto) => {
    setSelectedLookup(row);
    actionRefLookupItem.current?.reload();
  }, []);

  return (
    <PageContainer title={false}>
      <div
        style={{
          display: 'grid',
          gridTemplateColumns: '35% minmax(0, 1fr)',
          gap: token.margin,
        }}
      >
        <section>
          <LookupList
            actionRef={actionRefLookup}
            selectedRowKey={selectedLookup?.id}
            onLoaded={handleLookupLoaded}
            onSelect={handleSelect}
            onSuccess={() => {
              actionRefLookup.current?.reload();
            }}
          />
        </section>
        <section>
          {selectedLookup ? (
            <LookupItemList
              lookup={selectedLookup}
              actionRef={actionRefLookupItem}
              onSuccess={() => {
                actionRefLookupItem.current?.reload();
              }}
            />
          ) : (
            <Empty
              image={Empty.PRESENTED_IMAGE_SIMPLE}
              style={{ padding: '80px 0' }}
            />
          )}
        </section>
      </div>
    </PageContainer>
  );
};

export default LookupPage;
