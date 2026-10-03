import React from 'react';
import { Space, Tag, Popover } from 'antd';

interface TagPopoverItem {
  key?: React.Key;
  label?: React.ReactNode;
  color?: string;
  raw?: any;
}

export interface TagPopoverProps {
  /**
   * 要展示的标签项数组
   */
  items: TagPopoverItem[];
  /**
   * 页面上最多可见的标签数量，超出部分将收起到 Popover 中
   * @default 3
   */
  maxVisible?: number;
  /**
   * 触发 Popover 的方式
   * @default 'hover'
   */
  trigger?: 'hover' | 'click';
  /**
   * Popover 弹出的位置
   * @default 'topLeft'
   */
  placement?: any;
  /**
   * 应用于每个可见标签的自定义样式
   */
  tagStyle?: React.CSSProperties;
  /**
   * Popover 内容区域的最大宽度
   * @default 260
   */
  popoverWidth?: number;
  /**
   * Popover 中是否显示全部标签（而不是仅显示隐藏部分）
   * @default true
   */
  showAllInPopover?: boolean;
  /**
   * “更多”标签的颜色
   * @default 'blue'
   */
  moreTagColor?: string;
}

const TagPopover: React.FC<TagPopoverProps> = ({
  items,
  maxVisible = 3,
  trigger = 'hover',
  placement = 'topLeft',
  tagStyle,
  popoverWidth = 260,
  showAllInPopover = true,
  moreTagColor = 'blue',
}) => {
  if (!items?.length) return null;

  if (items.length <= maxVisible) {
    return (
      <Space size={4} wrap>
        {items.map((it) => (
          <Tag
            key={it.key}
            color={it.color || 'default'}
            variant="filled"
            style={{ marginInlineEnd: 0, ...tagStyle }}
          >
            {it.label}
          </Tag>
        ))}
      </Space>
    );
  }

  const visible = items.slice(0, maxVisible);
  const hidden = items.slice(maxVisible);
  const popList = showAllInPopover ? items : hidden;

  const content = (
    <Space size={[4, 4]} wrap style={{ maxWidth: popoverWidth }}>
      {popList.map((it) => (
        <Tag key={it.key} color={it.color || 'default'} variant="filled" style={{ margin: 0 }}>
          {it.label}
        </Tag>
      ))}
    </Space>
  );

  return (
    <Space size={4} wrap>
      {visible.map((it) => (
        <Tag
          key={it.key}
          color={it.color || 'default'}
          variant="filled"
          style={{ marginInlineEnd: 0, ...tagStyle }}
        >
          {it.label}
        </Tag>
      ))}
      <Popover
        trigger={trigger}
        placement={placement}
        style={{ padding: 8, maxWidth: popoverWidth }}
        content={content}
      >
        <Tag
          color={moreTagColor}
          variant="filled"
          style={{ cursor: 'pointer', marginInlineEnd: 0 }}
        >
          +{hidden.length}
        </Tag>
      </Popover>
    </Space>
  );
};

export default TagPopover;
