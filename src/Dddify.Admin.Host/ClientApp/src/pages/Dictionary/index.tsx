import React, { useState } from 'react';
import { PageContainer, ProCard } from '@ant-design/pro-components';
import DictionaryList from "./components/DictionaryList";
import DictionaryItemList from "./components/DictionaryItemList";

const DictionaryPage: React.FC = () => {

  const [dictionary, setDictionary] = useState<Dictionary>();

  return (
    <PageContainer>
      <ProCard split="vertical">
        <ProCard colSpan={12} ghost>
          <DictionaryList
            onChange={(dict) => setDictionary(dict)}
          />
        </ProCard>
        <ProCard colSpan={12} ghost>
          <DictionaryItemList dictionary={dictionary} />
        </ProCard>
      </ProCard>
    </PageContainer>
  );
};

export default DictionaryPage;