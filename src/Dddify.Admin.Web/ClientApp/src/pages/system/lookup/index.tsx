import type { ActionType } from '@ant-design/pro-components';
import { PageContainer, ProCard } from '@ant-design/pro-components';
import React, { useCallback, useRef, useState } from 'react';
import LookupItemList from './components/LookupItemList';
import LookupList from './components/LookupList';

const LookupPage: React.FC = () => {
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
      <ProCard split="vertical">
        <ProCard colSpan="50%" styles={{ body: { padding: 0, paddingTop: 8 } }}>
          <LookupList
            actionRef={actionRefLookup}
            selectedRowKey={selectedLookup?.id}
            onLoaded={handleLookupLoaded}
            onSelect={handleSelect}
            onSuccess={() => {
              actionRefLookup.current?.reload();
            }}
          />
        </ProCard>
        <ProCard colSpan="50%" styles={{ body: { padding: 0, paddingTop: 8 } }}>
          <LookupItemList
            lookup={selectedLookup}
            actionRef={actionRefLookupItem}
            onSuccess={() => {
              actionRefLookupItem.current?.reload();
            }}
          />
        </ProCard>
      </ProCard>
    </PageContainer>
  );
};

export default LookupPage;
